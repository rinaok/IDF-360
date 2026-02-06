using UnityEngine;

public class InterceptorManager : MonoBehaviour
{
    public static InterceptorManager Instance;

    [Header("Interceptor Projectile Prefab")]
    public GameObject InterceptorProjectilePrefab;

    [Header("Outline Settings")]
    public Color outlineColor = Color.green;
    public float outlineWidth = 5f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public LayerMask groundLayer;

    [Header("Range Indicator")]
    public Color rangeIndicatorColor = new Color(0f, 1f, 0f, 0.3f);
    public int rangeIndicatorSegments = 64;
    private LineRenderer rangeIndicator;

    private Interceptor _selectedInterceptor;
    public Interceptor selectedInterceptor
    {
        get { return _selectedInterceptor; }
        set { _selectedInterceptor = value; }
    }
    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        CreateRangeIndicator();
    }

    void CreateRangeIndicator()
    {
        GameObject indicatorObj = new GameObject("RangeIndicator");
        indicatorObj.transform.SetParent(transform);
        rangeIndicator = indicatorObj.AddComponent<LineRenderer>();
        rangeIndicator.useWorldSpace = true;
        rangeIndicator.loop = true;
        rangeIndicator.positionCount = rangeIndicatorSegments;
        rangeIndicator.startWidth = 0.2f;
        rangeIndicator.endWidth = 0.2f;
        
        // Create a simple material
        rangeIndicator.material = new Material(Shader.Find("Sprites/Default"));
        rangeIndicator.startColor = rangeIndicatorColor;
        rangeIndicator.endColor = rangeIndicatorColor;
        rangeIndicator.enabled = false;
    }

    private void Update()
    {
        if (selectedInterceptor != null)
        {
            RotateInterceptorTowardsMouse();
            UpdateRangeIndicator();
        }
    }

    void UpdateRangeIndicator()
    {
        if (rangeIndicator == null || selectedInterceptor == null) return;

        rangeIndicator.enabled = true;
        Vector3 center = selectedInterceptor.transform.position;
        float radius = selectedInterceptor.range;

        for (int i = 0; i < rangeIndicatorSegments; i++)
        {
            float angle = (float)i / rangeIndicatorSegments * 360f * Mathf.Deg2Rad;
            float x = center.x + Mathf.Cos(angle) * radius;
            float z = center.z + Mathf.Sin(angle) * radius;
            rangeIndicator.SetPosition(i, new Vector3(x, center.y + 0.1f, z));
        }
    }

    private void RotateInterceptorTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Try to hit the ground or any object to get the world position
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // Calculate direction from interceptor to mouse position
            Vector3 direction = hit.point - selectedInterceptor.transform.position;
            direction.y = 0; // Keep rotation on horizontal plane only

            if (direction != Vector3.zero)
            {
                float offset = selectedInterceptor.modelRotationOffset;
                Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, offset, 0);
                selectedInterceptor.transform.rotation = Quaternion.Slerp(
                    selectedInterceptor.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            // If no ground hit, use a plane at interceptor's height
            Plane plane = new Plane(Vector3.up, selectedInterceptor.transform.position);
            float distance;
            
            if (plane.Raycast(ray, out distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                Vector3 direction = worldPoint - selectedInterceptor.transform.position;
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    float offset = selectedInterceptor.modelRotationOffset;
                    Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, offset, 0);
                    selectedInterceptor.transform.rotation = Quaternion.Slerp(
                        selectedInterceptor.transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
        }
    }

    public void SelectInterceptor(Interceptor interceptor)
    {
        if (interceptor == null)
        {
            return;
        }

        // Hide previous range indicator if switching interceptors
        if (rangeIndicator != null && selectedInterceptor != interceptor)
        {
            rangeIndicator.enabled = false;
        }

        selectedInterceptor = interceptor;
        // Interceptor now manages its own outline color
    }

    public void FireAt(Missile missile)
    {
        if (selectedInterceptor == null)
        {
            Debug.LogWarning("Cannot fire - No interceptor selected!");
            return;
        }

        if (!selectedInterceptor.ready)
        {
            Debug.LogWarning("Cannot fire - Selected interceptor is on cooldown!");
            return;
        }

        if (InterceptorProjectilePrefab == null)
        {
            Debug.LogError("Cannot fire - InterceptorProjectilePrefab is not assigned!");
            return;
        }

        if (missile == null)
        {
            Debug.LogError("Cannot fire - Target missile is null!");
            return;
        }

        // Raise the projectile spawn position by 2 units (adjust as needed)
        Vector3 spawnPos = selectedInterceptor.transform.position + Vector3.up * 2f;
        GameObject projectile = Instantiate(
            InterceptorProjectilePrefab,
            spawnPos,
            selectedInterceptor.transform.rotation
        );

        InterceptorProjectile interceptorProjectile = projectile.GetComponent<InterceptorProjectile>();
        if (interceptorProjectile == null)
        {
            Debug.LogError("InterceptorProjectilePrefab does not have an InterceptorProjectile component!");
            Destroy(projectile);
            return;
        }

        // Set target for homing and range limit
        interceptorProjectile.SetTarget(missile);
        interceptorProjectile.SetRange(selectedInterceptor.range);
        interceptorProjectile.initialDirection = selectedInterceptor.transform.forward;

        selectedInterceptor.StartCooldown();
    }

    public bool HasSelectedInterceptor()
    {
        return selectedInterceptor != null && selectedInterceptor.ready;
    }

    public bool IsInterceptorSelected(Interceptor interceptor)
    {
        return selectedInterceptor == interceptor;
    }

    public Missile FindNearestMissile()
    {
        if (selectedInterceptor == null) return null;

        Missile[] missiles = FindObjectsByType<Missile>(FindObjectsSortMode.None);
        Missile nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Missile missile in missiles)
        {
            if (missile == null) continue;
            float distance = Vector3.Distance(selectedInterceptor.transform.position, missile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = missile;
            }
        }

        return nearest;
    }
}
