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

    private Interceptor selectedInterceptor;
    private Outline currentOutline;
    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (selectedInterceptor != null)
        {
            RotateInterceptorTowardsMouse();
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
        if (interceptor == null || !interceptor.ready)
        {
            return;
        }

        // Remove outline from previously selected interceptor
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
        }

        selectedInterceptor = interceptor;

        // Add or enable outline on newly selected interceptor
        currentOutline = interceptor.GetComponent<Outline>();
        if (currentOutline == null)
        {
            try
            {
                currentOutline = interceptor.gameObject.AddComponent<Outline>();
                currentOutline.OutlineColor = outlineColor;
                currentOutline.OutlineWidth = outlineWidth;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not add Outline to {interceptor.name}. " +
                    "Enable Read/Write in mesh import settings: " +
                    "Select the model in Project window > Inspector > Read/Write Enabled checkbox.\n" +
                    $"Error: {e.Message}");
            }
        }
        else
        {
            currentOutline.OutlineColor = outlineColor;
            currentOutline.OutlineWidth = outlineWidth;
            currentOutline.enabled = true;
        }
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

        // Set target for homing
        interceptorProjectile.SetTarget(missile);
        interceptorProjectile.initialDirection = selectedInterceptor.transform.forward;

        selectedInterceptor.StartCooldown();
    }

    public bool HasSelectedInterceptor()
    {
        return selectedInterceptor != null && selectedInterceptor.ready;
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
