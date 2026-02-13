using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawner Settings")]
    public MissileSpawnerManager missileSpawnerManager;

    [Header("Game Phase Settings")]
    public GamePhase startingPhase = GamePhase.Gaza;
    
    [Header("Game Timing")]
    public float phase1Duration = 15f;
    public float phase2Duration = 15f;
    public float phase3Duration = 15f;
    public float phase4Duration = 15f;

    [Header("Audio")]
    public AudioClip backgroundMusic;
    public AudioClip sirenSound;
    private AudioSource audioSource;
    
    [Header("UI")]
    public PhaseNotificationUI phaseNotificationUI;

    private GamePhase currentPhase;
    private float gameStartTime;
    private bool gameStarted = false;
    private int totalStrikes = 0;
    private int activeMissileCount = 0;
    private bool lastPhaseStarted = false;
    private int hitCount = 0;
    private int missCount = 0;

    private void Awake()
    {
        Instance = this;
        
        // Setup audio source
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.3f; // Set background music to be quieter
    }

    public void IncrementStrikeCount()
    {
        totalStrikes++;
        activeMissileCount++;
    }

    public void OnMissileHit()
    {
        hitCount++;
        OnMissileDestroyed();
    }

    public void OnMissileMiss()
    {
        missCount++;
        OnMissileDestroyed();
    }

    public void OnMissileDestroyed()
    {
        activeMissileCount--;
        
        // Check if game should end (last phase and no missiles left)
        if (lastPhaseStarted && (activeMissileCount < 0))
        {
            OnGameComplete();
        }
    }

    private void Start()
    {
        // Start background music immediately
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    public void StartGame()
    {
        if (gameStarted) return;
        
        gameStarted = true;
        gameStartTime = Time.time;
        
        SetPhase(startingPhase);
        StartCoroutine(GamePhaseProgression());
    }

    private IEnumerator GamePhaseProgression()
    {
        SetPhase(GamePhase.Gaza);
        yield return new WaitForSeconds(phase1Duration);

        SetPhase(GamePhase.Lebanon);
        yield return new WaitForSeconds(phase2Duration);

        SetPhase(GamePhase.Yemen);
        yield return new WaitForSeconds(phase3Duration);

        SetPhase(GamePhase.Iran);
        lastPhaseStarted = true;
        // Don't wait for time - game will end when last missile hits/misses
    }

    public void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;

        // Play siren sound if not gaza phase
        if (sirenSound != null && newPhase != GamePhase.Gaza)
        {
            AudioSource.PlayClipAtPoint(sirenSound, Camera.main.transform.position, 1.5f);
        }
        
        // Show phase notification
        if (phaseNotificationUI != null)
        {
            phaseNotificationUI.ShowPhaseNotification(newPhase);
        }

        if (missileSpawnerManager != null)
        {
            missileSpawnerManager.UpdatePhase(currentPhase);
        }
    }

    private void OnGameComplete()
    {
        // Stop all spawners
        if (missileSpawnerManager != null)
        {
            missileSpawnerManager.StopAllSpawners();
        }

        // Show final score
        if (phaseNotificationUI != null)
        {
            phaseNotificationUI.ShowFinalScore(hitCount, missCount);
        }

        // Reset game state
        gameStarted = false;
        lastPhaseStarted = false;
        activeMissileCount = 0;
        hitCount = 0;
        missCount = 0;

        // Show start button with "Play Again" text
        StartButtonController startButtonController = FindFirstObjectByType<StartButtonController>();
        if (startButtonController != null)
        {
            startButtonController.ShowAsPlayAgain();
        }
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
