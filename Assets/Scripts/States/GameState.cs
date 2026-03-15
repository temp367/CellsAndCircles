using UnityEngine;

// Базовый класс для всех состояний игры
public abstract class GameState
{
    protected GameManager gameManager;
    protected GridManager grid;
    protected TurnManager turn;
    protected UIManager ui;
    protected CommandSystem cmds;

    public GridManager Grid => grid;
    public TurnManager Turn => turn;
    public UIManager UI => ui;
    public GameManager Game => gameManager;
    public CommandSystem Cmds => cmds;
    
    // Конструктор получает ссылки на все системы
    public GameState(GameManager manager)
    {
        gameManager = manager;
        grid = manager.gridManager;
        turn = manager.turnManager;
        ui = manager.uiManager;
        cmds = manager.commandSystem;
    }
    
    // Вызывается при входе в состояние
    public virtual void Enter() { }
    
    // Вызывается при выходе из состояния
    public virtual void Exit() { }
    
    // Обработка клика по клетке
    public virtual void HandleCellClick(int x, int y) { }
    
    // Обработка клика по зоне (для ZoneSelection)
    public virtual void HandleZoneClick(int zoneNumber, int zoneX, int zoneY) { }
    
    // Обновление каждый кадр (если нужно)
    public virtual void Update() { }
    public virtual void StartPlaceCircleEther(CircleType type) {}
    public virtual void StartActivateCircleEther() {}
    public virtual void StartTriggerEther(TriggerKind kindTrig, CircleType? type, GameManager gm) {}
}