using System;
using System.Collections.Generic;
using UnityEngine;

//Управление ходами
public class TurnManager : MonoBehaviour, IInitializable
{
    public int InitPriority => 1; // после GridManager
    public string SystemName => "TurnManager";
    
    private bool isInitialized = false;

    private int startingPlayer = 1; // игрок, который ходит первым

    private int currentPlayer;

    [Header("Менеджеры")]
    [SerializeField]private GridManager gridManager;
    [SerializeField]private UIManager uiManager;

    public int CurrentPlayer => currentPlayer;
    

    public System.Action<int> OnPlayerChanged;  // Событие для оповещения о смене игрока

    // Вызывается из GameManager после старта
    public bool Initialize()
    {
        try
        {
            if (!isInitialized)
            {
                currentPlayer = startingPlayer;
                OnPlayerChanged?.Invoke(currentPlayer);

                isInitialized = true;

                return isInitialized;   
            }
            else
            {
                return isInitialized;    
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"{this.name}: ошибка инициализации - {e.Message}");
            return isInitialized;
        }
    }

    // Смена игрока
    public void SwitchPlayer()
    {
        GameLogger.Log($"Смена хода: игрок {currentPlayer} -> игрок {(currentPlayer == 1 ? 2 : 1)}");

        // Переключаем игрока
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        // Оповещаем подписчиков
        OnPlayerChanged?.Invoke(currentPlayer);

        // Обновляем UI
        if (uiManager != null)
            uiManager.UpdatePlayerTurnText(currentPlayer);


        Debug.Log($"TurnManager: Ход игрока {currentPlayer}");
    }

    public TurnTrigger CreateTriggerForCurrentPlayer()
    {
        TurnTrigger trigger = new TurnTrigger(currentPlayer);
        
        OnPlayerChanged += (player) => {
        trigger?.SwitchTurn(currentPlayer);
        trigger?.CheckTurn(currentPlayer);
        };

        trigger.OnTurnReached += () => {
            // Когда триггер срабатывает, оповещаем подписчиков
            
        };

        return trigger;
    }

    // Проверка, является ли круг принадлежащим текущему игроку
    public bool IsOwnedByCurrentPlayer(Circle circle)
    {
        return circle != null && circle.Player == currentPlayer;
    }
}