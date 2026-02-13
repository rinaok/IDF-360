using UnityEngine;
using TMPro;
using System.Collections;

public class PhaseNotificationUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI phaseNotificationText;
    
    [Header("Display Settings")]
    public float displayDuration = 3f;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Start visible so initial text shows before game starts
        canvasGroup.alpha = 1;
    }
    
    public void ClearInitialText()
    {
        if (phaseNotificationText != null)
        {
            phaseNotificationText.text = string.Empty;
        }
        // Hide after clearing
        canvasGroup.alpha = 0;
    }

    public void ShowPhaseNotification(GamePhase phase)
    {
        string phaseName = GetPhaseName(phase);
        phaseNotificationText.text = $"Missiles incoming from: {phaseName}";
        StartCoroutine(ShowNotificationCoroutine());
    }

    public void ShowFinalScore(int hits, int misses)
    {
        int totalMissiles = hits + misses;
        string victoryMessage = hits > misses ? "\n\nGood Job!" : "\n\nYou failed to save Israel :(";
        phaseNotificationText.text = $"Final Score:\nHits: {hits}\nMisses: {misses}{victoryMessage}";
        StartCoroutine(ShowFinalScoreCoroutine());
    }

    private IEnumerator ShowFinalScoreCoroutine()
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
        
        // Keep visible until manually hidden
    }

    private IEnumerator ShowNotificationCoroutine()
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Display
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }

    private string GetPhaseName(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Gaza:
                return "Gaza";
            case GamePhase.Lebanon:
                return "Lebanon";
            case GamePhase.Yemen:
                return "Yemen";
            case GamePhase.Iran:
                return "Iran";
            default:
                return phase.ToString();
        }
    }
}
