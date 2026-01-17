using UnityEngine;
using System.Collections;

public class MissileSpawner : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform[] spawnPoints;
    public Transform target;

    private float spawnRate = 2f;

    public void UpdatePhase(GamePhase phase)
    {
        StopAllCoroutines();
        Debug.Log("MissileSpawner: Updating phase to " + phase);
        switch (phase)
        {
            case GamePhase.GazaOnly:
                spawnRate = 2f;
                break;

            case GamePhase.GazaLebanon:
                spawnRate = 1.3f;
                break;

            case GamePhase.FinalIran:
                spawnRate = 0.8f;
                break;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        Debug.Log("MissileSpawner: Starting spawn loop with rate " + spawnRate);
        while (true)
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
