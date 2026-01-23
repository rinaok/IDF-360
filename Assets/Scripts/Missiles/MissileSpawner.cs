using UnityEngine;
using System.Collections;

public class MissileSpawner : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform[] spawnPoints;
    public Transform target;
    public float spawnRate = 2f;

    private bool isSpawning = false;

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
            SpawnMissile();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnMissile()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || target == null)
        {
            Debug.LogError("MissileSpawner not configured properly");
            return;
        }

        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject missileObj = Instantiate(
            missilePrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Missile missile = missileObj.GetComponent<Missile>();
        missile.target = target;
    }
}
