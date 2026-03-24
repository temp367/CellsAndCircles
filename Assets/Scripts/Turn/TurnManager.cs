using System;
using System.Collections.Generic;
using UnityEngine;

//Управление ходами
public class TurnManager : MonoBehaviour, IInitializable
{
    public InitStage InitStage => InitStage.Gameplay;

    private int startingPlayer = 1; // игрок, который ходит первым

    private int currentPlayer;

    [Header("Менеджеры")]
    [SerializeField]private GridManager gridManager;
    [SerializeField]private UIManager uiManager;

    public int CurrentPlayer => currentPlayer;
    

    public System.Action<int> OnPlayerChanged;  // Событие для оповещения о смене игрока

    // Вызывается из GameManager после старта
    public void Initialize()
    {
        currentPlayer = startingPlayer;
        OnPlayerChanged?.Invoke(currentPlayer);

        GameServices.Register(this);

        Debug.Log("TurnManager initialized");
    }

    // Смена игрока
    public void SwitchPlayer()
    {
        // Переключаем игрока
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        // Оповещаем подписчиков
        OnPlayerChanged?.Invoke(currentPlayer);

        // Обновляем UI
        GameServices.Ui.UpdatePlayerTurnText(currentPlayer);
        GameServices.Ui.UpdatePlayerPanel(currentPlayer);

        Debug.Log($"TurnManager: Ход игрока {currentPlayer}");
    }

    // Проверка, является ли круг принадлежащим текущему игроку
    public bool IsOwnedByCurrentPlayer(Circle circle)
    {
        return circle != null && circle.Player == currentPlayer;
    }
}