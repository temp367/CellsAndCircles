using System;
using System.Collections.Generic;
using UnityEngine;

public class EtherSystem : MonoBehaviour, IInitializable
{
    public int InitPriority => 4; // после UIManager
    public string SystemName => "EtherSystem";
    
    private bool isInitialized = false;
    
    // Словарь для хранения связи триггеров и команд
    private Dictionary<TurnTrigger, Command> pendingCommands = new Dictionary<TurnTrigger, Command>();
    
    // Ссылки на менеджеры
    private GameManager gameManager;
    private TurnManager turnManager;
    private CommandSystem commandSystem;
    private UIManager uiManager;
    
    public bool Initialize()
    {
        if (!isInitialized)
        {
            // Получаем ссылки на менеджеры
            gameManager = FindAnyObjectByType<GameManager>();
            turnManager = FindAnyObjectByType<TurnManager>();
            commandSystem = FindAnyObjectByType<CommandSystem>();
            uiManager = FindAnyObjectByType<UIManager>();
            
            isInitialized = true;
            
            return isInitialized;
        }
        else
        {
            return isInitialized;
        }
    }
    
    // Добавить команду в эфир с созданием триггера
    public void AddCommandToEther(Command command, int player)
    {
        // Создаём триггер для игрока
        TurnTrigger trigger = turnManager.CreateTriggerForCurrentPlayer();
        
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