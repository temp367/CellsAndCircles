using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GamePhase
{
    ZoneSelection, // Выбор зон и установка ядер
    MainGame       // Основная игра (зоны больше не нужны)
}

public enum ActionState
{
    Normal,          // ход игрока
    WaitingForTarget, // ожидание выбора цели для толчка (красные круги)
    WaitingForBarrierPlacement, // ожидание выбора барьера (синий круг)
    WaitingForGreenReproduction,  // для зелёного круга
}

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] public UIManager uiManager;
    [SerializeField] public TurnManager turnManager;
    [SerializeField] public EtherSystem etherSystem;
    [SerializeField] public GridManager gridManager;


    [Header("Фазы игры")]
    public GamePhase currentPhase = GamePhase.ZoneSelection;
    public ActionState currentActionState = ActionState.Normal;

    private Dictionary<int, int> zoneOwner = new Dictionary<int, int>(); // ключ: номер зоны (1-9), значение: игрок (1 или 2)


    private CircleType selectedCircleType;

    // Для активации способностей
    private Circle activatingRedCircle; // красный круг активирован
    private List<Circle> possibleTargets = new List<Circle>(); // список целей для толчка
    private Circle activatingBlueCircle; // синий круг активирован
    private List<Vector2Int> possibleBarrierPositions; // список клеток для барьера
    private Circle activatingGreenCircle; // зеленый круг активирован
    private List<Vector2Int> possibleGreenPositions; // список клеток для раздвоения


    private void Start()
    {
        InitializeManagers();
        SubscribeToEvents();
        InitializeTurnManager();
        UpdateUI();

    }

     private void InitializeManagers()
    {
        if (gridManager == null)
            Debug.LogError("GameManager: GridManager не найден!");
        if (uiManager == null)
            Debug.LogError("GameManager: UIManager не найден!");
        if (turnManager == null)
            Debug.LogError("GameManager: TurnManager не найден!");
        if (etherSystem == null)
            Debug.LogError("GameManager: EtherSystem не найден!");
    }

    private void SubscribeToEvents()
    {
        // UI события
        uiManager.OnCircleTypeSelected += SelectCircleType;
        uiManager.OnRestartClicked += RestartGame;
        
        // События эфира перенаправляем напрямую в EtherSystem
        uiManager.OnEtherClicked += () => etherSystem.StartEtherCreation();
        uiManager.OnCancelEtherClicked += () => etherSystem.CancelEtherCreation();
        uiManager.OnEtherPlaceClicked += () => etherSystem.OnEtherPlaceSelected();
        uiManager.OnEtherActivateClicked += () => etherSystem.OnEtherActivateSelected();
        uiManager.OnPlaceTypeSelected += (index) => etherSystem.OnPlaceTypeSelected(index);
        uiManager.OnConfirmPlaceClicked += () => etherSystem.OnConfirmPlace();
        uiManager.OnBackFromPlaceClicked += () => etherSystem.OnBackToEtherMenu();
        uiManager.OnActivateTypeSelected += (index) => etherSystem.OnActivateTypeSelected(index);
        uiManager.OnConfirmActivateTypeClicked += () => etherSystem.OnConfirmActivateType();
        uiManager.OnBackFromActivateTypeClicked += () => etherSystem.OnBackToEtherMenu();
    }

    private void InitializeTurnManager()
    {
        // Инициализация через публичный метод
        turnManager.Initialize();
        
        // Подписываемся на смену игрока для обновления UI
        turnManager.OnPlayerChanged += (player) => {
            UpdateUI();
            gridManager?.SetGlowForPlayer(player, true);
        };
    }

    public void SelectCircleType(CircleType type)
    {
        selectedCircleType = type;
        uiManager.UpdateCircleTypeButtons(type);
        Debug.Log($"Выбран тип круга: {type}");
    }

    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateCircleTypeButtons(selectedCircleType);
            uiManager.UpdatePlayerTurnText(turnManager.CurrentPlayer);
        }
    }

    // Главный обработчик кликов по клеткам
    public void HandleCellClick(int x, int y)
    {
        // 1. Сначала проверяем эфир (имеет приоритет)
        if (etherSystem.IsEtherActive() && etherSystem.HandleEtherCellClick(x, y))
            return;

        // 2. Проверяем состояния активации способностей
        if (currentActionState == ActionState.WaitingForTarget)
        {
            HandleTargetSelectionClick(x, y);
            return;
        }
        if (currentActionState == ActionState.WaitingForBarrierPlacement)
        {
            HandleBarrierSelectionClick(x, y);
            return;
        }
        if (currentActionState == ActionState.WaitingForGreenReproduction)
        {
            HandleGreenReproductionClick(x, y);
            return;
        }

        // 3. Обычный режим игры
        Circle circleOnCell = gridManager.GetCircleAt(x, y);

        if (circleOnCell != null)
        {
            if (circleOnCell.Player == turnManager.CurrentPlayer)
                HandleOwnCircleClick(circleOnCell, x, y);
            else
                HandleEnemyCircleClick();
        }
        else
        {
            HandleEmptyCellClick(x, y);
        }
    }

    // Обработка выбора зоны (для начальной фазы)
    public void HandleZoneSelection(int zoneNumber, int zoneX, int zoneY)
    {
        if (currentPhase != GamePhase.ZoneSelection)
            return;

        if (zoneOwner.ContainsKey(zoneNumber))
        {
            Debug.Log($"Зона {zoneNumber} уже занята");
            return;
        }

        bool success = gridManager.PlaceCircle(zoneX, zoneY, turnManager.CurrentPlayer, CircleType.Core);

        if (success)
        {
            zoneOwner[zoneNumber] = turnManager.CurrentPlayer;
            gridManager.RemoveZone(zoneNumber);

            if (zoneOwner.Count == 2) // Оба игрока походили
                SwitchToMainGame();
            else
                turnManager.SwitchPlayer();
        }
    }

    // Обработка клика по пустой клетке
    private void HandleEmptyCellClick(int x, int y)
    {
        bool success = gridManager.PlaceCircle(x, y, turnManager.CurrentPlayer, selectedCircleType);
        
        if (success) 
        {
            Debug.Log($"Игрок {turnManager.CurrentPlayer} ставит {selectedCircleType} на ({x}, {y})");
            turnManager.SwitchPlayer();
        }
    }

    // Обработка клика по своему кругу
    private void HandleOwnCircleClick(Circle circle, int x, int y)
    {
        if (circle.CanActivate)
        {
            bool hasTargets = circle.Activate();
            if (!hasTargets)
                Debug.Log("Нет целей для активации");
        }
        else
        {
            Debug.Log("Этот круг нельзя активировать");
        }
    }

    private void HandleEnemyCircleClick()
    {
        Debug.Log("Это круг противника");
    }

    // Обработка выбора цели (красный круг)
    private void HandleTargetSelectionClick(int x, int y)
    {
        Circle clickedCircle = gridManager.GetCircleAt(x, y);

        if (clickedCircle != null && possibleTargets.Contains(clickedCircle) && clickedCircle.CanBePushed)
        {
            if (activatingRedCircle is RedCircle red)
                red.PushTarget(clickedCircle);

            CompleteAction();
        }
    }

    // Обработка выбора клетки для барьера (синий круг)
    private void HandleBarrierSelectionClick(int x, int y)
    {
        Vector2Int clickedPos = new Vector2Int(x, y);

        if (possibleBarrierPositions.Contains(clickedPos))
        {
            bool success = gridManager.PlaceBarrier(x, y, turnManager.CurrentPlayer, gridManager.CurrentTurn);
            
            if (success)
                CompleteAction();
        }
    }

    // Обработка выбора клетки для размножения (зелёный круг)
    private void HandleGreenReproductionClick(int x, int y)
    {
        Vector2Int clickedPos = new Vector2Int(x, y);

        if (possibleGreenPositions.Contains(clickedPos) && activatingGreenCircle is GreenCircle green)
        {
            green.ReproduceAt(x, y);
            CompleteAction();
        }
    }

    // Завершение действия и смена хода
    private void CompleteAction()
    {
        gridManager.ClearHighlights();
        currentActionState = ActionState.Normal;
        
        activatingRedCircle = null;
        activatingBlueCircle = null;
        activatingGreenCircle = null;
        
        possibleTargets.Clear();
        possibleBarrierPositions?.Clear();
        possibleGreenPositions?.Clear();

        turnManager.SwitchPlayer();
    }

    // Публичные методы для запуска выбора целей (вызываются из кругов)
    public void StartTargetSelection(Circle activator, List<Circle> targets)
    {
        currentActionState = ActionState.WaitingForTarget;
        activatingRedCircle = activator;
        possibleTargets = targets;

        HighlightTargets(targets, Color.red);
        Debug.Log($"Выбор цели. Доступно: {targets.Count}");
    }

    public void StartBarrierSelection(Circle blueCircle, List<Vector2Int> positions)
    {
        currentActionState = ActionState.WaitingForBarrierPlacement;
        activatingBlueCircle = blueCircle;
        possibleBarrierPositions = positions;

        gridManager.HighlightCells(positions, Color.cyan);
        Debug.Log($"Выбор клетки для барьера. Доступно: {positions.Count}");
    }

    public void StartGreenReproduction(Circle greenCircle, List<Vector2Int> positions)
    {
        currentActionState = ActionState.WaitingForGreenReproduction;
        activatingGreenCircle = greenCircle;
        possibleGreenPositions = positions;

        gridManager.HighlightCells(positions, Color.green);
        Debug.Log($"Выбор клетки для размножения. Доступно: {positions.Count}");
    }

    private void HighlightTargets(List<Circle> targets, Color color)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach (Circle target in targets)
            positions.Add(new Vector2Int(target.GridX, target.GridY));
        
        gridManager.HighlightCells(positions, color);
    }

    public void SwitchToMainGame()
    {
        currentPhase = GamePhase.MainGame;
        gridManager.RemoveAllZones();
        gridManager.EnableCellColliders();
        turnManager.SwitchPlayer();
    }

    public void RestartGame()
    {
        Debug.Log("Перезапуск игры...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}