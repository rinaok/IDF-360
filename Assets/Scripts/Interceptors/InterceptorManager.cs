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
            return;
        }

        selectedInterceptor = interceptor;
    }

    public void FireAt(Missile missile)
    {
        if (selectedInterceptor == null)
        {
            return;
        }

        if (!selectedInterceptor.ready)
        {
            return;
        }

        if (InterceptorProjectilePrefab == null)
        {
            return;
        }

        if (missile == null)
        {
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
            Destroy(projectile);
            return;
        }

        interceptorProjectile.SetTarget(missile);

        selectedInterceptor.StartCooldown();
    }
}
