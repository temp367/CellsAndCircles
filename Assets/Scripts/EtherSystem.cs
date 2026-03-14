using System;
using System.Collections.Generic;
using UnityEngine;

public class EtherSystem : MonoBehaviour, IInitializable
{
     public InitStage InitStage => InitStage.Ethir;
    
    // Словарь для хранения связи триггеров и команд
    private Dictionary<TurnTrigger, Command> pendingCommands = new Dictionary<TurnTrigger, Command>();
    
    public void Initialize()
    {
        GameServices.Register(this);

        Debug.Log("EtherSystem initialized");
    }
    
    // Добавить команду в эфир с созданием триггера
    public void AddCommandToEther(Command command, int player)
    {
        // Создаём триггер для игрока
        TurnTrigger trigger = GameServices.Turn.CreateTriggerForCurrentPlayer();
        
        // Подписываемся на срабатывание триггера
        trigger.OnTurnReached += () => HandleTriggerActivated(trigger);
        
        // Сохраняем связку триггер-команда
        pendingCommands[trigger] = command;

        
        Debug.Log($"EtherSystem: команда добавлена в эфир для игрока {player}");
    }
    
    // Обработка срабатывания триггера
    private void HandleTriggerActivated(TurnTrigger trigger)
    {
        if (pendingCommands.ContainsKey(trigger))
        {
            Command command = pendingCommands[trigger];

            command.Execute();
            
            
            // Удаляем из словаря
            pendingCommands.Remove(trigger);

            command = null;
            trigger = null;
        }
    }
}