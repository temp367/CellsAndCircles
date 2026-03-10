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
        Debug.Log("ZoneSelectionState: вошли в состояние выбора зон");
        
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
        Debug.Log("ZoneSelectionState: выходим из состояния выбора зон");
        
        // Скрываем подсказку
        ui.ShowHint("");
    }
    
    public override void HandleZoneClick(int zoneNumber, int zoneX, int zoneY)
    {
        Debug.Log($"ZoneSelectionState: клик по зоне {zoneNumber}, центр ({zoneX}, {zoneY})");
        
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
            
            Debug.Log($"ZoneSelectionState: игрок {turn.CurrentPlayer} занял зону {zoneNumber}");
            
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