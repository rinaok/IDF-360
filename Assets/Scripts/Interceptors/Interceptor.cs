using UnityEngine;

public class Interceptor : MonoBehaviour
{
    public bool ready = true;
    public float cooldown = 1f;
    [Tooltip("Rotation offset if the model faces the wrong direction (0=correct, 180=backwards, 90/-90=sideways)")]
    public float modelRotationOffset = 0f;
    
    [Header("Range Settings")]
    [Tooltip("Maximum range the projectile can travel before falling")]
    public float range = 50f;

    private void OnMouseDown()
    {
        if (!ready) return;

        InterceptorManager.Instance.SelectInterceptor(this);
    }

    public void StartCooldown()
    {
        ready = false;
        StartCoroutine(CooldownRoutine());
    }

    private System.Collections.IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}
