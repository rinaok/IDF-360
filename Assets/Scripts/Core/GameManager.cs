using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawner Settings")]
    public MissileSpawner missileSpawner;

    [Header("Game Phase Settings")]
    public GamePhase startingPhase = GamePhase.GazaOnly;
    
    [Header("Game Timing (Total: 60 seconds)")]
    [Tooltip("Duration for Phase 1: Gaza Only")]
    public float phase1Duration = 20f;
    [Tooltip("Duration for Phase 2: Gaza + Lebanon")]
    public float phase2Duration = 20f;
     [Tooltip("Duration for Phase 2: Gaza + Lebanon + Yemen")]
    public float phase3Duration = 20f;
    [Tooltip("Duration for Phase 3: Final Iran")]
    public float phase4Duration = 0.1f;

    [Header("Target for Missiles")]
    public Transform target;

    private GamePhase currentPhase;
    private float gameStartTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameStartTime = Time.time;
        SetPhase(startingPhase);
        StartCoroutine(GamePhaseProgression());
    }

    private IEnumerator GamePhaseProgression()
    {
        // Phase 1: Gaza Only
        SetPhase(GamePhase.GazaOnly);
        Debug.Log($"Starting Phase 1 - Gaza Only ({phase1Duration}s)");
        yield return new WaitForSeconds(phase1Duration);

        // Phase 2: Gaza + Lebanon
        SetPhase(GamePhase.GazaLebanon);
        Debug.Log($"Starting Phase 2 - Gaza + Lebanon ({phase2Duration}s)");
        yield return new WaitForSeconds(phase2Duration);

        // Phase 3: Final Iran
        SetPhase(GamePhase.FinalIran);
        Debug.Log($"Starting Phase 3 - Final Iran ({phase3Duration}s)");
        yield return new WaitForSeconds(phase3Duration);

        // Game Over
        float totalTime = Time.time - gameStartTime;
        Debug.Log($"Game Complete! Total time: {totalTime:F1} seconds");
        OnGameComplete();
    }

    public void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;

        if (missileSpawner != null)
        {
            missileSpawner.target = target;
            missileSpawner.UpdatePhase(currentPhase);
        }
    }

    private void OnGameComplete()
    {
        Debug.Log("Game Over - All phases completed!");
        // You can add end game logic here
    }

    public GamePhase GetCurrentPhase()
    {
        return currentPhase;
    }

    public float GetGameTime()
    {
        return Time.time - gameStartTime;
    }
}
