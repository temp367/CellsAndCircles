using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CircleType selectedCircleType = CircleType.Red;

    [Header("Системы")]
    public GridManager gridManager;
    public TurnManager turnManager;
    public CommandSystem commandSystem;
    public UIManager uiManager;
    public EtherSystem etherSystem;

    private List<IInitializable> systems = new List<IInitializable>(); // Список всех инициализируемых систем
    private bool allSystemsInitialized = false;

    [Header("State Machine")]
    public GameStateMachine StateMachine { get; private set; }

    private Command lastPendingCommand; // Храненит последнюю команду

    private void Awake()
    {
        // Собираем все системы
        RegisterSystems();
        
        // Запускаем инициализацию и игру
        if(InitializeSystems()) StartGame();
    }

     private void RegisterSystems()
    {
        if (gridManager != null) systems.Add(gridManager);
        else Debug.LogError($"{gridManager.name}: GridManager не найден!");

        if (turnManager != null) systems.Add(turnManager);
        else Debug.LogError($"{turnManager.name}: TurnManager не найден!");

        if (commandSystem != null) systems.Add(commandSystem);
        else Debug.LogError($"{name}: CommandSystem не найден!");

        if (uiManager != null) systems.Add(uiManager);
        else Debug.LogError($"{uiManager.name}: UiManager не найден!");
        
        if (etherSystem != null) systems.Add(etherSystem);
        else Debug.LogError($"{etherSystem.name}: EtherSystem не найден!");
        
        // Сортируем по приоритету
        systems.Sort((a, b) => a.InitPriority.CompareTo(b.InitPriority));
    }

    private bool InitializeSystems()
    {
        try
        {
            //Инициализация зависимости
            foreach (var system in systems) system.Initialize();

            // Создаём машину состояний
            StateMachine = new GameStateMachine(this);

            // Подписываемся на события UI 
            SubscribeToEvents();
        
        }
        catch(Exception e)
        {
            Debug.LogError($"{this.name}: {e.Message}");
            return allSystemsInitialized;
        }

            allSystemsInitialized = true;

            return allSystemsInitialized;
    }
    
    private void StartGame()
    {
        // Начинаем с выбора зон
        StateMachine.ChangeState(new ZoneSelectionState(this));
    }

    private void SubscribeToEvents()
    {
        // UI события
        uiManager.OnCircleTypeSelected += SelectCircleType;
        uiManager.OnRestartClicked += RestartGame;

        uiManager.OnPlaceTypeConfirmed += (type) => {
            StateMachine.StartPlaceCircleEther(type);
        };

        uiManager.OnActivateTypeConfirmed += StateMachine.StartActivateCircleEther;
        

        turnManager.OnPlayerChanged += (player) => {
            gridManager?.HandlePlayerChanged(player);
        };

        PlaceCircleEtherState.OnCommandCreatedStatic += (command) => {
            uiManager.ShowTriggerPlacePanel(true);
            lastPendingCommand = command;
        };

        TargetSelectionEtherState.OnCommandActivateRedStatic += (command) =>
        {
            uiManager.ShowTriggerPlacePanel(true);
            lastPendingCommand = command;
        };
        BarrierSelectionEtherState.OnCommandActivateBlueStatic += (command) =>
        {
            uiManager.ShowTriggerPlacePanel(true);
            lastPendingCommand = command;
        };
        GreenReproductionEtherState.OnCommandActivateGreenStatic += (command) =>
        {
            uiManager.ShowTriggerPlacePanel(true);
            lastPendingCommand = command;
        };

        uiManager.OnBackClicked += () => {
            Debug.Log("GameManager: отмена команды"); 
        };

        uiManager.OnNextMyTurnClicked += () => {
            if (StateMachine.CurrentState is MainGameState mainGameState)
            {
                if (lastPendingCommand != null)
                {
                    etherSystem.AddCommandToEther(lastPendingCommand, turnManager.CurrentPlayer);
                    
                    Debug.Log($"Триггер активирован. Сработает в ваш следующий ход");
                    
                    // Очищаем временное хранение
                    lastPendingCommand = null;
                }
                else
                {
                    uiManager.ShowHint($"Ход игрока {turnManager.CurrentPlayer}");
                }

                mainGameState.ReturnToNormalAndSwitchPlayer();

                // Скрываем панель
                uiManager.HideAllEtherPanels();
            }
            
        };
    }

    public void SelectCircleType(CircleType type)
    {
        selectedCircleType = type;
        uiManager.UpdateCircleTypeButtons(type);
        //Debug.Log($"Выбран тип круга: {type}");
    }

    // Главный обработчик кликов по клеткам
    public void HandleCellClick(int x, int y)
    {
        GameLogger.LogClick(x, y, "клетка");
        
        // Передаём в текущее состояние
        StateMachine.HandleCellClick(x, y);
    }

     // Для кликов по зонам (вызывается из ZoneCell)
    public void HandleZoneClick(int zoneNumber, int zoneX, int zoneY)
    {
        GameLogger.LogClick(zoneX, zoneY, $"зона {zoneNumber}");
        StateMachine.HandleZoneClick(zoneNumber, zoneX, zoneY);
    }
    
    // Метод для перехода в основную игру (может вызываться из других мест)
    public void SwitchToMainGame()
    {
        StateMachine.ChangeState(new MainGameState(this));
    }

    // Прокси-методы для вызова из кругов
    public void StartTargetSelection(Circle activator, List<Circle> targets)
    {
        if (StateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartTargetSelection(activator, targets);
        }
    }
    public void StartTargetCellsSelectionEther(Circle activator, List<Vector2Int> targetCells)
    {
        if (StateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartTargetCellsSelectionEther(activator, targetCells);
        }
    }


    public void StartBarrierSelection(Circle blueCircle, List<Vector2Int> positions)
    {
        if (StateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartBarrierSelection(blueCircle, positions);
        }
    }
    public void StartBarrierCellsSelectionEther(Circle blueCircle, List<Vector2Int> positions)
    {
        if (StateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartBarrierCellsSelectionEther(blueCircle, positions);
        }
    }

    public void StartGreenReproduction(Circle greenCircle, List<Vector2Int> positions)
    {
        if (StateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartGreenReproduction(greenCircle, positions);
        }
    }
    public void StartGreenReproductionEther(Circle greenCircle, List<Vector2Int> positions)
    {
        if (StateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartGreenReproductionEther(greenCircle, positions);
        }
    }

    

    private void OnDestroy()
    {
        GameLogger.Log("=== ИГРА ЗАВЕРШЕНА ===");
        GameLogger.Shutdown();
    }

    public void RestartGame()
    {
        GameLogger.Log("=== ПЕРЕЗАПУСК ИГРЫ ===");
        GameLogger.Shutdown(); // закрываем текущий лог
        
        Debug.Log("Перезапуск игры...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}