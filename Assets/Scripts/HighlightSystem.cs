using System.Collections.Generic;
using UnityEngine;

public class HighlightSystem : MonoBehaviour, IInitializable
{
    public InitStage InitStage => InitStage.Visual;

    private List<GameObject> activeHighlights; 

    public void Initialize()
    {
        activeHighlights = new List<GameObject>();

        GameServices.Register(this);

        GameServices.Turn.OnPlayerChanged += (player) =>
        {
            ShowEtherPreview(
                GameServices.Ether.GetTriggers(),
                GameServices.Ether.GetPendingCommands(),
                player
            );
        };

        Debug.Log("HighlightSystem initialized");
    }

    public void ShowCells(List<Vector2Int> cells, Color color)
    {
        Clear();

        foreach (var pos in cells)
        {
            GameObject cell = GameServices.Grid.GetCellObject(pos.x, pos.y);
            if (cell == null) continue;

            var renderer = cell.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = color;
                activeHighlights.Add(cell);
            }
        }
    }

    public void ShowEtherPreview(List<Trigger> triggers, Dictionary<Trigger, Command> commands, int currentPlayer)
    {
        Clear();

        foreach (var pair in commands)
        {
            Trigger trigger = pair.Key;
            Command command = pair.Value;

            if (command.OwnerPlayer != currentPlayer)
                continue;

            // Пример для PlaceCircleCommand
            if (command is PlaceCircleCommand place)
            {
                HighlightCell(place.X, place.Y, Color.magenta);
                HighlightCell(trigger.TargetCell.Value.x, trigger.TargetCell.Value.y, Color.darkMagenta);
            }
            else if (command is PlaceBarrierCommand bar)
            {
                HighlightCell(bar.X, bar.Y, Color.magenta);
                HighlightCell(trigger.TargetCell.Value.x, trigger.TargetCell.Value.y, Color.darkMagenta);
            }
            else if (command is PushTargetCommand pushTarget)
            {
                HighlightCell(pushTarget.OldX, pushTarget.OldY, Color.magenta);
                HighlightCell(pushTarget.NewX, pushTarget.NewY, Color.magenta);
                HighlightCell(trigger.TargetCell.Value.x, trigger.TargetCell.Value.y, Color.darkMagenta);
            }
            else if (command is ReproduceCommand repCom)
            {
                HighlightCell(repCom.X, repCom.Y, Color.magenta);
                HighlightCell(trigger.TargetCell.Value.x, trigger.TargetCell.Value.y, Color.darkMagenta);
            }
        }
    }

    public void HighlightCell(int x, int y, Color color)
    {
        GameObject cell = GameServices.Grid.GetCellObject(x, y);
        if (cell == null) return;

        var renderer = cell.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = color;
            activeHighlights.Add(cell);
        }
    }

    public void Clear()
    {
        foreach (var obj in activeHighlights)
        {
            if (obj == null) continue;

            var renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.white;
            }
        }

        activeHighlights.Clear();
    }
}