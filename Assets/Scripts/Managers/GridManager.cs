using System.Collections.Generic;
using UnityEngine;

// Этот класс будет висеть на пустом объекте в сцене и управлять сеткой
public class GridManager : MonoBehaviour
{
    [Header("Настройки зон")]
    public GameObject zonePrefab; // префаб ZoneCell
    private List<ZoneCell> zoneCells = new List<ZoneCell>(); // список созданных зон

    [Header("Настройки сетки")]
    public int width = 9; // Количество клеток по ширине
    public int height = 9; // Количество клеток по высоте
    public float cellSize = 1.0f; // Размер клетки в мировых единицах

    [Header("Префабы")]
    public GameObject cellPrefab; // Префаб клетки
    public GameObject barrierPrefab; // Префаб барьера (способность синего круга)

    [Header("Префабы кругов")]
    public List<CirclePrefabMapping> prefabMappings; // для настройки в инспекторе (связь между двумя наборами данных)

     private Dictionary<Vector2Int, Circle> placedCircles = new Dictionary<Vector2Int, Circle>(); // координаты и круг
    private Dictionary<CircleType, GameObject> prefabsByType = new Dictionary<CircleType, GameObject>(); // Тип и Префаб круга
    private Dictionary<Vector2Int, Barrier> barriers = new Dictionary<Vector2Int, Barrier>(); // координата и барьер
    private Dictionary<Vector2Int, Color> originalCellColors = new Dictionary<Vector2Int, Color>(); // оригинальый цвет клеток

    [System.Serializable]
    public class CirclePrefabMapping
    {
        public CircleType type;
        public GameObject prefab;
    }


    private GameManager gameManager; 
    private TurnManager turnManager;   
    private GameObject[,] cellObjects; // двумерный массив для хранения клеток

    private int currentTurn = 0; // глобальный счётчик ходов
    public int CurrentTurn => currentTurn;
   

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        turnManager = FindAnyObjectByType<TurnManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager не найден в сцене!");
            return;
        }
        if (turnManager == null)
        {
            Debug.LogError("TurnManager не найден в сцене!");
            return;
        }
        
        foreach (var mapping in prefabMappings) // Заполнить словарь связкой тип-префаб круга с инспектора
        {
            if (mapping.prefab != null && !prefabsByType.ContainsKey(mapping.type))
            {
                prefabsByType.Add(mapping.type, mapping.prefab);
            }
            else
            {
                Debug.LogWarning($"Проблема с маппингом префаба для типа {mapping.type}");
            }
        }

        GenerateGrid(); // Как только игра запустится, создается сетка
        DisableCellColliders();  // Отключаем коллайдеры клеток для фазы ZoneSelection
        GenerateZones();
        Debug.Log($"Игра началась. Ходит игрок {turnManager.CurrentPlayer}");
    }

    void GenerateGrid()
    {
        if (cellPrefab == null) // Проверка префаба клетки
        {
            Debug.LogError("Ошибка: Префаб клетки не назначен в GridManager!");
            return;
        }

        cellObjects = new GameObject[width, height]; // Инициализируем массив нужного размера

        float centerX = (width - 1) / 2f;
        float centerY = (height - 1) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Центр поля в точке (0,0).
                // Поэтому отнимаем половину ширины и высоты.
                float posX = (x - centerX) * cellSize;
                float posY = (y - centerY) * cellSize;

                Vector3 cellPosition = new Vector3(posX, posY, 0);  // Позиция для текущей клетки.

                // Создание клетки из префаба в вычисленной позиции
                GameObject newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity); // Quaternion.identity - "без поворота"

                // Имя для клетке, чтобы не путаться в иерархии
                newCell.name = $"Cell_{x}_{y}";

                // Клетка дочерний объект этого GridManager
                newCell.transform.parent = this.transform;

                // Сохраняем клетку в массив
                cellObjects[x, y] = newCell;

                CellClick clickHandler = newCell.GetComponent<CellClick>();

                if (clickHandler == null)
                    Debug.LogError("Ошибка: Компонент CellClick нет на префабе");

                clickHandler.Initialize(x, y);
                clickHandler.OnCellClicked += gameManager.HandleCellClick; //подписка на событие
            }
        }
    }

    public bool PlaceCircle(int x, int y, int player, CircleType type)
    {
        if (IsCellOccupied(x, y) && HasBarrierAt(x, y)) //Клетка свободна?
        {
            Debug.LogWarning($"Клетка ({x}, {y}) уже занята! Нельзя поставить круг.");
            return false;
        }

        GameObject cell = GetCellObject(x, y); // Таргетная клетка
        
        if (cell == null)
        {
            Debug.LogError($"Ошибка: клетка ({x}, {y}) не найдена!");
            return false;
        }
        if (!prefabsByType.TryGetValue(type, out GameObject prefabe)) // Получаю префаб конкретного круга через параметр type
        {
            Debug.LogError("Нет префаба для типа круга {type}");
            return false;
        }

        // Создаю круг как дочерний объект клетки
        GameObject circleObj = Instantiate(prefabe, cell.transform);

        // Локальные координаты (0,0,0))
        circleObj.transform.localPosition = Vector3.zero;
    
        // Имя в инспекторе
        circleObj.name = $"{type}Circle_{x}_{y}_P{player}";

        // Получаю компонент Circle 
        Circle сircle = circleObj.GetComponent<Circle>();
        if (сircle == null)
        {
            Debug.LogError($"На префабе для типа {type} нет компонента {type}Circle!");
            Destroy(circleObj);
            return false;
        }
        
        // Инициализация круга
        сircle.Initialize(x, y, player, this, gameManager);

        // Внесение круга в словарь 
        Vector2Int pos = new Vector2Int(x, y);
        placedCircles[pos] = сircle;

        if (сircle.Type != CircleType.Core)
        {
            Debug.Log($"{type}Circle игрока {player} создан на клетке ({x}, {y})");   
        }

        return true;
    }

    public bool PlaceBarrier(int x, int y, int player, int turn)
    {
        // Проверка, свободна ли клетка
        if (IsCellOccupied(x, y))
        {
            Debug.LogWarning($"Клетка ({x}, {y}) уже занята кругом!");
            return false;
        }

        // Проверка, нет ли уже барьера
        if (barriers.ContainsKey(new Vector2Int(x, y)))
        {
            Debug.LogWarning($"Клетка ({x}, {y}) уже занята барьером!");
            return false;
        }

        GameObject cell = GetCellObject(x, y);
        if (cell == null) return false;

        if (barrierPrefab == null)
        {
            Debug.LogError("Нет префаба барьера!");
            return false;
        }

        GameObject barrierObj = Instantiate(barrierPrefab, cell.transform);
        barrierObj.transform.localPosition = Vector3.zero;
        barrierObj.name = $"Barrier_{x}_{y}_P{player}";

        Barrier barrier = barrierObj.GetComponent<Barrier>();
        if (barrier == null)
            Debug.LogError($"На префабе для барьера нет компонента!");

        barrier.Initialize(x, y, player, turn);

        barriers[new Vector2Int(x, y)] = barrier;

        Debug.Log($"Барьер игрока {player} установлен на ({x}, {y})");
        return true;
    }

    // Метод для проверки наличия барьера
    public bool HasBarrierAt(int x, int y)
    {
        return barriers.ContainsKey(new Vector2Int(x, y));
    }

    // Метод для доступа к клетке по координатам
    public GameObject GetCellObject(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return cellObjects[x, y];
        }

        return null;
    }

    public int GetCellZone(int x, int y)
    {
        // Для сетки 9x9 делится на зоны 3x3
        // Зоны нумеруются от 1 до 9:
        // 1 2 3
        // 4 5 6
        // 7 8 9

        int zoneX = x / 3; // 0, 1, 2
        int zoneY = y / 3; // 0, 1, 2

        return zoneY * 3 + zoneX + 1; // +1 чтобы зоны были 1-9
    }

    // public Vector2Int GetZoneCenter(int zone)
    // {
    //     // zone от 1 до 9
    //     int zoneIndex = zone - 1; // 0-8
    //     int zoneX = zoneIndex % 3; // 0,1,2
    //     int zoneY = zoneIndex / 3; // 0,1,2

    //     // Центр зоны 3x3: это клетка с координатами (zoneX*3 + 1, zoneY*3 + 1)
    //     // Например, для зоны 1 (zoneX=0, zoneY=0) центр = (1,1)
    //     // Для зоны 5 (zoneX=1, zoneY=1) центр = (4,4)
    //     int centerX = zoneX * 3 + 1;
    //     int centerY = zoneY * 3 + 1;

    //     return new Vector2Int(centerX, centerY);
    // }

    void GenerateZones()
    {
        if (zonePrefab == null)
        {
            Debug.LogError("Ошибка: zonePrefab не назначен в GridManager!");
            return;
        }

        // Очищаем предыдущие зоны, если есть
        foreach (var zone in zoneCells)
        {
            if (zone != null)
                Destroy(zone.gameObject);
        }
        zoneCells.Clear();

        // Создаём 9 зон (3 ряда по 3 зоны)
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                // Номер зоны (1-9)
                int zoneNumber = row * 3 + col + 1;

                // Вычисляем центр зоны в мировых координатах
                // Каждая зона покрывает 3x3 клетки

                // Координаты центральной клетки зоны
                int centerCellX = col * 3 + 1;
                int centerCellY = row * 3 + 1;

                // Получаем позицию этой клетки через GetCellObject
                GameObject centerCell = GetCellObject(centerCellX, centerCellY);
                if (centerCell == null)
                {
                    Debug.LogError($"Не удалось найти центральную клетку для зоны {zoneNumber}");
                    continue;
                }

                // Создаём зону как дочерний объект GridManager
                GameObject zoneObj = Instantiate(zonePrefab, transform);

                // Размещаем в центре зоны (там же, где центральная клетка)
                zoneObj.transform.position = centerCell.transform.position;

                // Масштабируем до размера 3x3 (если спрайт изначально 1x1)
                //zoneObj.transform.localScale = new Vector3(3f, 3f, 1f);

                // Инициализируем компонент ZoneCell
                ZoneCell zoneCell = zoneObj.GetComponent<ZoneCell>();
                if (zoneCell == null)
                {
                   Debug.LogError("Отсутсвует компонент ZoneCell в префабе зоны");
                }

                zoneCell.Initialize(zoneNumber, centerCellX, centerCellY);

                // Подписываемся на событие клика
                zoneCell.OnZoneClicked += HandleZoneClick;

                // Сохраняем в список
                zoneCells.Add(zoneCell);

                zoneObj.name = $"Zone_{zoneNumber}";
            }
        }
    }

    private void HandleZoneClick(ZoneCell zone)
    {
        // Передаём информацию в GameManager
        gameManager.HandleZoneSelection(zone.ZoneNumber, zone.CenterX, zone.CenterY);
        
    }

    public void RemoveZone(int zoneNumber)
    {
        // Ищем зону по номеру
        ZoneCell zoneToRemove = zoneCells.Find(z => z.ZoneNumber == zoneNumber);

        if (zoneToRemove != null)
        {
            zoneCells.Remove(zoneToRemove);
            Destroy(zoneToRemove.gameObject);
            Debug.Log($"Зона {zoneNumber} удалена");
        }
    }

    // Метод для удаления всех зон (при переходе в MainGame)
    public void RemoveAllZones()
    {
        foreach (var zone in zoneCells)
        {
            if (zone != null)
                Destroy(zone.gameObject);
        }

        zoneCells.Clear();
        Debug.Log("Все зоны удалены");
    }

    public void DisableCellColliders()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cell = GetCellObject(x, y);

                if (cell != null)
                {
                    Collider2D collider = cell.GetComponent<Collider2D>();

                    if (collider != null)
                    {
                        collider.enabled = false;
                    }
                }
            }
        }
    }

    public void EnableCellColliders()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cell = GetCellObject(x, y);
                if (cell != null)
                {
                    Collider2D collider = cell.GetComponent<Collider2D>();
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }
                }
            }
        }
    }


    public bool IsCellOccupied(int x, int y)
    {
        return placedCircles.ContainsKey(new Vector2Int(x, y));
    }

    public Circle GetCircleAt(int x, int y)
    {
        Vector2Int position = new Vector2Int(x, y);

        //есть ли другой круг на соседней клетке и какой он?
        placedCircles.TryGetValue(position, out Circle circle);
        return circle; // если нет круга, вернёт null
    }

    public void MoveCircle(int oldX, int oldY, int newX, int newY, Circle circle)
    {
        Vector2Int oldPos = new Vector2Int(oldX, oldY);
        Vector2Int newPos = new Vector2Int(newX, newY);
        
        if (placedCircles.ContainsKey(oldPos))
        {
            placedCircles.Remove(oldPos);
            placedCircles[newPos] = circle;
        }
        else
        {
            Debug.LogError($"Попытка переместить круг, но его нет в словаре на позиции ({oldX}, {oldY})");
        }
    }

    public void IncrementTurn()
    {
        currentTurn++;
    }

    public void RemoveBarriersForPlayer(int player, int currentTurn)
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var kvp in barriers)  //KeyValuePair 
        {
            Barrier barrier = kvp.Value;
            // Если барьер принадлежит игроку И был поставлен не в этот ход (т.е. в предыдущий)
            if (barrier.OwnerPlayer == player && barrier.TurnPlaced < currentTurn)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var pos in toRemove)
        {
            Destroy(barriers[pos].gameObject);
            barriers.Remove(pos);
            Debug.Log($"Барьер игрока {player} удалён на клетке ({pos.x}, {pos.y})");
        }
    }

    public void HighlightCells(List<Vector2Int> cells, Color highlightColor)
    {
        // Сначала сбрасываем предыдущую подсветку
        ClearHighlights();

        foreach (Vector2Int pos in cells)
        {
            GameObject cell = GetCellObject(pos.x, pos.y);

            if (cell != null)
            {
                SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();

                if (sr != null)
                {
                    // Сохраняем оригинальный цвет, если ещё не сохраняли
                    if (!originalCellColors.ContainsKey(pos))
                    {
                        originalCellColors[pos] = sr.color;
                    }
                    // Устанавливаем цвет подсветки
                    sr.color = highlightColor;
                }
            }
        }
    }

    public void ClearHighlights()
    {
        foreach (var kvp in originalCellColors)
        {
            GameObject cell = GetCellObject(kvp.Key.x, kvp.Key.y);

            if (cell != null)
            {
                SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = kvp.Value; // возвращаем оригинальный цвет
                }
            }
        }
        originalCellColors.Clear();
    }

    public void SetGlowForPlayer(int player, bool enabled)
    {
        foreach (var kvp in placedCircles)
        {
            Circle circle = kvp.Value;
            if (circle.Player == player)
            {
                circle.SetGlow(enabled);
            }
        }
    }

    // Если нужно сбросить подсветку у всех
    /*public void ResetAllGlow()
    {
        foreach (var kvp in placedCircles)
        {
            kvp.Value.SetGlow(false);
        }
    }*/
}