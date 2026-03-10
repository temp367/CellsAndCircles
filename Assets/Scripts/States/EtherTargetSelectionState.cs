// using System.Collections.Generic;
// using UnityEngine;

// // Состояние выбора цели для эфира
// public class EtherTargetSelectionState : GameState
// {
//     private CircleType targetType;
//     private int triggerX;
//     private int triggerY;
//     private EtherSystem ether;
    
//     public EtherTargetSelectionState(GameManager manager, CircleType type, int x, int y, EtherSystem e) : base(manager)
//     {
//         targetType = type;
//         triggerX = x;
//         triggerY = y;
//         ether = e;
//     }
    
//     public override void Enter()
//     {
//         Debug.Log($"EtherTargetSelectionState: выбор цели для {targetType}");
        
//         if (targetType == CircleType.Red)
//         {
//             // Для красного - подсвечиваем вражеские круги
//             List<Vector2Int> targets = new List<Vector2Int>();
//             for (int x = 0; x < grid.width; x++)
//             {
//                 for (int y = 0; y < grid.height; y++)
//                 {
//                     Circle circle = grid.GetCircleAt(x, y);
//                     if (circle != null && circle.Player != turn.CurrentPlayer && circle.Type != CircleType.Core)
//                     {
//                         targets.Add(new Vector2Int(x, y));
//                     }
//                 }
//             }
            
//             if (targets.Count == 0)
//             {
//                 ui.ShowHint("Нет доступных целей");
//                 gameManager.StateMachine.ChangeState(new MainGameState(gameManager));
//                 return;
//             }
            
//             grid.HighlightCells(targets, Color.red);
//             ui.ShowHint("Выберите цель для красного круга");
//         }
//         else
//         {
//             // Для синего/зелёного - подсвечиваем пустые клетки
//             List<Vector2Int> emptyCells = new List<Vector2Int>();
//             for (int x = 0; x < grid.width; x++)
//             {
//                 for (int y = 0; y < grid.height; y++)
//                 {
//                     if (grid.GetCircleAt(x, y) == null && !grid.HasBarrierAt(x, y))
//                     {
//                         emptyCells.Add(new Vector2Int(x, y));
//                     }
//                 }
//             }
            
//             grid.HighlightCells(emptyCells, Color.cyan);
//             ui.ShowHint("Выберите клетку для активации");
//         }
//     }
    
//     public override void Exit()
//     {
//         grid.ClearHighlights();
//     }
    
//     public override void HandleCellClick(int x, int y)
//     {
//         if (targetType == CircleType.Red)
//         {
//             // Выбор вражеского круга
//             Circle target = grid.GetCircleAt(x, y);
//             if (target != null && target.Player != turn.CurrentPlayer && target.Type != CircleType.Core)
//             {
//                 ether.CreateDelayedActivateFactory(triggerX, triggerY, x, y, targetType);
//                 gameManager.StateMachine.ChangeState(new MainGameState(gameManager));
//             }
//             else
//             {
//                 ui.ShowHint("Выберите вражеский круг (не Core)");
//             }
//         }
//         else
//         {
//             // Выбор пустой клетки
//             if (grid.GetCircleAt(x, y) == null && !grid.HasBarrierAt(x, y))
//             {
//                 ether.CreateDelayedActivateFactory(triggerX, triggerY, x, y, targetType);
//                 gameManager.StateMachine.ChangeState(new MainGameState(gameManager));
//             }
//             else
//             {
//                 ui.ShowHint("Выберите пустую клетку");
//             }
//         }
//     }
// }