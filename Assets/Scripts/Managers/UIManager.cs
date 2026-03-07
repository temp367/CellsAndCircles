using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Эфирные параметры")]
    public TextMeshProUGUI pendingEtherText;

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
    public TMP_Dropdown circleTypeDropdown;    // выпадающий список
    public Button confirmPlaceButton;          // кнопка подтверждения
    public Button backFromPlaceButton;         // кнопка назад
    
    public GameObject etherActivatePanel;      // панель выбора типа для активации
    public TMP_Dropdown activateTypeDropdown;  // выпадающий список
    public Button confirmActivateTypeButton;   // кнопка подтверждения типа
    public Button backFromActivateTypeButton;  // кнопка назад

    [Header("Тексты")]
    public TextMeshProUGUI playerTurnText;     // текст текущего игрока
    public TextMeshProUGUI etherHintText;      // текст подсказки для эфира

    // События для связи с GameManager
    public System.Action<CircleType> OnCircleTypeSelected;
    public System.Action OnRestartClicked;
    public System.Action OnEtherClicked;
    public System.Action OnCancelEtherClicked;
    public System.Action OnEtherPlaceClicked;
    public System.Action OnEtherActivateClicked;
    public System.Action<int> OnPlaceTypeSelected;      // индекс из выпадающего списка
    public System.Action OnConfirmPlaceClicked;
    public System.Action OnBackFromPlaceClicked;
    public System.Action<int> OnActivateTypeSelected;   // индекс из выпадающего списка
    public System.Action OnConfirmActivateTypeClicked;
    public System.Action OnBackFromActivateTypeClicked;

    private void Start()
    {
        // Подписываемся на события кнопок
        redButton.onClick.AddListener(() => OnCircleTypeSelected?.Invoke(CircleType.Red));
        blueButton.onClick.AddListener(() => OnCircleTypeSelected?.Invoke(CircleType.Blue));
        greenButton.onClick.AddListener(() => OnCircleTypeSelected?.Invoke(CircleType.Green));
        restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
        etherButton.onClick.AddListener(() => OnEtherClicked?.Invoke());
        cancelEtherButton.onClick.AddListener(() => OnCancelEtherClicked?.Invoke());
        
        etherPlaceButton.onClick.AddListener(() => OnEtherPlaceClicked?.Invoke());
        etherActivateButton.onClick.AddListener(() => OnEtherActivateClicked?.Invoke());
        
        circleTypeDropdown.onValueChanged.AddListener((index) => OnPlaceTypeSelected?.Invoke(index));
        confirmPlaceButton.onClick.AddListener(() => OnConfirmPlaceClicked?.Invoke());
        backFromPlaceButton.onClick.AddListener(() => OnBackFromPlaceClicked?.Invoke());
        
        activateTypeDropdown.onValueChanged.AddListener((index) => OnActivateTypeSelected?.Invoke(index));
        confirmActivateTypeButton.onClick.AddListener(() => OnConfirmActivateTypeClicked?.Invoke());
        backFromActivateTypeButton.onClick.AddListener(() => OnBackFromActivateTypeClicked?.Invoke());
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
        
        // Сбрасываем состояние кнопки подтверждения
        if (show && confirmPlaceButton != null)
            confirmPlaceButton.interactable = false;
    }

    // Показать/скрыть панель активации круга
    public void ShowEtherActivatePanel(bool show)
    {
        if (etherActivatePanel != null)
            etherActivatePanel.SetActive(show);
        
        // Сбрасываем состояние кнопки подтверждения
        if (show && confirmActivateTypeButton != null)
            confirmActivateTypeButton.interactable = false;
    }

    // Активировать кнопку подтверждения для установки
    public void SetConfirmPlaceButtonInteractable(bool interactable)
    {
        if (confirmPlaceButton != null)
            confirmPlaceButton.interactable = interactable;
    }

    // Активировать кнопку подтверждения для активации
    public void SetConfirmActivateButtonInteractable(bool interactable)
    {
        if (confirmActivateTypeButton != null)
            confirmActivateTypeButton.interactable = interactable;
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

    // Скрыть все панели эфира
    public void HideAllEtherPanels()
    {
        ShowEtherMenu(false);
        ShowEtherPlacePanel(false);
        ShowEtherActivatePanel(false);
        ShowHint("");
    }

    // Получить выбранный тип из выпадающего списка установки
    public CircleType GetSelectedPlaceType()
    {
        int index = circleTypeDropdown != null ? circleTypeDropdown.value : 0;
        return index switch
        {
            0 => CircleType.Red,
            1 => CircleType.Blue,
            2 => CircleType.Green,
            _ => CircleType.Red,
        };
    }

    // Получить выбранный тип из выпадающего списка активации
    public CircleType GetSelectedActivateType()
    {
        int index = activateTypeDropdown != null ? activateTypeDropdown.value : 0;
        return index switch
        {
            0 => CircleType.Red,
            1 => CircleType.Blue,
            2 => CircleType.Green,
            _ => CircleType.Red
        };
    }

    public void UpdatePendingEtherList(List<string> descriptions)
    {
        if (pendingEtherText != null)
        {
            if (descriptions.Count == 0)
            {
                pendingEtherText.text = "Нет отложенных действий";
            }
            else
            {
                string text = "Отложенные действия:\n";
                foreach (var desc in descriptions)
                {
                    text += $"• {desc}\n";
                }
                pendingEtherText.text = text;
            }
        }
    }
}



