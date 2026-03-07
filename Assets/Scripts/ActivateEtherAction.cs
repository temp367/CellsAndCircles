using UnityEngine;

// Действие: активировать круг на клетке-триггере с целью
public class ActivateEtherAction : EtherAction
{
    private int triggerX;
    private int triggerY;
    private int targetX;
    private int targetY;
    private CircleType circleType;
    
    public ActivateEtherAction(int ownerPlayer, int triggerX, int triggerY, int targetX, int targetY, CircleType type) 
        : base(ownerPlayer)
    {
        this.triggerX = triggerX;
        this.triggerY = triggerY;
        this.targetX = targetX;
        this.targetY = targetY;
        this.circleType = type;
    }
    
    public override bool CanExecute(GridManager gridManager, TurnManager turnManager)
    {
        // Проверяем, что на клетке-триггере есть круг нужного типа и принадлежащий владельцу
        Circle triggerCircle = gridManager.GetCircleAt(triggerX, triggerY);
        if (triggerCircle == null)
        {
            Debug.Log($"ActivateEtherAction: нет круга на клетке-триггере ({triggerX}, {triggerY})");
            return false;
        }
        
        if (triggerCircle.Player != OwnerPlayer)
        {
            Debug.Log($"ActivateEtherAction: круг на триггере принадлежит не владельцу");
            return false;
        }
        
        if (triggerCircle.Type != circleType)
        {
            Debug.Log($"ActivateEtherAction: тип круга на триггере ({triggerCircle.Type}) не совпадает с ожидаемым ({circleType})");
            return false;
        }
        
        // Здесь можно добавить проверки для цели в зависимости от типа
        switch (circleType)
        {
            case CircleType.Red:
                // Проверяем, что цель - вражеский круг (не Core)
                Circle targetCircle = gridManager.GetCircleAt(targetX, targetY);
                if (targetCircle == null || targetCircle.Player == OwnerPlayer || targetCircle.Type == CircleType.Core)
                {
                    Debug.Log($"ActivateEtherAction: цель для красного круга некорректна");
                    return false;
                }
                break;
                
            case CircleType.Blue:
                // Проверяем, что цель - пустая клетка для барьера
                if (gridManager.GetCircleAt(targetX, targetY) != null || gridManager.HasBarrierAt(targetX, targetY))
                {
                    Debug.Log($"ActivateEtherAction: цель для синего круга должна быть пустой");
                    return false;
                }
                break;
                
            case CircleType.Green:
                // Проверяем, что цель - пустая клетка для размножения
                if (gridManager.GetCircleAt(targetX, targetY) != null || gridManager.HasBarrierAt(targetX, targetY))
                {
                    Debug.Log($"ActivateEtherAction: цель для зелёного круга должна быть пустой");
                    return false;
                }
                break;
        }
        
        return true;
    }
    
    public override void Execute(GridManager gridManager)
    {
        Circle triggerCircle = gridManager.GetCircleAt(triggerX, triggerY);
        
        if (triggerCircle != null)
        {
            // Активируем круг в зависимости от типа
            switch (circleType)
            {
                case CircleType.Red:
                    // Для красного: толкаем цель
                    if (triggerCircle is RedCircle redCircle)
                    {
                        Circle targetCircle = gridManager.GetCircleAt(targetX, targetY);
                        if (targetCircle != null)
                        {
                            redCircle.PushTarget(targetCircle);
                            Debug.Log($"ActivateEtherAction: красный круг толкнул цель");
                        }
                    }
                    break;
                    
                case CircleType.Blue:
                    // Для синего: ставим барьер
                    bool success = gridManager.PlaceBarrier(targetX, targetY, OwnerPlayer, gridManager.CurrentTurn);
                    if (success)
                    {
                        Debug.Log($"ActivateEtherAction: синий круг создал барьер");
                    }
                    break;
                    
                case CircleType.Green:
                    // Для зелёного: размножаемся
                    if (triggerCircle is GreenCircle greenCircle)
                    {
                        greenCircle.ReproduceAt(targetX, targetY);
                        Debug.Log($"ActivateEtherAction: зелёный круг размножился");
                    }
                    break;
            }
        }
    }
    
    public override string GetDescription()
    {
        return $"Активировать {circleType} круг на ({triggerX},{triggerY}) с целью ({targetX},{targetY})";
    }
}