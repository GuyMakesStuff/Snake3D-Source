using Snake3D.Grids;
using System.Collections.Generic;
using UnityEngine;
using Snake3D.Managers;

namespace Snake3D.Visuals
{
    public class DemoSnake : CellBehaviour
    {
        List<GridCell> SnakeParts;

        public override void BehavoiourStart(GridCell Parent)
        {
            base.BehavoiourStart(Parent);
            SnakeParts = new List<GridCell>();
            SnakeParts.Add(ParentCell);

            for (int P = 0; P < 3; P++)
            {
                Vector2Int Pos = ParentCell.GridPosition + (Vector2Int.right * (P + 1));
                GridCell NewBod = GridManager.Instance.AddCell("DemoSnakeBod_" + P, "DemoSnakeBody", Color.white, Pos);
                SnakeParts.Add(NewBod);
            }

            UpdateSnake();
        }

        public void UpdateSnake()
        {
            SkinManager.Skin DisplaySkin = SkinManager.Instance.GetSkin(ShopManager.Instance.SkinSelection.Value);
            for (int B = 0; B < SnakeParts.Count; B++)
            {
                Color Col = (B == 0) ? DisplaySkin.HeadColor : DisplaySkin.BodyColor;
                ColorManager.Instance.ColorObject(SnakeParts[B].OBJ, Col);
            }
        }
    }
}