using UnityEngine;

public class InterceptorProjectile : MonoBehaviour
{
    private Missile target;
    public float speed = 15f;
    private Rigidbody rb;

    public Vector3 initialDirection;

    [Tooltip("Rotation offset if the model faces the wrong direction (0=correct, 180=backwards, 90/-90=sideways)")]
    public float modelRotationOffset = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (rb != null)
        {
            rb.linearVelocity = initialDirection.normalized * speed;
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            Debug.LogError($"InterceptorProjectile missing Rigidbody at {transform.position}");
            Destroy(gameObject);
            return;
        }

        if (target != null)
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
        if (rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity) * Quaternion.Euler(0, modelRotationOffset, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Missile missile = other.GetComponent<Missile>();
        if (missile != null)
        {
            missile.Intercept();
            Destroy(gameObject);
        }
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
