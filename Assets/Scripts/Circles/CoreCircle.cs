using UnityEngine;

public class CoreCircle : Circle
{
    public override CircleType Type => CircleType.Core;
    public override bool CanBePushed => true; // белое ядро нельзя толкать
    public override bool CanActivate => false; // у бедлого ядра нет никаких способностей
    
    public override void ApplyEffect()
    {
        // У белого ядра пока нет эффекта при установке
        //Debug.Log($"CoreCircle на клетке ({GridX}, {GridY}) не имеет эффекта.");
    }
}