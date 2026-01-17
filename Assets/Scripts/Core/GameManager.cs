using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawner Settings")]
    public MissileSpawner missileSpawner;

    [Header("Game Phase Settings")]
    public GamePhase startingPhase = GamePhase.GazaOnly;

    [Header("Target for Missiles")]
    public Transform target;

    private GamePhase currentPhase;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetPhase(startingPhase);
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
}
