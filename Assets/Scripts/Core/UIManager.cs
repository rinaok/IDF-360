using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Log Display")]
    [SerializeField] private TextMeshProUGUI logsText;
    [SerializeField] private int maxLogLines = 50;
    [SerializeField] private bool showTimestamp = true;
    [SerializeField] private bool showLogType = true;
    [SerializeField] private float logDisplayDuration = 5f;

    private class LogEntry
    {
        public string message;
        public float expirationTime;
    }

    private List<LogEntry> logEntries = new List<LogEntry>();
    private static UIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Subscribe to Unity's log message received callback
        Application.logMessageReceived += HandleLog;

        // Start the coroutine to clean up expired logs
        StartCoroutine(CleanupExpiredLogs());
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string prefix = "";
        string colorCode = "";

        // Add timestamp if enabled
        if (showTimestamp)
        {
            prefix = $"[{System.DateTime.Now:HH:mm:ss}] ";
        }

        // Add log type and color if enabled
        if (showLogType)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    colorCode = "<color=red>";
                    break;
                case LogType.Warning:
                    colorCode = "<color=yellow>";
                    break;
                case LogType.Log:
                    colorCode = "<color=white>";
                    break;
                case LogType.Assert:
                    colorCode = "<color=green>";
                    break;
            }
        }

        string formattedLog = $"{colorCode}{prefix}{logString}</color>";

        // Add to list with expiration time
        LogEntry newEntry = new LogEntry
        {
            message = formattedLog,
            expirationTime = Time.time + logDisplayDuration
        };
        logEntries.Add(newEntry);

        // Remove oldest log if we exceed max lines
        if (logEntries.Count > maxLogLines)
        {
            logEntries.RemoveAt(0);
        }

        // Update the UI text
        UpdateLogsDisplay();
    }

    private IEnumerator CleanupExpiredLogs()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // Check twice per second

            bool needsUpdate = false;
            float currentTime = Time.time;

            // Remove expired logs
            logEntries.RemoveAll(entry =>
            {
                if (entry.expirationTime <= currentTime)
                {
                    needsUpdate = true;
                    return true;
                }
                return false;
            });

            if (needsUpdate)
            {
                UpdateLogsDisplay();
            }
        }
    }

    private void UpdateLogsDisplay()
    {
        if (logsText != null)
        {
            List<string> messages = new List<string>();
            foreach (var entry in logEntries)
            {
                messages.Add(entry.message);
            }
            logsText.text = string.Join("\n", messages);
        }
    }

    // Public method to manually add a log
    public static void AddLog(string message, LogType type = LogType.Log)
    {
        Debug.Log(message); // This will be caught by HandleLog
    }

    // Public method to clear logs
    public static void ClearLogs()
    {
        if (instance != null)
        {
            instance.logEntries.Clear();
            instance.UpdateLogsDisplay();
        }
    }
}
