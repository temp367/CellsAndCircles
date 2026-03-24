using UnityEngine;

public class Barrier : MonoBehaviour
{
    public int GridX { get; set; }
    public int GridY { get; set; }
    public int OwnerPlayer { get; set; }
    public int ExpireTurn { get; set; } // номер хода, когда поставлен

    public void Initialize(int x, int y, int player, int expireOnPlayerTurn)
    {
        GridX = x;
        GridY = y;
        OwnerPlayer = player;
        ExpireTurn = expireOnPlayerTurn + 1;
    }
}