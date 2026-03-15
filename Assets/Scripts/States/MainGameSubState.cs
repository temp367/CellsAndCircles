using UnityEngine;

// Базовый класс для подсостояний внутри MainGame
public abstract class MainGameSubState
{
    protected MainGameState mainGameState;
    
    public MainGameSubState(MainGameState state)
    {
        mainGameState = state;
    }
    
    public abstract void Enter();
    public abstract void Exit();
    public abstract void HandleCellClick(int x, int y);
}