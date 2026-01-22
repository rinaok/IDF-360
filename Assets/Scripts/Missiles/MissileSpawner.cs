using UnityEngine;
using System.Collections;

public class MissileSpawner : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform[] spawnPoints;
    public Transform target;

    private float spawnRate = 2f;
    private GamePhase currentPhase;

    public void UpdatePhase(GamePhase phase)
    {
        StopAllCoroutines();
        currentPhase = phase;
                
        switch (phase)
        {
            case GamePhase.GazaOnly:
                spawnRate = 2f; // Spawn every 2 seconds (easier)
                Debug.Log("Gaza Only - Spawn rate: 2s");
                break;

            case GamePhase.GazaLebanon:
                spawnRate = 1.3f; // Spawn every 1.3 seconds (medium)
                Debug.Log("Gaza + Lebanon - Spawn rate: 1.3s");
                break;

            case GamePhase.GazaLebanonYemen:
                spawnRate = 0.8f; // Spawn every 0.8 seconds (intense)
                Debug.Log("Gaza + Lebanon + Yemen - Spawn rate: 0.8s");
                break;

            case GamePhase.FinalIran:
                spawnRate = 0.8f; // Spawn every 0.8 seconds (intense)
                Debug.Log("Final Phase + Iran - Spawn rate: 0.8s");
                break;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
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
