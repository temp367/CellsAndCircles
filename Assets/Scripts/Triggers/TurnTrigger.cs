using System;
using UnityEngine;

public class TurnTrigger
{
    public int TargetPlayer { get; private set; }  // Игрок, чей ход ждём
    public int TargetTurn { get; private set; }     // Номер хода, когда был создан
    public bool IsActive { get; private set; }      // Активен ли триггер
    
    private int startTurn = 0;
    
    // Событие, которое сработает, когда наступит нужный ход
    public event Action OnTurnReached;
    
    public TurnTrigger(int targetPlayer)
    {
        TargetPlayer = targetPlayer;
        IsActive = true;
        TargetTurn = 1;
        
        
        Debug.Log($"TurnTrigger: создан для игрока {targetPlayer} в ход {TargetTurn}");
    }

    public void SwitchTurn(int playr)
    {
        TargetTurn++;
    }
    
    // Вызывается каждый раз при смене хода
    public void CheckTurn(int currentPlayer)
    {
        if (!IsActive) return;
        
        // Проверяем, наступил ли ход целевого игрока И прошёл ли хотя бы один ход
        if (currentPlayer == TargetPlayer && TargetTurn > startTurn)
        {
            Debug.Log($"TurnTrigger: сработал для игрока {TargetPlayer} в ход {TargetTurn}");
            
            IsActive = false;
            OnTurnReached?.Invoke();
        }
    }
}