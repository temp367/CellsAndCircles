using System.Collections.Generic;
using UnityEngine;

// Состояние выбора цели для фиолетового круга (удаление цепочки)
public class RemoveChainSelectionState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Circle> possibleTargets;
    
    public RemoveChainSelectionState(MainGameState state, Circle activator, List<Circle> targets) : base(state)
    {
        activatingCircle = activator;
        possibleTargets = targets;
    }
    
    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
        
        // Подсвечиваем возможные цели (соседние круги)
        List<Vector2Int> targetPositions = new List<Vector2Int>();

        foreach (Circle target in possibleTargets)
        {
            targetPositions.Add(new Vector2Int(target.GridX, target.GridY));
        }
        
        GameServices.Highlight.ShowCells(targetPositions, new Color(0.5f, 0f, 0.5f)); // фиолетовый
        
        GameServices.Ui.ShowHint("Выберите первый круг для удаления цепочки");
    }
    
    public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");
        GameServices.Highlight.Clear();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");
        
        Circle clickedCircle = GameServices.Grid.GetCircleAt(x, y);

        // Проверяем, что кликнули по одной из возможных целей
        if (possibleTargets.Contains(clickedCircle))
        {
            // Создаём команду для удаления цепочки
            Command command = new RemoveChainCommand(activatingCircle, clickedCircle, false);
            
            bool success = command.Execute();

            GameLog.Action(success
                ? $"SUCCESS chain removal started from ({x},{y})"
                : $"FAILED chain removal from ({x},{y})"
            );

            // Добавляем команду в историю
            GameServices.CommandSys.AddCommandToHistory(command, activatingCircle.Type, success, false);

            // Уведомляем AbilitySystem о выполненной команде
            GameServices.Ability.NotifyGameCommandExecuted(command);

            // Возвращаемся в обычный режим и переключаем ход
            mainGameState.ReturnToNormalAndSwitchPlayer();
        }
        else if(clickedCircle != null && clickedCircle.GridX == activatingCircle.GridX && clickedCircle.GridY == activatingCircle.GridY)
        {
            // Игрок кликнул по активирующему кругу - отмена
            GameLog.Action($"Player cancelled ability {GetType().Name}");

            GameServices.Ui.ShowHint($"Ход игрока {GameServices.Turn.CurrentPlayer}");
            mainGameState.ReturnToNormal();
        }
        else
        {
            // Неверная цель
            GameLog.Error($"Invalid target ({x},{y}) for {GetType().Name}");

            GameServices.Ui.ShowHint("Это не цель");
        }
    }
}