using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem : MonoBehaviour, IInitializable
{
    public event Action<Command> OnEtherCommandCreated;

    public InitStage InitStage => InitStage.Abil;

    public void Initialize()
    {
        GameServices.Register(this);

        Debug.Log("AbilitySystem initialized");
    }

    public GameStateMachine stateMachine;

    public void SetStateMachine(GameStateMachine machine)
    {
        stateMachine = machine;
    }

    public void NotifyCommandCreated(Command command) => OnEtherCommandCreated?.Invoke(command);
    
    // Прокси-методы для вызова из кругов
    public void StartTargetSelection(Circle activator, List<Circle> targets)
    {
        if (stateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartTargetSelection(activator, targets);
        }
    }
    public void StartTargetCellsSelectionEther(Circle activator, List<Vector2Int> targetCells)
    {
        if (stateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartTargetCellsSelectionEther(activator, targetCells);
        }
    }

    public void StartBarrierSelection(Circle blueCircle, List<Vector2Int> positions)
    {
        if (stateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartBarrierSelection(blueCircle, positions);
        }
    }
    public void StartBarrierCellsSelectionEther(Circle blueCircle, List<Vector2Int> positions)
    {
        if (stateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartBarrierCellsSelectionEther(blueCircle, positions);
        }
    }

    public void StartGreenReproduction(Circle greenCircle, List<Vector2Int> positions)
    {
        if (stateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartGreenReproduction(greenCircle, positions);
        }
    }
    public void StartGreenReproductionEther(Circle greenCircle, List<Vector2Int> positions)
    {
        if (stateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartGreenReproductionEther(greenCircle, positions);
        }
    }
}