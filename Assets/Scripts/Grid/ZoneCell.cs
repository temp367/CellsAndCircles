using UnityEngine;

public class ZoneCell : MonoBehaviour
{
    public int ZoneNumber { get; private set; } // номер зоны (1-9)
    public int CenterX { get; private set; } // координаты центра зоны
    public int CenterY { get; private set; }
    
    // Событие, которое будет вызываться при клике на зону
    public event System.Action<int, int, int> OnZoneClicked;
    
    public void Initialize(int zoneNumber, int centerX, int centerY)
    {
        ZoneNumber = zoneNumber;
        CenterX = centerX;
        CenterY = centerY;
    }

    
    // Этот метод будут вызывать из MouseInputHandler
    public void HandleClick()
    {
        Debug.Log($"Клик по зоне {ZoneNumber}");
        OnZoneClicked?.Invoke(ZoneNumber, CenterX, CenterY);
    }
}