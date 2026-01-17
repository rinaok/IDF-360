using UnityEngine;

public class InterceptorProjectile : MonoBehaviour
{
    private Missile target;
    public float speed = 15f;

    public void SetTarget(Missile missile)
    {
        target = missile;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            target.Intercept();
            Destroy(gameObject);
        }
    }
}
