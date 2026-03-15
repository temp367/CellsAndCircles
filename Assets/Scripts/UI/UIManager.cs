using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour, IInitializable
{
    [Header("Кнопки выбора типа круга для хода")]
    public Button redButton;
    public Button blueButton;
    public Button greenButton;

    [Header("Управление сессиями")]
    public Button restartButton;

    [Header("Ячейки эфира")]
    public GameObject etherCellsPanel;
    public Button firstEButton;
    public Button secondEButton;
    public Button threedEButton;

    [Header("Панель выбора команды для эфира")]
    public GameObject etherMenuPanel;          // панель выбора действия
    public Button etherPlaceButton;            // кнопка "Поставить шар"
    public Button etherActivateButton;         // кнопка "Активировать шар"
    
    [Header("Панель выбора типа круга для команды эфира")]
    public GameObject etherPutPanel;         // панель выбора типа для установки
    public Button placeRedButton;
    public Button placeBlueButton;
    public Button placeGreenButton;

    [Header("Панель выбора триггера")]
    public GameObject etherTriggerPanel;      
    public Button enemyPutButton;           
    public Button iPutButton;
    public Button enemyActivateButton;
    

    [Header("Панель выбора типа круга для триггера")]
    public GameObject etherTriggerTypePanel;
    public Button triggerRedButton;
    public Button triggerBlueButton;
    public Button triggerGreenButton;
    public Button triggerAnyButton;

    [Header("Панель выбора координаты круга для триггера")]
    public GameObject etherTriggerCoordinataPanel;
    public Button anyCoordinatButton;


    [Header("Тексты")]
    public TextMeshProUGUI playerTurnText;     // текст текущего игрока
    public TextMeshProUGUI etherHintText;      // текст подсказки для эфира
    public Button backButton; 

    
    private CircleType selectedPlaceType = CircleType.Red; // Текущий выбранный тип для установки
    private GameObject currentEtherPanel;

    // События для связи с GameManager
    public System.Action<CircleType> OnCircleTypeSelected;
    public System.Action OnRestartClicked;
    public System.Action<CircleType> OnPlaceTypeConfirmed;
    public System.Action<TriggerKind, CircleType?> OnTriggerTypeConfirmed;
    public System.Action OnActivateTypeConfirmed;
    public System.Action OnBackClicked;

    private CircleType? selectedTypeForTrigger;
    private TriggerKind currentTriggerKind;
    
    public InitStage InitStage => InitStage.UI;

    public void Initialize()
    {
        InitUIEvents();

        currentEtherPanel = etherCellsPanel;

        GameServices.Register(this);

        Debug.Log("UIManager initialized");
    }

    private void InitUIEvents()
    {
        // Подписываемся на события кнопок
        redButton.onClick.AddListener(() => OnCircleTypeSelected?.Invoke(CircleType.Red));
        blueButton.onClick.AddListener(() => OnCircleTypeSelected?.Invoke(CircleType.Blue));
        greenButton.onClick.AddListener(() => OnCircleTypeSelected?.Invoke(CircleType.Green));
        restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());

        // Кнопки выбора типа для установки (панель Place)
        placeRedButton.onClick.AddListener(() => OnPlaceTypeSelected(CircleType.Red));
        placeBlueButton.onClick.AddListener(() => OnPlaceTypeSelected(CircleType.Blue));
        placeGreenButton.onClick.AddListener(() => OnPlaceTypeSelected(CircleType.Green));

        
        //Кнопки для выбора типа круга триггера
        triggerRedButton.onClick.AddListener(() =>
        {
            OnTriggerTypeSelected(CircleType.Red);
            SwitchEtherPanel(etherTriggerCoordinataPanel);
        });

        triggerBlueButton.onClick.AddListener(() =>
        {
            OnTriggerTypeSelected(CircleType.Blue);
            SwitchEtherPanel(etherTriggerCoordinataPanel);
        });

        triggerGreenButton.onClick.AddListener(() =>
        {
            OnTriggerTypeSelected(CircleType.Green);
            SwitchEtherPanel(etherTriggerCoordinataPanel);
        });

        triggerAnyButton.onClick.AddListener(() =>
        {
            OnTriggerTypeSelected(null);
            SwitchEtherPanel(etherTriggerCoordinataPanel);
        });

        // Кнопки выбора триггера для команды эфира
        enemyPutButton.onClick.AddListener(() =>
        {
            currentTriggerKind = TriggerKind.EnemyPlaceCircle;
            SwitchEtherPanel(etherTriggerTypePanel);
        });

        iPutButton.onClick.AddListener(() =>
        {
            currentTriggerKind = TriggerKind.SelfPlaceCircle;
            SwitchEtherPanel(etherTriggerTypePanel);
        });

        enemyActivateButton.onClick.AddListener(() =>
        {
            currentTriggerKind = TriggerKind.enemyActivate;
            SwitchEtherPanel(etherTriggerTypePanel);
        });

        // Управление панелью эфира
        firstEButton.onClick.AddListener(() => {
            SwitchEtherPanel(etherMenuPanel);
        });

        secondEButton.onClick.AddListener(() => {
            SwitchEtherPanel(etherMenuPanel);
        });
        
        threedEButton.onClick.AddListener(() => {
            SwitchEtherPanel(etherMenuPanel);
        });
        

        //  Переход к панелям Place
        etherPlaceButton.onClick.AddListener(() => {
            SwitchEtherPanel(etherPutPanel);
        });

        etherActivateButton.onClick.AddListener(() => {
            OnActivateTypeSelected();
        });
    
        // Кнопки навигации внутри панелей
        backButton.onClick.AddListener(() => {
        });
    }

    public void SwitchEtherPanel(GameObject newPanel)
    {
        if (currentEtherPanel != null)
        {
            currentEtherPanel.SetActive(false);
        }
            
        currentEtherPanel = newPanel;

        if (currentEtherPanel != null)
        {
            Debug.Log("UI Panel → " + currentEtherPanel.name);
            currentEtherPanel.SetActive(true);   
        }
    }

    public void HideAllEtherPanels()
    {
        if (currentEtherPanel != null)
            currentEtherPanel.SetActive(false);

        currentEtherPanel = null;
    }

    private void OnPlaceTypeSelected(CircleType type)
    {
        selectedPlaceType = type;

        // Визуально подсвечиваем выбранную кнопку
        UpdatePlaceTypeButtons(type);
    
        OnPlaceTypeConfirmed?.Invoke(selectedPlaceType);

        ShowHint("Выбери клетку для установки");
    }

    private void OnTriggerTypeSelected(CircleType? type)
    {
        selectedTypeForTrigger = type;
        OnTriggerTypeConfirmed?.Invoke(currentTriggerKind, selectedTypeForTrigger);

        ShowHint("Можешь Выбрать клетку или оставить любую");
    }

    private void OnActivateTypeSelected()
    {
        OnActivateTypeConfirmed?.Invoke();
        
        ShowHint("Выбери клетку активации круга");
    }

    // Подсветка выбранной кнопки в панели Place
    private void UpdatePlaceTypeButtons(CircleType selected)
    {
        if (placeRedButton != null)
            placeRedButton.interactable = (selected != CircleType.Red);
        if (placeBlueButton != null)
            placeBlueButton.interactable = (selected != CircleType.Blue);
        if (placeGreenButton != null)
            placeGreenButton.interactable = (selected != CircleType.Green);
    }

    // Обновление текста игрока
    public void UpdatePlayerTurnText(int player)
    {
        if (playerTurnText != null)
        {
            playerTurnText.text = $"Ход игрока: {player}";
        }
    }

    // Обновление подсветки кнопок выбора типа
    public void UpdateCircleTypeButtons(CircleType selectedType)
    {
        redButton.interactable = (selectedType != CircleType.Red);
        blueButton.interactable = (selectedType != CircleType.Blue);
        greenButton.interactable = (selectedType != CircleType.Green);
    }

    // Показать подсказку
    public void ShowHint(string hint)
    {
        if (etherHintText != null)
        {
            etherHintText.text = hint;
            etherHintText.gameObject.SetActive(!string.IsNullOrEmpty(hint));
        }
    }

    public void GetMainGameView()
    {
        firstEButton.interactable = true;
        secondEButton.interactable = true;
        threedEButton.interactable = true;
        redButton.interactable = true;
        blueButton.interactable = true;
        greenButton.interactable = true;
    }
}