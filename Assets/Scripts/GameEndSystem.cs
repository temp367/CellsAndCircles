// using System.Collections.Generic;
// using UnityEngine;

// public class GameEndSystem
// {
//     private bool isGameEnded = false;

//     public void Initialize()
//     {
//         GameServices.Ability.OnGameCommandExecuted += OnCommandExecuted;
//     }

//     private void OnCommandExecuted(Command command)
//     {
//         if (isGameEnded) return;

//         CheckCoreRemoved();
//     }

//     private void OnPlayerChanged(int newPlayer)
//     {
//         if (isGameEnded) return;

//         CheckCoreSurrounded();
//     }

//     // =========================
//     // 🔴 Проверка: Core удалён
//     // =========================
//     private void CheckCoreRemoved()
//     {
//         List<Circle> cores = GameServices.Grid.GetAllCores();

//         if (cores.Count == 2)
//             return;

//         if (cores.Count == 1)
//         {
//             int winner = cores[0].Player;
//             EndGame(winner);
//             return;
//         }

//         if (cores.Count == 0)
//         {
//             Debug.Log("Оба Core уничтожены — ничья");
//             EndGame(-1); // -1 = ничья
//         }
//     }

//     // =========================
//     // 🔵 Проверка: Core окружён
//     // =========================
//     private void CheckCoreSurrounded()
//     {
//         List<Circle> cores = GameServices.Grid.GetAllCores();

//         foreach (var core in cores)
//         {
//             if (IsCoreSurrounded(core))
//             {
//                 int loser = core.Player;
//                 int winner = GetOpponent(loser);

//                 Debug.Log($"Core игрока {loser} окружён → победа игрока {winner}");

//                 EndGame(winner);
//                 return;
//             }
//         }
//     }

//     private bool IsCoreSurrounded(Circle core)
//     {
//         int enemyCount = 0;

//         for (int dx = -1; dx <= 1; dx++)
//         {
//             for (int dy = -1; dy <= 1; dy++)
//             {
//                 if (dx == 0 && dy == 0)
//                     continue;

//                 int x = core.GridX + dx;
//                 int y = core.GridY + dy;

//                 // вне поля — не считаем
//                 if (!GameServices.Grid.IsInside(x, y))
//                     continue;

//                 Circle circle = GameServices.Grid.GetCircleAt(x, y);

//                 // ❗ пустая клетка → не окружён
//                 if (circle == null)
//                     return false;

//                 if (circle.Player != core.Player)
//                     enemyCount++;
//             }
//         }

//         return enemyCount >= 2;
//     }

//     private int GetOpponent(int player)
//     {
//         return (player == 1) ? 2 : 1;
//     }

//     private void EndGame(int winner)
//     {
//         if (isGameEnded) return;

//         isGameEnded = true;

//         GameServices.Game.EndGame(winner);
//     }
// }