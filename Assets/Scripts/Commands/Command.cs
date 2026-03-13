public abstract class Command 
{
    public int OwnerPlayer { get; protected set; }
    protected bool executed = false;
    
    public Command(int ownerPlayer)
    {
        OwnerPlayer = ownerPlayer;
    }
    
    public abstract bool Execute();
    public abstract void Undo();
    public abstract string GetDescription();
}