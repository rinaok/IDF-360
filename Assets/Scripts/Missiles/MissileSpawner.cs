using UnityEngine;
using System.Collections;

public class MissileSpawner : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform[] spawnPoints;
    public Transform[] targets;
    public float spawnRate = 2f;
    [Header("Effects")]
    public GameObject flashPrefab;

    private bool isSpawning = false;
    private Missile activeMissile;

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnLoop());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            // Only spawn if there's no active missile
            if (activeMissile == null)
            {
                SpawnMissile();
            }
            
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnMissile()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || targets == null || targets.Length == 0)
        {
            Debug.LogError("MissileSpawner not configured properly - missing spawn points or targets");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Transform target = targets[Random.Range(0, targets.Length)];

        // Calculate direction to target for initial rotation
        Vector3 direction = (target.position - spawnPoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject missileObj = Instantiate(
            missilePrefab,
            spawnPoint.position,
            rotation
        );

        Missile missile = missileObj.GetComponent<Missile>();
        missile.target = target;
        
        // Spawn flash effect at spawner location
        if (flashPrefab != null)
        {
            Instantiate(flashPrefab, spawnPoint.position, Quaternion.identity);
        }

        // Track strike
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IncrementStrikeCount();
        }
        
        // Track this as the active missile
        activeMissile = missile;
    }
}
