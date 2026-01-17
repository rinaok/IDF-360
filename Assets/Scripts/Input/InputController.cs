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
            Missile missile = hit.collider.GetComponent<Missile>();
            if (missile != null)
            {
                InterceptorManager.Instance.FireAt(missile);
            }
        }
    }
}
