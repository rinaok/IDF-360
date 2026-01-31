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

        // Application.logMessageReceived += HandleLog; // Disable log capture
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        // Application.logMessageReceived -= HandleLog;
    }

    // Remove or comment out HandleLog and UpdateLogsDisplay methods if you want to fully disable log UI

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
            // instance.UpdateLogsDisplay();
        }
    }
}
