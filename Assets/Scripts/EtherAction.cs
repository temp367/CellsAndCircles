using UnityEngine;

// Базовый класс для всех эфирных действий
public abstract class EtherAction
{
    public int OwnerPlayer { get; protected set; } // игрок, который создал эфир
    
    public EtherAction(int ownerPlayer)
    {
        OwnerPlayer = ownerPlayer;
    }
    
    // Проверка, может ли действие выполниться сейчас
    public abstract bool CanExecute(GridManager gridManager, TurnManager turnManager);
    
    // Выполнение действия
    public abstract void Execute(GridManager gridManager);
    
    // Возвращает описание действия (для отладки)
    public abstract string GetDescription();
}