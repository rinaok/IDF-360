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

    [Header("Targets")]
    public Transform[] targets;

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
            return;
        }

        if (targets == null || targets.Length == 0)
        {
            Debug.LogError("No targets set in MissileSpawnerManager!");
            return;
        }

        // Set targets and start spawning for all spawners in this phase
        foreach (MissileSpawner spawner in activeSpawners)
        {
            if (spawner != null)
            {
                spawner.targets = targets;
                spawner.StartSpawning();
            }
        }
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

    public void StopAllSpawners()
    {
        StopCurrentSpawners();
    }

    private void OnDestroy()
    {
        StopCurrentSpawners();
    }
}
