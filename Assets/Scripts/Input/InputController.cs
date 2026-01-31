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
            Debug.Log($"Clicked on: {hit.collider.gameObject.name}");
            
            // Only select interceptor on click
            Interceptor interceptor = hit.collider.GetComponent<Interceptor>();
            if (interceptor != null)
            {
                if (InterceptorManager.Instance != null)
                {
                    InterceptorManager.Instance.SelectInterceptor(interceptor);
                }
                else
                {
                    Debug.LogError("InterceptorManager.Instance is null!");
                }
                return;
            }
        }

        // If an interceptor is selected, fire forward on any click (not on interceptor)
        if (InterceptorManager.Instance != null && InterceptorManager.Instance.HasSelectedInterceptor())
        {
            InterceptorManager.Instance.FireForward();
        }
    }
}
