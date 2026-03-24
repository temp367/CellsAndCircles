using System;
using System.Collections.Generic;
using UnityEngine;

public class EtherSystem : MonoBehaviour, IInitializable
{
    public InitStage InitStage => InitStage.Ethir;

    private List<Trigger> triggers = new List<Trigger>();   // активные триггеры
    
    // Связь триггеров и команд
    private Dictionary<Trigger, Command> pendingCommands = new Dictionary<Trigger, Command>();

    public List<Trigger> GetTriggers() => triggers ?? new List<Trigger>();

    public Dictionary<Trigger, Command> GetPendingCommands() => pendingCommands ?? new Dictionary<Trigger, Command>();
    
    public void Initialize()
    {
        GameServices.Register(this);

        GameServices.Ability.OnGameCommandExecuted += HandleGameCommandExecuted;

        Debug.Log("EtherSystem initialized");
    }


    public void AddEnemyPlaceTrigger(Command command, CircleType? type, Vector2Int? cell, int player)
    {
        EnemyPlaceCircleTrigger trigger = new EnemyPlaceCircleTrigger(player, type, cell);
    
        trigger.OnTriggered += HandleEnemyTriggerActivated;
    
        GameLog.Trigger($"Trigger created Enemy_Place_Circle | Player:{player} | Type:{type} | Cell:{cell}");

        triggers.Add(trigger);
        pendingCommands.Add(trigger, command);
    }

    public void AddSelfPlaceTrigger(Command command, CircleType? type, Vector2Int? cell, int player)
    {
        SelfPlaceCircleTrigger trigger = new SelfPlaceCircleTrigger(player, type, cell);
    
        trigger.OnTriggered += HandleEnemyTriggerActivated;
    
        GameLog.Trigger($"Trigger created Self_Place_Circle | Player:{player} | Type:{type} | Cell:{cell}");
        
        triggers.Add(trigger);
        pendingCommands.Add(trigger, command);
    }

    public void AddEnemyActivateTrigger(Command command, CircleType? type, Vector2Int? cell, int player)
    {
        EnemyActivateTrigger trigger = new EnemyActivateTrigger(player, type, cell);

        trigger.OnTriggered += HandleEnemyTriggerActivated;

        triggers.Add(trigger);

        pendingCommands.Add(trigger, command);

        GameLog.Trigger($"Trigger created Enemy_Activate_ | Player:{player} | Type:{type} | Cell:{cell}");
        
    }

    // вызывается когда выполнена обычная команда
    private void HandleGameCommandExecuted(Command command)
    {
        foreach (var trigger in triggers.ToArray())
        {
            trigger.TryCheck(command);

            if (!trigger.IsActive)
            {
                triggers.Remove(trigger);
            }
        }
    }

    private void HandleEnemyTriggerActivated(Trigger trigger)
    {
        if (pendingCommands.ContainsKey(trigger))
        {
            Command command = pendingCommands[trigger];

            GameLog.Trigger($"Checking trigger {GetType().Name} against {command.GetType().Name}");

            command.Execute();
            
            GameLog.Ether($"Executing pending ether command {command.GetType().Name}");

            pendingCommands.Remove(trigger);
        }
    }


    public void ClearAllCommands()
    {
        pendingCommands.Clear();
    }
}