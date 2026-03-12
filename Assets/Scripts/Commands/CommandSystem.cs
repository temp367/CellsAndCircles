using System.Collections.Generic;
using UnityEngine;

public class CommandSystem : MonoBehaviour, IInitializable
{
    public int InitPriority => 2; // после TurnManager
    public string SystemName => "CommandSystem";
    
    private bool isInitialized = false;

    private List<Command> history;
    
    // Событие, которое вызывается после выполнения команды
    //public System.Action<Command> OnCommandExecuted;
    public System.Action<Command> OnCommandAddedToHistory;

    public bool Initialize()
    {
        try
        {
            if (!isInitialized)
            {
               history = new List<Command>();
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
        //OnCommandExecuted?.Invoke(command);

        GameLogger.LogCommand(command, "выполнена");
    }

    public void AddCommandToHistory(Command command, CircleType typeToPlace)
    {
        // Добавляем команду в историю
        history.Add(command);
        
        // Логируем в нужном формате
         string logEntry = $"Игрок:{command.OwnerPlayer}_Действие:{GetActionDescription(command)}_Тип:{typeToPlace}_координата:{GetCoordinates(command)}";
        Debug.Log(logEntry);
        
        GameLogger.Log(logEntry);
        
        OnCommandAddedToHistory?.Invoke(command);
    }
    
    private string GetActionDescription(Command command)
    {
        if (command is PlaceCircleCommand) return "Поставить";
        if (command is PushTargetCommand) return "Толкнуть";
        if (command is PlaceBarrierCommand) return "Барьер";
        if (command is ReproduceCommand) return "Размножить";
        return "Неизвестно";
    }
    
    private string GetCoordinates(Command command)
    {
        if (command is PlaceCircleCommand placeCmd)
        {
            return $"({placeCmd.X}, {placeCmd.Y})";
        }
        if (command is PushTargetCommand pushCmd)
        {
            return $"({pushCmd.Target.GridX}, {pushCmd.Target.GridY})";
        }
        if (command is PlaceBarrierCommand barrierCmd)
        {
            return $"({barrierCmd.X}, {barrierCmd.Y})";
        }
        if (command is ReproduceCommand reproduceCmd)
        {
            return $"({reproduceCmd.X}, {reproduceCmd.Y})";
        }
        return "(?, ?)";
    }
    
}