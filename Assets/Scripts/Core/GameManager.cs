using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawner Settings")]
    public MissileSpawnerManager missileSpawnerManager;

    [Header("Game Phase Settings")]
    public GamePhase startingPhase = GamePhase.GazaOnly;
    
    [Header("Game Timing")]
    [Tooltip("Duration for Phase 1: Gaza Only (0-1 minute)")]
    public float phase1Duration = 60f;
    [Tooltip("Duration for Phase 2: Gaza + Lebanon (1-2 minutes)")]
    public float phase2Duration = 60f;
     [Tooltip("Duration for Phase 3: Gaza + Lebanon + Yemen (2-4 minutes)")]
    public float phase3Duration = 120f;
    [Tooltip("Duration for Phase 4: Final Iran (4+ minutes)")]
    public float phase4Duration = 60f;

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

        // Phase 3: Gaza + Lebanon + Yemen
        SetPhase(GamePhase.GazaLebanonYemen);
        Debug.Log($"Starting Phase 3 - Gaza + Lebanon + Yemen ({phase3Duration}s)");
        yield return new WaitForSeconds(phase3Duration);

        // Phase 4: Final Iran
        SetPhase(GamePhase.FinalIran);
        Debug.Log($"Starting Phase 4 - Final Iran ({phase4Duration}s)");
        yield return new WaitForSeconds(phase4Duration);

        // Game Over
        float totalTime = Time.time - gameStartTime;
        Debug.Log($"Game Complete! Total time: {totalTime:F1} seconds");
        OnGameComplete();
    }

    public void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;

        if (missileSpawnerManager != null)
        {
            missileSpawnerManager.UpdatePhase(currentPhase);
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
