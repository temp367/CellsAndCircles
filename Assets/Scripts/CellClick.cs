using UnityEngine;

public class CellClick : MonoBehaviour
{
    public event System.Action<int, int> OnCellClicked;  // Событие. Подписчики будут получать координаты и номер зоны.

    // Эти переменные будут заполнены из GridManager при создании клетки
    private int cellX;
    private int cellY;

    private SpriteRenderer spriteRenderer;
    private Color originalColor; // для восстановления исходного цвета

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void Initialize(int x, int y)
    {
        cellX = x;
        cellY = y;
    }

    // Метод вызова ивента, который будут вызывать из Raycast
    public void HandleClick()
    {
        OnCellClicked?.Invoke(cellX, cellY);
    }
     
    /*private void OnMouseDown() --Устарел
    {Встроенный метод Unity, вызывается при клике мышкой по коллайдеру 
        //Debug.Log($"Клик по клетке ({cellX}, {cellY}) — зона {zone}");
        
        // Вызов события, если на него кто-то подписан.
        OnCellClicked?.Invoke(cellX, cellY);
    }*/
}