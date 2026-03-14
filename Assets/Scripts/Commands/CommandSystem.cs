using System.Collections.Generic;
using UnityEngine;

public class CommandSystem : MonoBehaviour, IInitializable
{
     public InitStage InitStage => InitStage.Command;

    private List<Command> history;

    public void Initialize()
    {
        history = new List<Command>();

        GameServices.Register(this);

        Debug.Log("CommandSystem initialized");
    }

    public void AddCommandToHistory(Command command, CircleType typeToPlace, bool execute, bool isEther)
    {
        // Добавляем команду в историю
        history.Add(command);
        
        // Логируем в нужном формате
        string logEntry = $"Игрок:{command.OwnerPlayer}_Действие:{GetActionDescription(command)}_Тип:{typeToPlace}_координата:{GetCoordinates(command)}";
        
        GameLogger.Log(logEntry);
        
        //OnCommandAddedToHistory?.Invoke(command);
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
            return $"({pushCmd.NewX}, {pushCmd.NewY})";
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