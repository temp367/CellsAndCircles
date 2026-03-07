using UnityEngine;

//Управление ходами
public class TurnManager : MonoBehaviour
{
    private int startingPlayer = 1; // игрок, который ходит первым

    private int currentPlayer;
    private GridManager gridManager;
    private UIManager uiManager;

    public int CurrentPlayer => currentPlayer;

    public System.Action<int> OnPlayerChanged;  // Событие для оповещения о смене игрока

    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        uiManager = FindAnyObjectByType<UIManager>();

        if (gridManager == null)
        {
            Debug.LogError("TurnManager: GridManager не найден!");   
        }
        if (uiManager == null)
        {
            Debug.LogError("TurnManager: UiManager не найден!");
        }
    }

    // Вызывается из GameManager после старта
    public void Initialize()
    {
        currentPlayer = startingPlayer;
        OnPlayerChanged?.Invoke(currentPlayer);
        
        // Подсвечиваем круги первого игрока
        if (gridManager != null)
            gridManager.SetGlowForPlayer(currentPlayer, true);
        
        if (uiManager != null)
            uiManager.UpdatePlayerTurnText(currentPlayer);
    }

    // Смена игрока
    public void SwitchPlayer()
    {
        // Убираем подсветку у текущего игрока
        if (gridManager != null)
            gridManager.SetGlowForPlayer(currentPlayer, false);

        // Переключаем игрока
        currentPlayer = (currentPlayer == 1) ? 2 : 1;

        // Увеличиваем глобальный счётчик ходов
        if (gridManager != null)
        {
            gridManager.IncrementTurn();
            
            // Удаляем барьеры текущего игрока (того, чей ход начинается)
            gridManager.RemoveBarriersForPlayer(currentPlayer, gridManager.CurrentTurn);
            
            // Подсвечиваем круги нового игрока
            gridManager.SetGlowForPlayer(currentPlayer, true);
        }

        // Обновляем UI
        if (uiManager != null)
            uiManager.UpdatePlayerTurnText(currentPlayer);

        // Оповещаем подписчиков
        OnPlayerChanged?.Invoke(currentPlayer);

        Debug.Log($"TurnManager: Ход игрока {currentPlayer}");
    }

    // Сброс до начального игрока (например, при переходе в MainGame)
    /*public void ResetToFirstPlayer()
    {
        // Убираем подсветку у текущего
        if (gridManager != null)
            gridManager.SetGlowForPlayer(currentPlayer, false);

        currentPlayer = startingPlayer;

        // Подсвечиваем нового
        if (gridManager != null)
            gridManager.SetGlowForPlayer(currentPlayer, true);

        if (uiManager != null)
            uiManager.UpdatePlayerTurnText(currentPlayer);

        OnPlayerChanged?.Invoke(currentPlayer);
    }*/

    // Проверка, является ли круг принадлежащим текущему игроку
    public bool IsOwnedByCurrentPlayer(Circle circle)
    {
        return circle != null && circle.Player == currentPlayer;
    }
}