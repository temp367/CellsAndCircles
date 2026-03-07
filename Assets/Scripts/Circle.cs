using UnityEngine;

public enum CircleType
{
    Core,
    Red,
    Blue,
    Green
}

public abstract class Circle : MonoBehaviour
{
    public abstract CircleType Type { get; }
    
    public int Player { get; protected set; }      // владелец (1 или 2)
    public int GridX { get; protected set; }        // координата X
    public int GridY { get; protected set; }        // координата Y
    
    protected GridManager gridManager;              // ссылка на GridManager
    protected GameManager gameManager;
    public GameObject glowObject;

    public virtual bool CanBePushed => true;        // Можно толкать этот круг?
    public virtual bool CanActivate => true;        // У этого круга активируются способности? 

    // Событие, которое вызывается, когда круг толкают
    public System.Action<Circle> OnPushed;

    public virtual void OnPushedBy(Circle pusher)
    {
        Debug.Log($"{GetType().Name} на ({GridX}, {GridY}) толкнули");
        OnPushed?.Invoke(this);
    }

    // Метод для инициализации (будет вызываться после создания)
    public void Initialize(int x, int y, int player, GridManager GRmanager, GameManager Gmmanager)
    {
        GridX = x;
        GridY = y;
        Player = player;
        gridManager = GRmanager;
        gameManager = Gmmanager;
    }

    public void UpdatePosition(int newX, int newY)
    {
        GridX = newX;
        GridY = newY;
    }


    public abstract void ApplyEffect();

    public virtual bool Activate()
    {
        return false;
    }

    public virtual void SetGlow(bool enabled)
    {
        if (glowObject == null)
        {
            Debug.LogError("glowObject отсутсвует префаб в круге.");
        }

        if (glowObject != null)
            glowObject.SetActive(enabled);
    }
}