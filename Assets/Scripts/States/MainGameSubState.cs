using UnityEngine;

// Базовый класс для подсостояний внутри MainGame
public abstract class MainGameSubState
{
    protected MainGameState mainGameState;
    protected GridManager grid;
    protected TurnManager turn;
    protected UIManager ui;
    
    public MainGameSubState(MainGameState state)
    {
        mainGameState = state;
        grid = state.Grid;
        turn = state.Turn;
        ui = state.UI;
    }
    
    public abstract void Enter();
    public abstract void Exit();
    public abstract void HandleCellClick(int x, int y);
}