using UnityEngine;

// Машина состояний - управляет переключением между состояниями
public class GameStateMachine
{
    private GameState currentState;
    private GameManager gameManager;
    
    public GameState CurrentState => currentState;
    
    public GameStateMachine(GameManager manager)
    {
        gameManager = manager;
    }
    
    // Переключение на новое состояние
    public void ChangeState(GameState newState)
    {
        string fromState = currentState?.GetType().Name ?? "null";
        string toState = newState.GetType().Name;
        
        // Выходим из текущего состояния
        if (currentState != null)
        {
            currentState.Exit();
        }
        
        // Переключаемся на новое
        currentState = newState;
        
        // Входим в новое состояние
        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    public void StartPlaceCircleEther(CircleType type)
    {
        if (currentState != null)
        {
            currentState.StartPlaceCircleEther(type);
        }
    }

    public void StartTriggerEther(TriggerKind kindTrig, CircleType? typeCirc, GameManager gm)
    {
        if (currentState != null)
        {
            currentState.StartTriggerEther(kindTrig, typeCirc, gm);
        }
    }

    public void StartActivateCircleEther()
    {
        if (currentState != null)
        {
            currentState.StartActivateCircleEther();
        }
    }
    
    // Пробрасываем события в текущее состояние
    public void HandleCellClick(int x, int y)
    {
        if (currentState != null)
        {
            currentState.HandleCellClick(x, y);
        }
    }
    
    public void HandleZoneClick(int zoneNumber, int zoneX, int zoneY)
    {
        if (currentState != null)
        {
            currentState.HandleZoneClick(zoneNumber, zoneX, zoneY);
        }
    }
    
    // public void Update()
    // {
    //     if (currentState != null)
    //     {
    //         currentState.Update();
    //     }
    // }

    public void Stop()
    {
        if(currentState != null)
            currentState.Exit();
    
        currentState = null;
    
        Debug.Log("GameStateMachine остановлена");
    }
}