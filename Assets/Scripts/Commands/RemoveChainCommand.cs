using System.Collections.Generic;
using UnityEngine;

public class RemoveChainCommand : Command
{
    public Circle Activator{ get; private set; }
    public Circle targetCircle; // первый круг в цепочке
    private List<Circle> removedCircles = new List<Circle>();
    private Vector2Int direction;
    
    public RemoveChainCommand(Circle activator, Circle target, bool isEtherCommand) : base(activator.Player, isEtherCommand)
    {
        Activator = activator;
        this.targetCircle = target;
        
        // Вычисляем направление от активатора к цели
        int dirX = target.GridX - activator.GridX;
        int dirY = target.GridY - activator.GridY;
        direction = new Vector2Int(dirX, dirY);
    }
    
    public override bool Execute()
    {
        if (!executed)
        {
            CircleType targetType = targetCircle.Type;
            int currentX = targetCircle.GridX;
            int currentY = targetCircle.GridY;
            
            // Идём по направлению, пока не встретим препятствие
            while (true)
            {
                Circle circle = GameServices.Grid.GetCircleAt(currentX, currentY);
                
                // Если нет круга или другой тип - останавливаемся
                if (circle == null || circle.Type != targetType)
                    break;
                
                // Сохраняем 
                removedCircles.Add(circle);
                
                // Удаляем круг
                GameServices.Grid.RemoveCircle(circle);
                
                // Двигаемся дальше
                currentX += direction.x;
                currentY += direction.y;
                
                // Проверка границ сетки
                if (GameServices.Grid.GetCellObject(currentX, currentY) == null)
                    break;
            }
            
            executed = true;
            return executed;
        }

        return executed;
    }
    
    public override void Undo()
    {
    }
    
    public override string GetDescription()
    {
        return $"Удалить цепочку кругов типа {targetCircle.Type}";
    }
}