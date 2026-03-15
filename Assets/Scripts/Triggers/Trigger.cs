using System;

public enum TriggerKind
{
    EnemyPlaceCircle,
    SelfPlaceCircle,
    enemyActivate
}

public abstract class Trigger
{
    public int OwnerPlayer { get; protected set; }

    public bool IsActive { get; protected set; } = true;

    public event Action<Trigger> OnTriggered;

    protected Trigger(int ownerPlayer)
    {
        OwnerPlayer = ownerPlayer;
    }

    // Проверка команды
    public bool TryCheck(Command command)
    {
        if (!IsActive)
            return false;

        if (!Check(command))
            return false;

        Activate();
        return true;
    }

    // Логика проверки (реализуют наследники)
    protected abstract bool Check(Command command);

    protected void Activate()
    {
        IsActive = false;
        OnTriggered?.Invoke(this);
    }
}