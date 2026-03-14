using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour, IInitializable
{
    [Header("Лог")]
    public TextMeshProUGUI etherQueueText;

    [Header("Кнопки выбора типа круга")]
    public Button redButton;
    public Button blueButton;
    public Button greenButton;

    [Header("Управление сессиями")]
    public Button restartButton;
    
    [Header("Управление эфиром")]
    public Button etherButton;
    public Button cancelEtherButton;

    [Header("Панели эфира")]
    public GameObject etherMenuPanel;          // панель выбора действия
    public Button etherPlaceButton;            // кнопка "Поставить шар"
    public Button etherActivateButton;         // кнопка "Активировать шар"
    
    public GameObject etherPlacePanel;         // панель выбора типа для установки
    [Header("Кнопки выбора типа для установки")]
    public Button placeRedButton;
    public Button placeBlueButton;
    public Button placeGreenButton;

    public Button backFromPlaceButton;         // кнопка назад
    

    [Header("Панель выбора триггера")]
    public GameObject triggerPlacePanel;      
    public Button nextMyTurnButton;            // кнопка 1-го триггера             
    public Button backButton;  

    [Header("Тексты")]
    public TextMeshProUGUI playerTurnText;     // текст текущего игрока
    public TextMeshProUGUI etherHintText;      // текст подсказки для эфира

    
    private CircleType selectedPlaceType = CircleType.Red; // Текущий выбранный тип для установки

    // События для связи с GameManager
    public System.Action<CircleType> OnCircleTypeSelected;
    public System.Action OnRestartClicked;
    public System.Action<CircleType> OnPlaceTypeConfirmed;
    public System.Action OnActivateTypeConfirmed;
    public System.Action OnNextMyTurnClicked;
    public System.Action OnBackClicked;
    
    public InitStage InitStage => InitStage.UI;

    public void Initialize()
    {
        InitUIEvents();

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

        // Управление панелью эфира
        etherButton.onClick.AddListener(() => ShowEtherMenu(true));
        cancelEtherButton.onClick.AddListener(() => ShowEtherMenu(false));

        //  Переход к панелям Place
        etherPlaceButton.onClick.AddListener(() => {
            ShowEtherMenu(false);      
            ShowEtherPlacePanel(true); 
        });
        etherActivateButton.onClick.AddListener(() => {
            ShowEtherMenu(false);          
            OnActivateTypeSelected();
        });
    
        // Кнопки навигации внутри панелей
        backFromPlaceButton.onClick.AddListener(() => {
            ShowEtherPlacePanel(false);
            ShowEtherMenu(true); 
        });

        nextMyTurnButton.onClick.AddListener(() => OnNextMyTurnClicked?.Invoke());
        backButton.onClick.AddListener(() => OnBackClicked?.Invoke());
    }

    public void ShowTriggerPlacePanel(bool show)
    {
        if (triggerPlacePanel != null)
        {
            triggerPlacePanel.SetActive(show);        
        }
    }
    
    public void HideAllEtherPanels()
    {
        ShowEtherMenu(false);
        ShowEtherPlacePanel(false);
        ShowTriggerPlacePanel(false); 
    }

    private void OnPlaceTypeSelected(CircleType type)
    {
        selectedPlaceType = type;

        // Визуально подсвечиваем выбранную кнопку
        UpdatePlaceTypeButtons(type);

        ShowEtherPlacePanel(false);
        ShowHint("Выбери клетку для установки");
            
        OnPlaceTypeConfirmed?.Invoke(selectedPlaceType);
    }

    private void OnActivateTypeSelected()
    {
        ShowHint("Выбери клетку активации круга");
            
        OnActivateTypeConfirmed?.Invoke();
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

    // Показать/скрыть основную панель эфира
    public void ShowEtherMenu(bool show)
    {
        if (etherMenuPanel != null)
            etherMenuPanel.SetActive(show);
    }

    // Показать/скрыть панель установки круга
    public void ShowEtherPlacePanel(bool show)
    {
        if (etherPlacePanel != null)
            etherPlacePanel.SetActive(show);
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
        etherButton.interactable = true;
        cancelEtherButton.interactable = true;
        redButton.interactable = true;
        blueButton.interactable = true;
        greenButton.interactable = true;
    }
}