using System;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target; 

    public float speed = 10f;
    public float arcHeight = 5f;
    [Tooltip("Rotation offset if the model faces the wrong direction (0=correct, 180=backwards, 90/-90=sideways)")]
    public float modelRotationOffset = 0f;
    
    [Header("Effects")]
    public GameObject explosionPrefab;

    [Header("Trajectory Line")]
    public Color trajectoryColor = new Color(1f, 0f, 0f, 0.5f);
    public int trajectorySegments = 30;
    private LineRenderer trajectoryLine;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip missSound;

    private Vector3 startPos;
    private float timer;
    private float flightDuration;
    private bool isResolved = false;

    void Start()
    {
        startPos = transform.position;
        
        // Calculate flight duration from speed and distance
        if (target != null)
        {
            float distance = Vector3.Distance(startPos, target.position);
            flightDuration = distance / speed;
        }
        
        CreateTrajectoryLine();
    }

    void CreateTrajectoryLine()
    {
        if (target == null) return;
        
        GameObject lineObj = new GameObject("TrajectoryLine");
        lineObj.transform.SetParent(transform);
        trajectoryLine = lineObj.AddComponent<LineRenderer>();
        trajectoryLine.positionCount = trajectorySegments;
        trajectoryLine.startWidth = 0.15f;
        trajectoryLine.endWidth = 0.15f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = trajectoryColor;
        trajectoryLine.endColor = trajectoryColor;
        trajectoryLine.useWorldSpace = true;
        
        UpdateTrajectoryLine();
    }

    void UpdateTrajectoryLine()
    {
        if (trajectoryLine == null || target == null) return;
        
        for (int i = 0; i < trajectorySegments; i++)
        {
            float t = (float)i / (trajectorySegments - 1);
            Vector3 horizontalPos = Vector3.Lerp(startPos, target.position, t);
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            trajectoryLine.SetPosition(i, horizontalPos + Vector3.up * height);
        }
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
        if (isResolved) return;
        isResolved = true;

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
        
        // Notify GameManager that missile missed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMissileMiss();
        }
        
        Destroy(gameObject);
    }

    public void Intercept()
    {
        if (isResolved) return;
        isResolved = true;

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
        
        // Notify GameManager that missile was hit
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMissileHit();
        }
        
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Safety net: if missile is destroyed without being resolved, decrement count
        if (!isResolved && GameManager.Instance != null)
        {
            isResolved = true;
            GameManager.Instance.OnMissileDestroyed();
        }
    }
}
