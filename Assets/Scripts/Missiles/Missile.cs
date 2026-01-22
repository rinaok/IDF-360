using System;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target; 

    public float flightDuration = 4f;
    public float arcHeight = 5f;

    private Vector3 startPos;
    private float timer;

    void Start()
    {
        startPos = transform.position;
    }

   void Update()
    {
        if (target == null) return;

        timer += Time.deltaTime;
        float t = timer / flightDuration;
        t = Mathf.Clamp01(t);

        Vector3 horizontalPos = Vector3.Lerp(startPos, target.position, t);
        float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
        transform.position = horizontalPos + Vector3.up * height;

        if (t >= 1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        Debug.LogError("Missile hit target!");
        Destroy(gameObject);
    }

    public void Intercept()
    {
        Debug.LogAssertion("Missile intercepted");
        Destroy(gameObject);
    }
}
