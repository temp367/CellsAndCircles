public abstract class Command 
{
    public int OwnerPlayer { get; protected set; }
    public bool IsEtherCommand { get; protected set; }
    protected bool executed = false;
    
    public Command(int ownerPlayer, bool isEtherCommand)
    {
        OwnerPlayer = ownerPlayer;
        IsEtherCommand = isEtherCommand;
    }
    
    public abstract bool Execute();
    public abstract void Undo();
    public abstract string GetDescription();
}