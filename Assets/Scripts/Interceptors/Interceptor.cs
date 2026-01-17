using UnityEngine;

public class Interceptor : MonoBehaviour
{
    public bool ready = true;
    public float cooldown = 1f;

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
