using System.Collections;
using System.Collections.Generic;
using Snake3D.Managers;
using UnityEngine;

namespace Snake3D.Grids
{
    public class GridCell
    {
        public string CellName;
        public string Tag;
        public Vector2Int GridPosition;
        public Vector2Int PrevGridPosition;
        public GameObject OBJ;
        Color OriginalColor;
        List<CellBehaviour> Behaviours;
        bool FirstUpdate;

        public GridCell(string Name, string tag, Vector2Int Pos, Color CellColor)
        {
            // Apply Name And Tag
            CellName = Name;
            Tag = tag;
            // Apply Position
            GridPosition = Pos;
            SetCellPosition(GridPosition);

            // Initialize Behaviours List
            Behaviours = new List<CellBehaviour>();

            // Create Cell Object
            OBJ = GameObject.CreatePrimitive(PrimitiveType.Cube);
            OBJ.SetActive(false);
            OBJ.name = CellName;
            // Parent Cell Object To Cells Container
            OBJ.transform.SetParent(GridManager.Instance.CellsParent);
            // Color Cell Object
            ColorManager.Instance.ColorObject(OBJ, CellColor);
            OriginalColor = CellColor;

            // Set To First Update
            FirstUpdate = true;
        }

        public void UpdateCell()
        {
            if(FirstUpdate) { FirstUpdate = false; OBJ.SetActive(true); }

            // Set Object Position
            Vector3 OBJPos = GridManager.Instance.GridCoordsToWorldPos(GridPosition);
            OBJ.transform.position = OBJPos;

            // Update Cell Behaviours
            foreach (CellBehaviour CB in Behaviours)
            {
                CB.BehavoiourUpdate();
            }
        }

        public void MoveCell(Vector2Int Amount)
        {
            SetCellPosition(GridPosition + Amount);
        }
        public void SetCellPosition(Vector2Int NewPosition)
        {
            PrevGridPosition.x = GridPosition.x;
            PrevGridPosition.y = GridPosition.y;
            GridPosition = NewPosition;
        }

        public CellBehaviour AddBehaviour(CellBehaviour behaviour)
        {
            behaviour.BehavoiourStart(this);
            if(Behaviours.Contains(behaviour)) { return null; }
            Behaviours.Add(behaviour);
            return behaviour;
        }

        public void Destroy()
        {
            MonoBehaviour.Destroy(OBJ);
            GridManager.Instance.RemoveCell(this);
        }

        public IEnumerator FlashColor(Color FlashCol, float FlashIntervals, int FlashCount)
        {
            int FlashTimes = 0;
            bool Flashed = false;
            while (FlashTimes < FlashCount)
            {
                if(OBJ == null) { yield break; }

                Flashed = !Flashed;
                ColorManager.Instance.ColorObject(OBJ, (Flashed) ? FlashCol : OriginalColor);
                yield return new WaitForSeconds(FlashIntervals);
                FlashTimes++;
            }

            Flashed = false;
            ColorManager.Instance.ColorObject(OBJ, OriginalColor);
        }
    }
}