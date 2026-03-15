using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum InitStage
{
    Core = 0,        // базовые системы
    Grid = 10,       // поле и клетки
    Gameplay = 20,   // игровые системы
    Command = 21,   // команды действий
    Abil = 22,      // управление способностями юнитов
    Ethir = 23,     // управление эфиром
    UI = 30          // интерфейс
}

public class GameManager : MonoBehaviour
{
    public CircleType selectedCircleType = CircleType.Red;

    [Header("Системы")]
    public GridManager gridManager;
    public TurnManager turnManager;
    public CommandSystem commandSystem;
    public AbilitySystem abilitySystem;
    public UIManager uiManager;
    public EtherSystem etherSystem;

    private List<IInitializable> systems = new List<IInitializable>(); // Список всех инициализируемых систем

    [Header("State Machine")]
    public GameStateMachine StateMachine { get; private set; }

    public Command lastPendingCommand; // Храненит последнюю команду

    private void Awake()
    {
        GameLog.Initialize();
        
        // Собираем все системы
        RegisterSystems();
        
        InitializeSystems();
        // Запускаем инициализацию и игру
        StartGame();
    }

    private void RegisterSystems()
    {
        TryRegister(gridManager);
        TryRegister(turnManager);
        TryRegister(commandSystem);
        TryRegister(abilitySystem);
        TryRegister(etherSystem);
        TryRegister(uiManager);
    }
    
    void TryRegister(IInitializable system)
    {
        if(system == null)
        {
            Debug.LogError($"{system} missing!");
            return;
        }

        systems.Add(system);
    }

    private void InitializeSystems()
    {
        systems.Sort((a,b) => a.InitStage.CompareTo(b.InitStage));
        foreach (var system in systems) system.Initialize();

        StateMachine = new GameStateMachine(this);
        GameServices.Ability.SetStateMachine(StateMachine);
        // Подписываемся на события UI 
        SubscribeToEvents();
        
        
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
            StateMachine.StartPlaceCircleEther(type); //При следующем клике по клетке команда запишится в массив
        };

        GameServices.Ui.OnTriggerTypeConfirmed += (trigKind, typeCirc) =>
        {
            StateMachine.StartTriggerEther(trigKind, typeCirc, this);
        };

        uiManager.OnActivateTypeConfirmed += StateMachine.StartActivateCircleEther;
        

        turnManager.OnPlayerChanged += (player) => {
            gridManager?.HandlePlayerChanged(player);
        };

        GameServices.Ability.OnEtherCommandCreated += (command) => {
            GameServices.Ui.SwitchEtherPanel(GameServices.Ui.etherTriggerPanel);
            lastPendingCommand = command;
        };

        uiManager.OnBackClicked += () => {
            Debug.Log("GameManager: отмена команды"); 
        };
    }

    public void SelectCircleType(CircleType type)
    {
        selectedCircleType = type;
        uiManager.UpdateCircleTypeButtons(type);
    }

    // Главный обработчик кликов по клеткам
    public void HandleCellClick(int x, int y)
    {
        // Передаём в текущее состояние
        StateMachine.HandleCellClick(x, y);
    }

     // Для кликов по зонам (вызывается из ZoneCell)
    public void HandleZoneClick(int zoneNumber, int zoneX, int zoneY)
    {
        StateMachine.HandleZoneClick(zoneNumber, zoneX, zoneY);
    }
    
    // Метод для перехода в основную игру (может вызываться из других мест)
    public void SwitchToMainGame()
    {
        StateMachine.ChangeState(new MainGameState(this));
    }

    private void OnDestroy()
    {
       
    }

    public void RestartGame()
    {
        Debug.Log("Перезапуск игры...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}