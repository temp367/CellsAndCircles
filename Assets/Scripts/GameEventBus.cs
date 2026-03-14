using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Централизованный Event Bus для всех игровых событий.
/// Все системы подписываются на него вместо прямых ссылок друг на друга.
/// </summary>
public static class GameEventBus
{
    // Игровые события
    public static event Action<int> OnPlayerChanged;           // ход игрока
    public static event Action<Circle> OnCirclePlaced;        // круг размещён
    public static event Action<Circle> OnAbilityActivated;    // активирована способность
    public static event Action<Command> OnCommandExecuted;    // выполнена команда
    public static event Action<Command> OnCommandAddedToEther;// команда добавлена в эфир
    public static event Action<string> OnGameHint;            // текстовые подсказки
    public static event Action OnRestartGame;                 // перезапуск игры

    // Вспомогательные методы для вызова
    public static void PlayerChanged(int player) => OnPlayerChanged?.Invoke(player);
    public static void CirclePlaced(Circle circle) => OnCirclePlaced?.Invoke(circle);
    public static void AbilityActivated(Circle circle) => OnAbilityActivated?.Invoke(circle);
    public static void CommandExecuted(Command command) => OnCommandExecuted?.Invoke(command);
    public static void CommandAddedToEther(Command command) => OnCommandAddedToEther?.Invoke(command);
    public static void GameHint(string text) => OnGameHint?.Invoke(text);
    public static void RestartGame() => OnRestartGame?.Invoke();
}