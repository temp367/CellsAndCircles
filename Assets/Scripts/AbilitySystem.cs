using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem : MonoBehaviour, IInitializable
{
    public event Action<Command> OnEtherCommandCreated;
    public event Action<Command> OnGameCommandExecuted;


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

    public void NotifyEtherCommandCreated(Command command) 
    {
        GameLog.Ether($"Ether command created {command.GetType().Name}");
        OnEtherCommandCreated?.Invoke(command);

        if (command is PlaceCircleCommand placeCmd)
        {
            GameServices.Highlight.HighlightCell(placeCmd.X, placeCmd.Y, Color.yellow
            );
        }
        else if (command is ReproduceCommand reproduceCmd)
        {
            GameServices.Highlight.ShowCells(new List<Vector2Int> { new Vector2Int(reproduceCmd.X, reproduceCmd.Y) }, Color.yellow);
        }
        else if (command is PushTargetCommand pushCmd)
        {
            GameServices.Highlight.ShowCells(
                new List<Vector2Int> { new Vector2Int(pushCmd.NewX, pushCmd.NewY),  new Vector2Int(pushCmd.OldX, pushCmd.OldY) }, 
                Color.yellow
            );
        }
        else if (command is PlaceBarrierCommand barCmd)
        {
            GameServices.Highlight.ShowCells(
                new List<Vector2Int> { new Vector2Int(barCmd.X, barCmd.Y) }, 
                Color.yellow
            );
        }
        else if (command is RemoveChainCommand remCmd)
        {
            GameServices.Highlight.ShowCells(
                new List<Vector2Int> { new Vector2Int(remCmd.targetCircle.GridX, remCmd.targetCircle.GridY) }, 
                Color.yellow
            );
        }
    }

    public void NotifyGameCommandExecuted(Command command) // обычная команда
    {
        OnGameCommandExecuted?.Invoke(command);   
    }
    

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

    // PINK
    public void StartRemoveChainSelection(Circle activator, List<Circle> targets)
    {
        if (stateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartRemoveChainSelection(activator, targets);
        }
    }

    // Для эфирной активации
    public void StartRemoveChainEtherSelection(Circle activator,  List<Vector2Int> targets)
    {
        if (stateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartRemoveChainEtherSelection(activator, targets);
        }
    }
}