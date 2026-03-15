using System;
using System.IO;
using UnityEngine;

public static class GameLog
{
    private static string logFilePath;
    private static int actionIndex = 0;

    public static void Initialize()
    {
        string folder = Application.persistentDataPath + "/Logs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        logFilePath = folder + "/game_log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

        Write("=== GAME LOG START ===");
    }

    private static void Write(string message)
    {
        actionIndex++;

        string line = $"[{actionIndex:0000}] {DateTime.Now:HH:mm:ss} {message}";

        Debug.Log(line);

        try
        {
            File.AppendAllText(logFilePath, line + "\n");
        }
        catch (Exception e)
        {
            Debug.LogError("Log file write error: " + e.Message);
        }
    }

    public static void Action(string message)
    {
        Write("[ACTION] " + message);
    }

    public static void Command(string message)
    {
        Write("[COMMAND] " + message);
    }

    public static void Ether(string message)
    {
        Write("[ETHER] " + message);
    }

    public static void Trigger(string message)
    {
        Write("[TRIGGER] " + message);
    }

    public static void Event(string message)
    {
        Write("[EVENT] " + message);
    }

    public static void Error(string message)
    {
        Write("[ERROR] " + message);
    }
}