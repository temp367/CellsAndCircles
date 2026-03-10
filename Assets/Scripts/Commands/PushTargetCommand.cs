using UnityEngine;

public class PushTargetCommand : Command
{
    private Circle pusher;
    public Circle Target { get; private set; }
    private int oldX;
    private int oldY;
    private int newX;
    private int newY;
    private GridManager grid;
    private bool wasPushed = false;
    
    public PushTargetCommand(Circle pusher, Circle target, GridManager grid) : base(pusher.Player)
    {
        this.pusher = pusher;
        this.Target = target;
        this.grid = grid;
        
        oldX = target.GridX;
        oldY = target.GridY;
        
        // Вычисляем новую позицию
        int dx = target.GridX - pusher.GridX;
        int dy = target.GridY - pusher.GridY;
        newX = target.GridX + dx;
        newY = target.GridY + dy;
    }
    
    public override void Execute()
    {
        if (!wasPushed && pusher is RedCircle red)
        {
            // Проверяем, можно ли толкнуть
            if (newX >= 0 && newX < grid.width && newY >= 0 && newY < grid.height &&
                grid.GetCircleAt(newX, newY) == null && !grid.HasBarrierAt(newX, newY))
            {
                // Сохраняем в словаре перемещение
                grid.MoveCircle(oldX, oldY, newX, newY, Target);
                
                // Перемещаем GameObject
                GameObject newCell = grid.GetCellObject(newX, newY);
                Target.transform.SetParent(newCell.transform);
                Target.transform.localPosition = Vector3.zero;
                
                wasPushed = true;
                Debug.Log($"PushTargetCommand: цель перемещена на ({newX},{newY})");
            }
            else
            {
                Debug.Log($"PushTargetCommand: цель не может быть перемещена");
            }
        }
    }
    
    public override void Undo()
    {
        if (wasPushed)
        {
            // Возвращаем на старое место
            grid.MoveCircle(newX, newY, oldX, oldY, Target);
            
            GameObject oldCell = grid.GetCellObject(oldX, oldY);
            Target.transform.SetParent(oldCell.transform);
            Target.transform.localPosition = Vector3.zero;
            
            wasPushed = false;
            Debug.Log($"PushTargetCommand: отменено перемещение");
        }
    }
    
    public override string GetDescription()
    {
        return $"Толкнуть круг с ({oldX},{oldY}) на ({newX},{newY})";
    }
}