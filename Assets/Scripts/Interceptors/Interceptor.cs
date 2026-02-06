using UnityEngine;

public class Interceptor : MonoBehaviour
{
    public bool ready = true;
    public float cooldown = 10f;
    [Tooltip("Rotation offset if the model faces the wrong direction (0=correct, 180=backwards, 90/-90=sideways)")]
    public float modelRotationOffset = 0f;
    
    [Header("Range Settings")]
    [Tooltip("Maximum range the projectile can travel before falling")]
    public float range = 50f;

    private Outline outline;
    private bool isCoolingDown = false;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();
        outline.enabled = true;
        outline.OutlineWidth = 5f;
    }

    void Update()
    {
        UpdateOutlineColor();
    }

    private void UpdateOutlineColor()
    {
        if (outline == null) return;

        if (isCoolingDown)
        {
            outline.OutlineColor = Color.red;
        }
        else if (IsSelected())
        {
            outline.OutlineColor = Color.green;
        }
        else
        {
            // no outlinecolor
            outline.OutlineColor = Color.clear;
        }
    }

    private bool IsSelected()
    {
        return InterceptorManager.Instance != null && InterceptorManager.Instance.IsInterceptorSelected(this);
    }

    private void OnMouseDown()
    {
        InterceptorManager.Instance.SelectInterceptor(this);
    }

    public void StartCooldown()
    {
        ready = false;
        isCoolingDown = true;
        StartCoroutine(CooldownRoutine());
    }

    // Outline color logic handled in UpdateOutlineColor()

    private System.Collections.IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldown);
        ready = true;
        isCoolingDown = false;
    }
}
