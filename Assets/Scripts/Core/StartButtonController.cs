using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartButtonController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject panel;
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Awake()
    {
        if (startButton == null)
            startButton = GetComponent<Button>();
        
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        // Setup audio source for button click
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Make panel non-clickable if it exists
        if (panel != null)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = panel.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnStartClicked()
    {
        // Play button click sound
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        // Hide phase notification if visible
        PhaseNotificationUI phaseNotificationUI = FindFirstObjectByType<PhaseNotificationUI>();
        if (phaseNotificationUI != null)
        {
            phaseNotificationUI.Hide();
        }

        // Start the game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        
        // Hide the button
        if (startButton != null)
            startButton.gameObject.SetActive(false);

        // Hide the panel
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowAsPlayAgain()
    {
        if (startButton != null)
        {
            // Change button text to "Play Again"
            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Play Again";
            }
            
            // Show the button
            startButton.gameObject.SetActive(true);
        }

        // Show the panel
        if (panel != null)
            panel.SetActive(true);
    }
}
