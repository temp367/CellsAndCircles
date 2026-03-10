using System.Collections.Generic;
using UnityEngine;

public class CommandSystem : MonoBehaviour, IInitializable
{
    public int InitPriority => 2; // после TurnManager
    public string SystemName => "CommandSystem";
    
    private bool isInitialized = false;

    private List<Command> history;
    private int currentIndex; // для undo/redo
    
    // Событие, которое вызывается после выполнения команды
    public System.Action<Command> OnCommandExecuted;

    public bool Initialize()
    {
        try
        {
            if (!isInitialized)
            {
               history = new List<Command>();
               currentIndex = -1;
               isInitialized = true;
               Debug.Log($"{this.name}: инициализирован");

               return isInitialized; 
            }
            else
            {
                return isInitialized; 
            }
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{this.name}: ошибка инициализации - {e.Message}");
            return isInitialized;
        }
    }
    
    public void ExecuteCommand(Command command)
    {
        command.Execute();

        GameLogger.LogCommand(command, "выполнена");

        if (currentIndex < history.Count - 1)
        {
            history.RemoveRange(currentIndex + 1, history.Count - currentIndex - 1);
        }

        history.Add(command);
        currentIndex = history.Count - 1;
    }
    
    public void Undo()
    {
        if (currentIndex >= 0)
        {
            history[currentIndex].Undo();
            currentIndex--;
            Debug.Log($"CommandSystem: отмена команды. Текущий индекс: {currentIndex}");
        }
    }
    
    public void Redo()
    {
        if (currentIndex < history.Count - 1)
        {
            currentIndex++;
            history[currentIndex].Execute();
            Debug.Log($"CommandSystem: повтор команды. Текущий индекс: {currentIndex}");
        }
    }
    
    public List<string> GetHistoryDescription()
    {
        List<string> descriptions = new List<string>();
        for (int i = 0; i < history.Count; i++)
        {
            string prefix = (i == currentIndex) ? "▶ " : "  ";
            descriptions.Add($"{prefix}{i}: {history[i].GetDescription()}");
        }
        return descriptions;
    }
}