using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    public Button confirmPlaceButton;          // кнопка подтверждения
    public Button backFromPlaceButton;         // кнопка назад
    
    public GameObject etherActivatePanel;      // панель выбора типа для активации

    [Header("Панель выбора триггера")]
    public GameObject triggerPlacePanel;      
    public Button nextMyTurnButton;            // кнопка 1-го триггера             
    public Button backButton;  
    
    [Header("Кнопки выбора типа для активации")]
    public Button activateRedButton;
    public Button activateBlueButton;
    public Button activateGreenButton;

    public Button confirmActivateTypeButton;   // кнопка подтверждения типа
    public Button backFromActivateTypeButton;  // кнопка назад

    [Header("Тексты")]
    public TextMeshProUGUI playerTurnText;     // текст текущего игрока
    public TextMeshProUGUI etherHintText;      // текст подсказки для эфира

    
    private CircleType selectedPlaceType = CircleType.Red; // Текущий выбранный тип для установки

    private CircleType selectedActivateType = CircleType.Red; // Текущий выбранный тип для активации

    // События для связи с GameManager
    public System.Action<CircleType> OnCircleTypeSelected;
    public System.Action OnRestartClicked;
    public System.Action<CircleType> OnPlaceTypeConfirmed;
    public System.Action<CircleType> OnActivateTypeConfirmed;
    public System.Action OnNextMyTurnClicked;
    public System.Action OnBackClicked;
    
    public int InitPriority => 3; // последний
    public string SystemName => "UIManager";
    
    private bool isInitialized = false;

    public bool Initialize()
    {
        try
        {
            if (!isInitialized)
            {
                InitUIEvents();
                isInitialized = true;
                Debug.Log($"{this.name}: инициализирован");
                return isInitialized;
            }
            else
            {
                return isInitialized;
            }
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{this.name}: ошибка инициализации - {e.Message}");
            return isInitialized;
        }
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
    
        // Кнопки выбора типа для активации (панель Activate)
        activateRedButton.onClick.AddListener(() => OnActivateTypeSelected(CircleType.Red));
        activateBlueButton.onClick.AddListener(() => OnActivateTypeSelected(CircleType.Blue));
        activateGreenButton.onClick.AddListener(() => OnActivateTypeSelected(CircleType.Green));

        // Управление панелью эфира
        etherButton.onClick.AddListener(() => ShowEtherMenu(true));
        cancelEtherButton.onClick.AddListener(() => ShowEtherMenu(false));

        //  Переход к панелям Place и Activate
        etherPlaceButton.onClick.AddListener(() => {
            ShowEtherMenu(false);      
            ShowEtherPlacePanel(true); 
        });
        etherActivateButton.onClick.AddListener(() => {
            ShowEtherMenu(false);          
            ShowEtherActivatePanel(true);  
        });
    
        // Кнопки навигации внутри панелей
        backFromPlaceButton.onClick.AddListener(() => {
            ShowEtherPlacePanel(false);
            ShowEtherMenu(true); 
        });
        backFromActivateTypeButton.onClick.AddListener(() => {
            ShowEtherActivatePanel(false);
            ShowEtherMenu(true); 
        });
         
        confirmPlaceButton.onClick.AddListener(() => {
            ShowEtherPlacePanel(false);
            ShowHint("Выбери клетку для установки");
            
            OnPlaceTypeConfirmed?.Invoke(selectedPlaceType);
        });
        confirmActivateTypeButton.onClick.AddListener(() => {
            ShowEtherActivatePanel(false);
            ShowHint("Выбери клетку активации круга");
            
            OnActivateTypeConfirmed?.Invoke(selectedActivateType);
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
        ShowEtherActivatePanel(false);
        ShowTriggerPlacePanel(false); 
    }

    private void OnPlaceTypeSelected(CircleType type)
    {
        selectedPlaceType = type;

        // Визуально подсвечиваем выбранную кнопку
        UpdatePlaceTypeButtons(type);
    }

    private void OnActivateTypeSelected(CircleType type)
    {
        selectedActivateType = type;

        // Визуально подсвечиваем выбранную кнопку
        UpdateActivateTypeButtons(type);
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

    // Подсветка выбранной кнопки в панели Activate
    private void UpdateActivateTypeButtons(CircleType selected)
    {
        if (activateRedButton != null)
            activateRedButton.interactable = (selected != CircleType.Red);
        if (activateBlueButton != null)
            activateBlueButton.interactable = (selected != CircleType.Blue);
        if (activateGreenButton != null)
            activateGreenButton.interactable = (selected != CircleType.Green);
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

    // Показать/скрыть панель активации круга
    public void ShowEtherActivatePanel(bool show)
    {
        etherActivatePanel.SetActive(show);
        
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

    // Получить выбранный тип 
    public CircleType GetSelectedPlaceType()
    {
        return selectedPlaceType;
    }

    // Получить выбранный тип 
    public CircleType GetSelectedActivateType()
    {
        return selectedActivateType;
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