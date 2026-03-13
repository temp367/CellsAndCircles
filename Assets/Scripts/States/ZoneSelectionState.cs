using System.Collections.Generic;
using UnityEngine;

// Состояние выбора зон в начале игры
public class ZoneSelectionState : GameState
{
    private Dictionary<int, int> zoneOwner = new Dictionary<int, int>(); // зона -> игрок
    
    public ZoneSelectionState(GameManager manager) : base(manager)
    {
    }
    
    public override void Enter()
    {
        // Очищаем владельцев зон
        zoneOwner.Clear();
        
        // Отключаем коллайдеры клеток (чтобы не мешали кликам по зонам)
        grid.DisableCellColliders();
        
        // Показываем подсказку
        ui.ShowHint($"Игрок {turn.CurrentPlayer}, выберите зону для установки ядра");

        // Обновляем UI
        ui.UpdatePlayerTurnText(turn.CurrentPlayer);
    }
    
    public override void Exit()
    {
        // Скрываем подсказку
        ui.ShowHint("");

        // Удаляем все оставшиеся зоны
        grid.RemoveAllZones();
        
        // Включаем коллайдеры клеток
        grid.EnableCellColliders();
    }
    
    public override void HandleZoneClick(int zoneNumber, int zoneX, int zoneY)
    {
        // Проверяем, не занята ли зона
        if (zoneOwner.ContainsKey(zoneNumber))
        {
            ui.ShowHint($"Зона {zoneNumber} уже занята игроком {zoneOwner[zoneNumber]}");
            return;
        }
        
        // Пытаемся поставить ядро
        bool success = grid.PlaceCircle(zoneX, zoneY, turn.CurrentPlayer, CircleType.Core);
        
        if (success)
        {
            // Запоминаем, что зона занята
            zoneOwner[zoneNumber] = turn.CurrentPlayer;
            
            // Удаляем визуальную зону
            grid.RemoveZone(zoneNumber);
            
            // Проверяем, не закончилась ли фаза выбора
            if (zoneOwner.Count == 2)
            {
                // Переходим в основную игру
                gameManager.SwitchToMainGame();
            }
            else
            {
                // Переключаем игрока через TurnManager
                turn.SwitchPlayer();
                
                // Обновляем подсказку
                ui.ShowHint($"Игрок {turn.CurrentPlayer}, выберите зону");
            }
        }
        else
        {
            Debug.LogError($"ZoneSelectionState: не удалось установить ядро");
        }
    }
}