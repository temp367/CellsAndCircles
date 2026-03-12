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
        
        // Сортируем по приоритету
        systems.Sort((a, b) => a.InitPriority.CompareTo(b.InitPriority));
    }

    private bool InitializeSystems()
    {
        Debug.Log("=== НАЧАЛО ИНИЦИАЛИЗАЦИИ СИСТЕМ ===");

        try
        {
            //Инициализация зависимости
            foreach (var system in systems) system.Initialize();

            // Подписываемся на события UI 
            SubscribeToEvents();
        
        }
        catch(Exception e)
        {
            Debug.LogError($"{this.name}: ошибка инициализации - {e.Message}");
            return allSystemsInitialized;
        }

            allSystemsInitialized = true;
            Debug.Log("=== ВСЕ СИСТЕМЫ УСПЕШНО ИНИЦИАЛИЗИРОВАНЫ ===");

            return allSystemsInitialized;
    }
    
    private void StartGame()
    {
        GameLogger.Log("GameManager: запуск игры");
        
        // Создаём машину состояний
        StateMachine = new GameStateMachine(this);
        
        // Начинаем с выбора зон
        StateMachine.ChangeState(new ZoneSelectionState(this));
    }

    private void SubscribeToEvents()
    {
        // UI события
        uiManager.OnCircleTypeSelected += SelectCircleType;
        uiManager.OnRestartClicked += RestartGame;

        uiManager.OnPlaceTypeConfirmed += (type) => {
            SwitchToPlaceCircleEtherState(type);
        };

        uiManager.OnActivateTypeConfirmed += (type) => {
            SwitchToActivateCircleEtherState(type);
        };

        turnManager.OnPlayerChanged += (player) => {
            gridManager?.HandlePlayerChanged(player);
        };

        commandSystem.OnCommandAddedToHistory += (command) => {
            uiManager.ShowTriggerPlacePanel(true);
            lastPendingCommand = command;
        };

        uiManager.OnBackClicked += () => {
            Debug.Log("GameManager: отмена команды"); 
        };

        uiManager.OnNextMyTurnClicked += () => {
            StateMachine.ChangeState(new MainGameState(this));
            uiManager.HideAllEtherPanels();
        };
    }

    // Метод для переключения в состояние установки круга через эфир
    public void SwitchToPlaceCircleEtherState(CircleType type)
    {
        StateMachine.StartPlaceCircleEther(type);
    }

    // Метод для переключения в состояние активации круга через эфир
    public void SwitchToActivateCircleEtherState(CircleType type)
    {
        StateMachine.StartActivateCircleEther(type);
    }

    public void SelectCircleType(CircleType type)
    {
        selectedCircleType = type;
        uiManager.UpdateCircleTypeButtons(type);
        Debug.Log($"Выбран тип круга: {type}");
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

    public void StartBarrierSelection(Circle blueCircle, List<Vector2Int> positions)
    {
        if (StateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartBarrierSelection(blueCircle, positions);
        }
    }

    public void StartGreenReproduction(Circle greenCircle, List<Vector2Int> positions)
    {
        if (StateMachine.CurrentState is MainGameState mainGameState)
        {
            mainGameState.StartGreenReproduction(greenCircle, positions);
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