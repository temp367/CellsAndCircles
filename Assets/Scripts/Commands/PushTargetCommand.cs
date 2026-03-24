using UnityEngine;

public class PushTargetCommand : Command
{
    public Circle Activator{ get; private set; }
    public int OldX { get; private set; }
    public int OldY { get; private set; }
    public int NewX { get; private set; }
    public int NewY { get; private set; }
    
    public PushTargetCommand(Circle pusher, int xOld, int yOld, bool isEtherCommand) : base(pusher.Player, isEtherCommand)
    {
        this.Activator = pusher;
        
        OldX = xOld;
        OldY = yOld;
        
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
            Circle circlePushed = GameServices.Grid.GetCircleAt(OldX, OldY);

            // если клетка назначения вне поля
            if (GameServices.Grid.GetCellObject(NewX, NewY) == null)
            {
                // если это Core — конец игры
                if (circlePushed.Type == CircleType.Core)
                {
                    // Убираем Core с поля
                    //GameServices.Grid.RemoveCircle(circlePushed.GridX, circlePushed.GridY);
                    GameObject.Destroy(circlePushed.gameObject);
    
                    GameServices.Game.EndGame(Activator.Player);
    
                    executed = true;
                    return executed;
                }
            }

            executed = GameServices.Grid.MoveCircle(OldX, OldY, NewX, NewY, circlePushed);      
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
        return $"Толкнуть круг с ({OldX},{OldY}) на ({NewX},{NewY})";
    }
}