using UnityEngine;
using System.Collections.Generic;

public class EtherSystem : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TurnManager turnManager;

    // Состояния эфира (теперь только внутри EtherSystem)
    private bool isEtherActive = false;
    private EtherActionType currentEtherAction;
    private CircleType pendingEtherCircleType;
    private CircleType pendingEtherActivateType;
    private int pendingEtherTriggerX;
    private int pendingEtherTriggerY;

     // Список отложенных действий с их условиями
    private List<PendingEtherAction> pendingActions = new List<PendingEtherAction>();

    // Класс для хранения действия с условием
    private class PendingEtherAction
    {
        public EtherAction Action { get; set; }
        public IEtherTrigger Trigger { get; set; }
        public bool IsActive { get; set; } = true;
        
        public PendingEtherAction(EtherAction action, IEtherTrigger trigger)
        {
            Action = action;
            Trigger = trigger;
        }
    }

    private enum EtherActionType
    {
        None,
        Place,
        Activate
    }

    private void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("EtherSystem: GridManager не найден!");   
        }
        if (uiManager == null)
        {
            Debug.LogError("EtherSystem: UIManager не найден!");
        }
        if (gameManager == null)
        {
            Debug.LogError("EtherSystem: GameManager не найден!");
        }
        if (turnManager == null)
        {
            Debug.LogError("EtherSystem: TurnManager не найден!");
        }
            
    }

    private void Update()
    {
        // Каждый кадр проверяем условия для всех pending действий
        CheckPendingActions();
    }

      private void CheckPendingActions()
    {
        for (int i = pendingActions.Count - 1; i >= 0; i--)
        {
            var pending = pendingActions[i];
            if (!pending.IsActive) continue;
            
            if (pending.Trigger.Check(gridManager, turnManager, pending.Action))
            {
                // Условие выполнено - выполняем действие
                Debug.Log($"EtherSystem: условие сработало для действия: {pending.Action.GetDescription()}");
                
                if (pending.Action.CanExecute(gridManager, turnManager))
                {
                    pending.Action.Execute(gridManager);
                }
                else
                {
                    Debug.Log($"EtherSystem: действие не может быть выполнено сейчас");
                    // Можно либо удалить, либо оставить для повторной проверки
                }
                
                // Удаляем выполненное действие
                pendingActions.RemoveAt(i);
                uiManager.UpdatePendingEtherList(GetPendingActionsDescription());
            }
        }
    }
    
    // Сохраняем действие "поставить круг" с условием
    public void SaveEtherPlaceAction(int x, int y, CircleType type)
    {
        Debug.Log($"Сохранено эфирное действие: поставить {type} круг на ({x}, {y})");
        
        // Создаём само действие
        EtherAction action = new PlaceEtherAction(turnManager.CurrentPlayer, x, y, type);
        
        // Создаём условие (например, враг толкнул мой синий круг)
        // Здесь нужно выбирать условие в зависимости от того, что выбрал игрок
        // Пока для теста создадим условие "начало моего хода"
        IEtherTrigger trigger = new MyTurnStartedTrigger(turnManager.CurrentPlayer);
        
        // Сохраняем
        pendingActions.Add(new PendingEtherAction(action, trigger));
        uiManager.UpdatePendingEtherList(GetPendingActionsDescription());
        
        Debug.Log($"EtherSystem: добавлено отложенное действие. Всего в очереди: {pendingActions.Count}");
    }
    
    // Сохраняем действие "активировать круг" с условием
    public void SaveEtherActivateAction(int triggerX, int triggerY, int targetX, int targetY, CircleType type)
    {
        Debug.Log($"Сохранено эфирное действие: активировать {type} круг с триггером ({triggerX},{triggerY}) и целью ({targetX},{targetY})");
        
        EtherAction action = new ActivateEtherAction(turnManager.CurrentPlayer, triggerX, triggerY, targetX, targetY, type);
        
        // Для активации тоже нужно условие, например "начало хода"
        IEtherTrigger trigger = new MyTurnStartedTrigger(turnManager.CurrentPlayer);
        
        pendingActions.Add(new PendingEtherAction(action, trigger));
        uiManager.UpdatePendingEtherList(GetPendingActionsDescription());
    }
    
    // Метод для добавления действия с произвольным условием
    public void AddEtherAction(EtherAction action, IEtherTrigger trigger)
    {
        pendingActions.Add(new PendingEtherAction(action, trigger));
    }
    
    // Получить список всех отложенных действий (для UI)
    public List<string> GetPendingActionsDescription()
    {
        List<string> descriptions = new List<string>();
        foreach (var pending in pendingActions)
        {
            descriptions.Add($"{pending.Action.GetDescription()} | Условие: {pending.Trigger.GetDescription()}");
        }
        return descriptions;
    }


    // Проверка, активен ли эфир
    public bool IsEtherActive()
    {
        return isEtherActive;
    }

    // Начать создание эфира (вызывается из GameManager)
    public void StartEtherCreation()
    {
        isEtherActive = true;
        currentEtherAction = EtherActionType.None;
        uiManager.ShowEtherMenu(true); // Показать основную панель эфира
        uiManager.ShowHint("Выберите действие для эфира");
    }

    // Отмена создания эфира
    public void CancelEtherCreation()
    {
        isEtherActive = false;
        currentEtherAction = EtherActionType.None;
        uiManager.HideAllEtherPanels();
        gridManager?.ClearHighlights();
        Debug.Log("EtherSystem: Режим создания эфира отменён");
    }

    // Выбрано действие "Поставить шар"
    public void OnEtherPlaceSelected()
    {
        currentEtherAction = EtherActionType.Place;
        uiManager.ShowEtherMenu(false);
        uiManager.ShowEtherPlacePanel(true);
        uiManager.ShowHint("Выбери тип круга");
    }

    // Выбрано действие "Активировать шар"
    public void OnEtherActivateSelected()
    {
        currentEtherAction = EtherActionType.Activate;
        uiManager.ShowEtherMenu(false);
        uiManager.ShowEtherActivatePanel(true);
    }

    // Выбран тип круга в выпадающем списке (для установки)
    public void OnPlaceTypeSelected(int index)
    {
        uiManager.SetConfirmPlaceButtonInteractable(true);// Активировать кнопку подтверждения для установки
    }

    // Подтверждение выбора типа для установки
    public void OnConfirmPlace()
    {
        pendingEtherCircleType = uiManager.GetSelectedPlaceType(); // Получить выбранный тип из выпадающего списка установки
        uiManager.ShowEtherPlacePanel(false); //скрыть панель установки круга
        uiManager.ShowHint("Кликните на любую клетку (кроме Core) для установки круга");
    }

    // Выбран тип круга в выпадающем списке (для активации)
    public void OnActivateTypeSelected(int index)
    {
        uiManager.SetConfirmActivateButtonInteractable(true); // Активировать кнопку подтверждения для активации
    }

    // Подтверждение выбора типа для активации
    public void OnConfirmActivateType()
    {
        pendingEtherActivateType = uiManager.GetSelectedActivateType(); // Получить выбранный тип из выпадающего списка активации
        uiManager.ShowEtherActivatePanel(false); //скрыть панель активации круга
        uiManager.ShowHint("Выберите клетку-триггер (не Core)");
    }

    // Возврат к меню эфира (кнопка "Назад")
    public void OnBackToEtherMenu()
    {
        uiManager.HideAllEtherPanels();
        uiManager.ShowEtherMenu(true);
        uiManager.ShowHint("Выберите действие для эфира");
    }

    // Обработка клика по клетке в режиме эфира
    public bool HandleEtherCellClick(int x, int y)
    {
        // Проверяем, активен ли эфир
        if (!isEtherActive)
            return false;

        // Проверяем, не кликнули ли по CoreCircle
        if (!IsCellValidForEther(x, y))
        {
            uiManager.ShowHint("Клетка занята CoreCircle, выберите другую");
            return true; // съели клик, но не обработали
        }

        // В зависимости от текущего действия
        switch (currentEtherAction)
        {
            case EtherActionType.Place:
                return HandlePlaceCellClick(x, y);
            case EtherActionType.Activate:
                return HandleActivateTriggerCellClick(x, y);
            default:
                return false;
        }
    }

    private bool HandlePlaceCellClick(int x, int y)
    {
        if (gridManager.HasBarrierAt(x, y))
        {
            uiManager.ShowHint("На клетке барьер, нельзя поставить круг");
            return true;
        }

        Debug.Log($"EtherSystem: Выбрана клетка ({x}, {y}) для установки круга {pendingEtherCircleType}");
        
        // Сохраняем эфирное действие (пока заглушка)
        SaveEtherPlaceAction(x, y, pendingEtherCircleType);

        // Завершаем эфир
        CompleteEtherAction();
        
        return true;
    }

    private bool HandleActivateTriggerCellClick(int x, int y)
    {
        if (gridManager.HasBarrierAt(x, y))
        {
            uiManager.ShowHint("На клетке барьер, нельзя поставить");
            return true;
        }

        pendingEtherTriggerX = x;
        pendingEtherTriggerY = y;
        
        // Переходим к выбору цели в зависимости от типа круга
        StartTargetSelectionForActivation();
        
        return true;
    }

    private void StartTargetSelectionForActivation()
    {
        uiManager.ShowHint($"Выберите цель для активации {pendingEtherActivateType} круга");
        
        switch (pendingEtherActivateType)
        {
            case CircleType.Red:
                StartRedTargetSelection();
                break;
            case CircleType.Blue:
                StartBlueTargetSelection();
                break;
            case CircleType.Green:
                StartGreenTargetSelection();
                break;
        }
    }

    private void StartRedTargetSelection()
    {
        // Собираем все вражеские круги (кроме Core)
        List<Circle> targets = new List<Circle>();
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                Circle circle = gridManager.GetCircleAt(x, y);
                if (circle != null && 
                    circle.Player != turnManager.CurrentPlayer && 
                    circle.Type != CircleType.Core)
                {
                    targets.Add(circle);
                }
            }
        }

        if (targets.Count == 0)
        {
            uiManager.ShowHint("Нет доступных целей");
            return;
        }

        // Подсвечиваем цели
        List<Vector2Int> targetPositions = new List<Vector2Int>();
        foreach (Circle target in targets)
        {
            targetPositions.Add(new Vector2Int(target.GridX, target.GridY));
        }
        gridManager.HighlightCells(targetPositions, Color.red);

        // Здесь нужно сохранить цели и ждать клика
        // Пока просто заглушка
        uiManager.ShowHint("Выберите цель (кликните по вражескому кругу)");
    }

    private void StartBlueTargetSelection()
    {
        // Для синего: выбор пустой клетки для барьера
        List<Vector2Int> emptyCells = new List<Vector2Int>();
        // ... логика сбора пустых клеток
        uiManager.ShowHint("Выберите клетку для барьера");
    }

    private void StartGreenTargetSelection()
    {
        // Для зелёного: выбор пустой клетки для размножения
        List<Vector2Int> emptyCells = new List<Vector2Int>();
        // ... логика сбора пустых клеток
        uiManager.ShowHint("Выберите клетку для размножения");
    }

    // Обработка клика по цели (для красного круга)
    public bool HandleTargetClick(int x, int y, Circle target)
    {
        // Проверяем, что цель не Core
        if (target.Type == CircleType.Core)
        {
            uiManager.ShowHint("Нельзя выбрать CoreCircle как цель");
            return true;
        }

        Debug.Log($"EtherSystem: Выбрана цель ({x}, {y}) для активации");
        
        // Сохраняем эфирное действие (пока заглушка)
        SaveEtherActivateAction(pendingEtherTriggerX, pendingEtherTriggerY, x, y, pendingEtherActivateType);

        // Завершаем эфир
        CompleteEtherAction();

        return true;
    }

    private void CompleteEtherAction()
    {
        isEtherActive = false;
        currentEtherAction = EtherActionType.None;
        gridManager.ClearHighlights();
        uiManager.HideAllEtherPanels();
        turnManager.SwitchPlayer();
    }

    private bool IsCellValidForEther(int x, int y)
    {
        Circle circle = gridManager.GetCircleAt(x, y);
        return circle == null || circle.Type != CircleType.Core;
    }
}