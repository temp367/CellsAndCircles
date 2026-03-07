using UnityEngine;

// Интерфейс для условий срабатывания эфирного действия
public interface IEtherTrigger
{
    // Проверяет, выполнено ли условие
    bool Check(GridManager gridManager, TurnManager turnManager, EtherAction action);
    
    // Возвращает описание условия
    string GetDescription();
}

// Условие: враг толкнул мой синий круг
public class EnemyPushedMyBlueCircleTrigger : IEtherTrigger
{
    private int triggerX;
    private int triggerY;
    private int ownerPlayer;
    private bool conditionMet = false;
    private Circle targetCircle;
    
    public EnemyPushedMyBlueCircleTrigger(int ownerPlayer, int x, int y)
    {
        this.ownerPlayer = ownerPlayer;
        this.triggerX = x;
        this.triggerY = y;
    }
    
    public bool Check(GridManager gridManager, TurnManager turnManager, EtherAction action)
    {
        // Если условие уже выполнено, возвращаем true
        if (conditionMet)
            return true;
        
        // Находим круг на клетке-триггере
        if (targetCircle == null)
        {
            targetCircle = gridManager.GetCircleAt(triggerX, triggerY);
            
            // Если круг есть и это синий круг владельца - подписываемся
            if (targetCircle != null && 
                targetCircle.Player == ownerPlayer && 
                targetCircle.Type == CircleType.Blue)
            {
                // Подписываемся на событие
                targetCircle.OnPushed += OnTargetPushed;
                Debug.Log($"Trigger: подписались на событие толкания синего круга");
            }
        }
        
        return conditionMet;
    }
    
    private void OnTargetPushed(Circle pushedCircle)
    {
        Debug.Log($"Trigger: синий круг на ({triggerX}, {triggerY}) толкнули!");
        conditionMet = true;
        
        // Отписываемся от события (чтобы не было утечек памяти)
        if (targetCircle != null)
        {
            targetCircle.OnPushed -= OnTargetPushed;
        }
    }
    
    public string GetDescription()
    {
        return $"Враг толкнул синий круг на ({triggerX}, {triggerY})";
    }
}

// Условие: начался мой ход (срабатывает сразу в начале хода)
public class MyTurnStartedTrigger : IEtherTrigger
{
    private int ownerPlayer;
    
    public MyTurnStartedTrigger(int ownerPlayer)
    {
        this.ownerPlayer = ownerPlayer;
    }
    
    public bool Check(GridManager gridManager, TurnManager turnManager, EtherAction action)
    {
        return turnManager.CurrentPlayer == ownerPlayer;
    }
    
    public string GetDescription()
    {
        return $"Начало хода игрока {ownerPlayer}";
    }
}

// Условие: противник поставил круг на определённую клетку
public class EnemyPlacedCircleOnCellTrigger : IEtherTrigger
{
    private int targetX;
    private int targetY;
    private int ownerPlayer;
    
    public EnemyPlacedCircleOnCellTrigger(int ownerPlayer, int x, int y)
    {
        this.ownerPlayer = ownerPlayer;
        this.targetX = x;
        this.targetY = y;
    }
    
    public bool Check(GridManager gridManager, TurnManager turnManager, EtherAction action)
    {
        // Проверяем, что на клетке появился круг противника
        Circle circle = gridManager.GetCircleAt(targetX, targetY);
        return circle != null && circle.Player != ownerPlayer;
    }
    
    public string GetDescription()
    {
        return $"Противник поставил круг на ({targetX}, {targetY})";
    }
}