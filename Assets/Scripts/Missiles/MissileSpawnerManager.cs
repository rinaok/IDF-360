using UnityEngine;

[System.Serializable]
public class PhaseSpawnerGroup
{
    public GamePhase phase;
    public MissileSpawner[] spawners;
}

public class MissileSpawnerManager : MonoBehaviour
{
    [Header("Phase Spawner Configuration")]
    [Tooltip("Configure which spawners are active for each game phase")]
    public PhaseSpawnerGroup[] phaseSpawnerGroups;

    [Header("Target")]
    public Transform target;

    private GamePhase currentPhase;
    private MissileSpawner[] activeSpawners;

    public void UpdatePhase(GamePhase newPhase)
    {
        // Stop all currently active spawners
        StopCurrentSpawners();

        currentPhase = newPhase;

        // Find and activate spawners for the new phase
        activeSpawners = GetSpawnersForPhase(newPhase);

        if (activeSpawners == null || activeSpawners.Length == 0)
        {
            Debug.LogWarning($"No spawners configured for phase: {newPhase}");
            return;
        }

        if (target == null)
        {
            Debug.LogError("Target is not set in MissileSpawnerManager!");
            return;
        }

        // Set target and start spawning for all spawners in this phase
        foreach (MissileSpawner spawner in activeSpawners)
        {
            if (spawner != null)
            {
                spawner.target = target;
                spawner.StartSpawning();
                Debug.Log($"Started spawner: {spawner.name} with {spawner.spawnPoints?.Length ?? 0} spawn points");
            }
        }

        Debug.Log($"Phase {newPhase}: Activated {activeSpawners.Length} spawners");
    }

    private MissileSpawner[] GetSpawnersForPhase(GamePhase phase)
    {
        foreach (PhaseSpawnerGroup group in phaseSpawnerGroups)
        {
            if (group.phase == phase)
            {
                return group.spawners;
            }
        }
        return null;
    }

    private void StopCurrentSpawners()
    {
        if (activeSpawners != null)
        {
            foreach (MissileSpawner spawner in activeSpawners)
            {
                if (spawner != null)
                {
                    spawner.StopSpawning();
                }
            }
        }
    }

    private void OnDestroy()
    {
        StopCurrentSpawners();
    }
}
