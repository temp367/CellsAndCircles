using System.Collections.Generic;
using UnityEngine;

public class MainGameState : GameState
{
    // Текущее подсостояние (null = обычный режим)
    private MainGameSubState currentSubState;
    
    public MainGameState(GameManager manager) : base(manager)
    {
    }
    
    public override void Enter()
    {
        Debug.Log("MainGameState: вошли в основную игру");
        
        // Удаляем все оставшиеся зоны
        grid.RemoveAllZones();
        
        // Включаем коллайдеры клеток
        grid.EnableCellColliders();
        
        // Сбрасываем подсветку
        grid.ClearHighlights();
        
        // Сбрасываем подсостояние
        currentSubState = null;

        turn.SwitchPlayer();
        
        // Показываем подсказку
        ui.ShowHint($"Ход игрока {turn.CurrentPlayer}. Выберите действие.");
    }
    
    public override void Exit()
    {
        // Если есть активное подсостояние, выходим из него
        if (currentSubState != null)
        {
            currentSubState.Exit();
            currentSubState = null;
        }
        
        // Убираем подсветку
        //grid.SetGlowForPlayer(turn.CurrentPlayer, false);
        grid.ClearHighlights();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        // Если есть активное подсостояние, передаём клик ему
        if (currentSubState != null)
        {
            currentSubState.HandleCellClick(x, y);
            return;
        }
        
        // Обычный режим
        Circle circleOnCell = grid.GetCircleAt(x, y);
        
        if (circleOnCell != null)
        {
            if (turn.IsOwnedByCurrentPlayer(circleOnCell))
            {
                HandleOwnCircleClick(circleOnCell, x, y);
            }
            else
            {
                ui.ShowHint("Это круг противника");
            }
        }
        else
        {
            HandleEmptyCellClick(x, y);
        }
    }
    
    private void HandleEmptyCellClick(int x, int y)
    {
        CircleType selectedType = gameManager.selectedCircleType;
        
        if(selectedType != CircleType.Core)
        {
            bool success = grid.PlaceCircle(x, y, turn.CurrentPlayer, selectedType);
        
            if (success) 
            {
                GameLogger.Log($"Игрок {turn.CurrentPlayer} установил {selectedType} на ({x}, {y})");
                Debug.Log($"MainGameState: игрок {turn.CurrentPlayer} ставит {selectedType} круг на ({x}, {y})");
                ReturnToNormalAndSwitchPlayer();
            }
            else
            {
                ui.ShowHint("Нельзя поставить круг сюда");
            }
        }
        else
        {
            ui.ShowHint("Core нельзя установить несколько раз(");
        }
        
    }
    
    private void HandleOwnCircleClick(Circle circle, int x, int y)
    {
        if (circle.CanActivate)
        {
            Debug.Log($"MainGameState: активация {circle.Type} круга");
            
            // Круг сам собирает цели и сообщает через GameManager
            bool hasTargets = circle.Activate();
            
            if (!hasTargets)
            {
                ui.ShowHint("Нет целей для активации");
            }
            // Если цели есть - круг вызовет соответствующий метод
            // (StartTargetSelection, StartBarrierSelection и т.д.)
        }
        else
        {
            ui.ShowHint("Этот круг нельзя активировать");
        }
    }
    
    // Методы для переключения в подсостояния (вызываются из GameManager)
    public void StartTargetSelection(Circle activator, List<Circle> targets)
    {
        if (currentSubState != null)
            currentSubState.Exit();
            
        currentSubState = new TargetSelectionState(this, activator, targets);
        currentSubState.Enter();
    }
    
    public void StartBarrierSelection(Circle activator, List<Vector2Int> positions)
    {
        if (currentSubState != null)
            currentSubState.Exit();
            
        currentSubState = new BarrierSelectionState(this, activator, positions);
        currentSubState.Enter();
    }
    
    public void StartGreenReproduction(Circle activator, List<Vector2Int> positions)
    {
        if (currentSubState != null)
            currentSubState.Exit();
            
        currentSubState = new GreenReproductionState(this, activator, positions);
        currentSubState.Enter();
    }
    
    // Возврат в обычный режим с переключением хода
    public void ReturnToNormalAndSwitchPlayer()
    {
        // Выходим из подсостояния, если было
        if (currentSubState != null)
        {
            currentSubState.Exit();
            currentSubState = null;
        }
        
        // Переключаем игрока
        turn.SwitchPlayer();
        
        // Обновляем подсказку
        ui.ShowHint($"Ход игрока {turn.CurrentPlayer}");
        
        // Подсветка кругов обновится в SwitchPlayer через GridManager.SetGlowForPlayer
    }

    public void ReturnToNormal()
    {
        // Выходим из подсостояния, если было
        if (currentSubState != null)
        {
            currentSubState.Exit();
            currentSubState = null;
        }
    }
}