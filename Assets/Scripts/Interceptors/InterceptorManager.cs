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

    [Header("Fire Settings")]
    public float fireAngle = 45f; // Angle in degrees above the forward direction

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

        Debug.Log($"Selected interceptor: {interceptor.name}");
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

        GameObject projectile = Instantiate(
            InterceptorProjectilePrefab,
            selectedInterceptor.transform.position,
            selectedInterceptor.transform.rotation
        );

        InterceptorProjectile interceptorProjectile = projectile.GetComponent<InterceptorProjectile>();
        if (interceptorProjectile == null)
        {
            Debug.LogError("InterceptorProjectilePrefab does not have an InterceptorProjectile component!");
            Destroy(projectile);
            return;
        }

        // Set initial direction for projectile
        Vector3 forward = selectedInterceptor.transform.forward;
        Vector3 angledDirection = Quaternion.AngleAxis(fireAngle, selectedInterceptor.transform.right) * forward;
        interceptorProjectile.initialDirection = angledDirection;

        selectedInterceptor.StartCooldown();
        Debug.Log($"Fired interceptor at missile: {missile.name}");
    }

    public bool HasSelectedInterceptor()
    {
        return selectedInterceptor != null && selectedInterceptor.ready;
    }

    public void FireForward()
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

        GameObject projectile = Instantiate(
            InterceptorProjectilePrefab,
            selectedInterceptor.transform.position,
            selectedInterceptor.transform.rotation
        );

        InterceptorProjectile interceptorProjectile = projectile.GetComponent<InterceptorProjectile>();
        if (interceptorProjectile == null)
        {
            Debug.LogError("InterceptorProjectilePrefab does not have an InterceptorProjectile component!");
            Destroy(projectile);
            return;
        }

        // Set initial direction for projectile
        Vector3 forward = selectedInterceptor.transform.forward;
        Vector3 angledDirection = Quaternion.AngleAxis(fireAngle, selectedInterceptor.transform.right) * forward;
        interceptorProjectile.initialDirection = angledDirection;

        selectedInterceptor.StartCooldown();
        Debug.Log($"Fired interceptor FORWARD from: {selectedInterceptor.name}");
    }
}
