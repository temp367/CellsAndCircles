using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class GameLogger
{
    private static string logFilePath;
    private static bool initialized = false;
    private static StringBuilder currentSessionLog = new StringBuilder();
    
    // Инициализация логгера (создаёт файл с датой в имени)
    public static void Initialize()
    {
        if (initialized) return;
        
        // Создаём папку Logs, если её нет
        string logsDirectory = Path.Combine(Application.dataPath, "../Logs");
        if (!Directory.Exists(logsDirectory))
        {
            Directory.CreateDirectory(logsDirectory);
        }
        
        // Имя файла: GameLog_2025-03-07_14-30-45.txt
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"GameLog_{timestamp}.txt";
        logFilePath = Path.Combine(logsDirectory, fileName);
        
        // Записываем заголовок
        currentSessionLog.AppendLine($"=== Игровая сессия начата {DateTime.Now} ===");
        currentSessionLog.AppendLine();
        
        initialized = true;
        
        Debug.Log($"GameLogger: лог будет сохранён в {logFilePath}");
    }
    
    // Запись события в лог
    public static void Log(string message)
    {
        if (!initialized) Initialize();
        
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        string logEntry = $"[{timestamp}] {message}";
        
        // Добавляем в текущую сессию
        currentSessionLog.AppendLine(logEntry);
        
        // Сразу записываем в файл (можно накапливать и сбрасывать раз в несколько секунд,
        // но для простоты будем писать сразу)
        try
        {
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogError($"GameLogger: ошибка записи в файл: {e.Message}");
        }
    }
    
    // Запись действия игрока (команды)
    public static void LogCommand(Command command, string result = "успешно")
    {
        string message = $"Команда: {command.GetDescription()} | Игрок: {command.OwnerPlayer} | {result}";
        Log(message);
    }
    
    // Запись смены состояния
    public static void LogStateChange(string fromState, string toState)
    {
        Log($"Смена состояния: {fromState} -> {toState}");
    }
    
    // Запись клика
    public static void LogClick(int x, int y, string element = "клетка")
    {
        Log($"Клик: {element} ({x}, {y})");
    }
    
    // Завершение сессии и сохранение
    public static void Shutdown()
    {
        if (!initialized) return;
        
        currentSessionLog.AppendLine();
        currentSessionLog.AppendLine($"=== Игровая сессия завершена {DateTime.Now} ===");
        
        try
        {
            File.AppendAllText(logFilePath, currentSessionLog.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError($"GameLogger: ошибка сохранения при завершении: {e.Message}");
        }
        
        Debug.Log($"GameLogger: лог сохранён в {logFilePath}");
        initialized = false;
    }
}