using UnityEngine;

public class InterceptorProjectile : MonoBehaviour
{
    private Missile target;
    public float speed = 15f;
    private Rigidbody rb;

    public Vector3 initialDirection;

    [Tooltip("Rotation offset if the model faces the wrong direction (0=correct, 180=backwards, 90/-90=sideways)")]
    public float modelRotationOffset = 0f;

    [Header("Range & Gravity")]
    private Vector3 startPosition;
    private float maxRange;
    private bool isFalling = false;
    public float gravityMultiplier = 2f;
    
    [Header("Explosion")]
    public GameObject explosionPrefab;
    public LayerMask groundLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startPosition = transform.position;
        
        // Face forward immediately when spawned - do this BEFORE setting velocity
        if (initialDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(initialDirection) * Quaternion.Euler(0, modelRotationOffset, 0);
        }
        
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = initialDirection.normalized * speed;
        }
    }

    public void SetRange(float range)
    {
        maxRange = range;
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            Destroy(gameObject);
            return;
        }

        // Check if projectile has exceeded its range
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (!isFalling && distanceTraveled >= maxRange)
        {
            StartFalling();
        }

        if (isFalling)
        {
            // Apply gravity - let physics handle it
            rb.linearVelocity += Vector3.down * gravityMultiplier * Time.fixedDeltaTime * 10f;
        }
        else if (target != null)
        {
            // Homing: move toward the target
            Vector3 toTarget = (target.transform.position - rb.position).normalized;
            rb.linearVelocity = toTarget * speed;
        }
        else
        {
            // No target, just move straight
            rb.linearVelocity = initialDirection.normalized * speed;
        }

        // Rotate to face movement direction
        if (isFalling)
        {
            // Face downward when falling (180 degree flip to face down properly)
            transform.rotation = Quaternion.LookRotation(Vector3.down) * Quaternion.Euler(180, modelRotationOffset, 0);
        }
        else if (rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity) * Quaternion.Euler(0, modelRotationOffset, 0);
        }
    }

    void StartFalling()
    {
        isFalling = true;
        target = null; // Stop homing
        
        // Clear horizontal velocity so it falls straight down
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Missile missile = other.GetComponent<Missile>();
        if (missile != null)
        {
            missile.Intercept();
            Destroy(gameObject);
            return;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only react to ground when falling, ignore other collisions
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Explode();
        }
        else if (isFalling)
        {
            // Ignore collision with borders/other objects when falling
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void SetTarget(Missile missile)
    {
        target = missile;
    }
}
