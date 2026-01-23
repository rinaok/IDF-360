using UnityEngine;

public class InterceptorManager : MonoBehaviour
{
    public static InterceptorManager Instance;

    [Header("Interceptor Projectile Prefab")]
    public GameObject InterceptorProjectilePrefab;

    private Interceptor selectedInterceptor;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectInterceptor(Interceptor interceptor)
    {
        if (interceptor == null || !interceptor.ready)
        {
            Debug.Log("Interceptor not ready or null");
            return;
        }

        selectedInterceptor = interceptor;
        Debug.Log("Selected interceptor: " + interceptor.name);
    }

    public void FireAt(Missile missile)
    {
        if (selectedInterceptor == null)
        {
            Debug.Log("No interceptor selected!");
            return;
        }

        if (!selectedInterceptor.ready)
        {
            Debug.Log("Selected interceptor is on cooldown!");
            return;
        }

        if (InterceptorProjectilePrefab == null)
        {
            Debug.LogError("InterceptorProjectilePrefab is not assigned in InterceptorManager!");
            return;
        }

        if (missile == null)
        {
            Debug.LogError("Target missile is null!");
            return;
        }

        GameObject projectile = Instantiate(
            InterceptorProjectilePrefab,
            selectedInterceptor.transform.position,
            Quaternion.identity
        );

        InterceptorProjectile interceptorProjectile = projectile.GetComponent<InterceptorProjectile>();
        if (interceptorProjectile == null)
        {
            Debug.LogError("InterceptorProjectilePrefab does not have an InterceptorProjectile component!");
            Destroy(projectile);
            return;
        }

        interceptorProjectile.SetTarget(missile);

        selectedInterceptor.StartCooldown();
        Debug.Log($"Fired interceptor at missile: {missile.name}");
    }
}
