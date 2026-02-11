using System;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target; 

    public float flightDuration = 4f;
    public float arcHeight = 5f;
    [Tooltip("Rotation offset if the model faces the wrong direction (0=correct, 180=backwards, 90/-90=sideways)")]
    public float modelRotationOffset = 0f;
    
    [Header("Effects")]
    public GameObject explosionPrefab;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip missSound;

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

        Vector3 previousPos = transform.position;
        Vector3 horizontalPos = Vector3.Lerp(startPos, target.position, t);
        float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
        transform.position = horizontalPos + Vector3.up * height;

        // Rotate missile to face direction of travel
        Vector3 direction = (transform.position - previousPos).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, modelRotationOffset, 0);
        }

        if (t >= 1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        // Spawn explosion at ground hit position
        if (explosionPrefab != null && target != null)
        {
            Instantiate(explosionPrefab, target.position, Quaternion.identity);
        }

        // Play miss sound (missile reached target, wasn't intercepted)
        if (missSound != null)
        {
            AudioSource.PlayClipAtPoint(missSound, transform.position, 4.0f);
        }
        
        // Notify GameManager that missile is destroyed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMissileDestroyed();
        }
        
        Destroy(gameObject);
    }

    public void Intercept()
    {
        // Spawn explosion at intercept position
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Play hit sound (missile was intercepted)
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 4.0f);
        }
        
        // Notify GameManager that missile is destroyed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMissileDestroyed();
        }
        
        Destroy(gameObject);
    }
}
