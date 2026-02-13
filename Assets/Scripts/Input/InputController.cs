using UnityEngine;

public class InputController : MonoBehaviour
{
    private Camera cam;
    private InterceptorManager interceptorManager;

    void Start()
    {
        cam = Camera.main;
        interceptorManager = FindFirstObjectByType<InterceptorManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    void HandleClick()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Only select interceptor on click
            Interceptor interceptor = hit.collider.GetComponent<Interceptor>();
            if (interceptor != null)
            {
                if (InterceptorManager.Instance != null)
                {
                    InterceptorManager.Instance.SelectInterceptor(interceptor);
                }
                return;
            }
        }

        // If an interceptor is selected, fire at the missile closest to click direction
        if (InterceptorManager.Instance != null && InterceptorManager.Instance.HasSelectedInterceptor())
        {
            Missile targetMissile = InterceptorManager.Instance.FindMissileClosestToAim(ray);
            if (targetMissile != null)
            {
                InterceptorManager.Instance.FireAt(targetMissile);
            }
        }
    }
}
