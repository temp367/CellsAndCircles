using UnityEngine;

public class PushTargetCommand : Command
{
    public Circle Activator{ get; private set; }
    private int oldX;
    private int oldY;
    public int NewX { get; private set; }
    public int NewY { get; private set; }
    
    public PushTargetCommand(Circle pusher, int xOld, int yOld, bool isEtherCommand) : base(pusher.Player, isEtherCommand)
    {
        this.Activator = pusher;
        
        oldX = xOld;
        oldY = yOld;
        
        // Вычисляем новую позицию
        int dx = xOld - pusher.GridX;
        int dy = yOld - pusher.GridY;
        NewX = xOld + dx;
        NewY = yOld + dy;
    }
    
    public override bool Execute()
    {
        if (!executed)
        {
            Circle circlePushed = GameServices.Grid.GetCircleAt(oldX, oldY);

            executed = GameServices.Grid.MoveCircle(oldX, oldY, NewX, NewY, circlePushed);      
        }

        return executed;
    }
    
    public override void Undo()
    {
            // Возвращаем на старое место
            // grid.MoveCircle(newX, newY, oldX, oldY, Target);
            
            // GameObject oldCell = grid.GetCellObject(oldX, oldY);
            // Target.transform.SetParent(oldCell.transform);
            // Target.transform.localPosition = Vector3.zero;
            
            // wasPushed = false;
            // Debug.Log($"PushTargetCommand: отменено перемещение");
    }
    
    
    public override string GetDescription()
    {
        return $"Толкнуть круг с ({oldX},{oldY}) на ({NewX},{NewY})";
    }
}