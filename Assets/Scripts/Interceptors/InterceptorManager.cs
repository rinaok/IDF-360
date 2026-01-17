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

        GameObject projectile = Instantiate(
            InterceptorProjectilePrefab,
            selectedInterceptor.transform.position,
            Quaternion.identity
        );

        projectile.GetComponent<InterceptorProjectile>().SetTarget(missile);

        selectedInterceptor.StartCooldown();
    }
}
