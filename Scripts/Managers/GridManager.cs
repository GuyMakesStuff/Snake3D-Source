using Snake3D.Grids;
using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.Managers
{
    public class GridManager : Manager<GridManager>
    {
        [Header("General")]
        public int Dimentions;
        public Transform EnvironmentParent;
        public Transform BordersParent;
        public Transform CellsParent;
        List<GridCell> Cells;

        [Header("Visuals")]
        public Color GroundColor;
        public Color BorderColor;

        public void InitGrid()
        {
            // Initialize Manager Object
            Init(this);

            // Initialize Cells List
            Cells = new List<GridCell>(Dimentions * Dimentions);

            // Create Game Environment
            CreateEnvironment();

            // Test
            // AddCell("TestCell", "Test", Color.blue, new Vector2Int(5,4));
        }
        void CreateEnvironment()
        {
            // Create Ground Piece
            // Ground Piece Scale (Realetive To Dimentions)
            Vector3 GroundScale = (Vector3.right * (Dimentions + 1)) + (Vector3.forward * (Dimentions + 1)) + Vector3.up;
            // Create Ground Object
            GameObject Ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Ground.name = "Ground";
            // Parent Ground To Environment Object
            Ground.transform.SetParent(EnvironmentParent);
            Ground.transform.SetAsFirstSibling();
            // Apply Ground Position
            Ground.transform.position = Vector3.down;
            // Apply Ground Scale
            Ground.transform.localScale = GroundScale;
            // Color Ground Object
            ColorManager.Instance.ColorObject(Ground, GroundColor);

            // Create Borders
            // Border Directions
            Vector3[] Directions = new Vector3[]
            {
                Vector3.forward,
                Vector3.right,
                Vector3.back,
                Vector3.left
            };
            // Create A Border Object In Every Direction
            for (int B = 0; B < Directions.Length; B++)
            {
                // Border Position
                Vector3 BorderPos = Ground.transform.position + (Directions[B] * ((Dimentions / 2f) + 1f)) + (Vector3.up / 2f);
                // Border Rotation
                Quaternion BorderRot = Quaternion.LookRotation(Directions[B], Vector3.up);
                // Border Scale
                float BorderScaleRight = Dimentions + 3f;
                if(IsEvenNumber(B)) { BorderScaleRight = Dimentions + 1f; }
                Vector3 BorderScale = new Vector3(BorderScaleRight, 2f, 1f);
                // Create New Border Object
                GameObject Border = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Border.name = "Border_" + B;
                // Parent New Border To Borders Container Object
                Border.transform.SetParent(BordersParent);
                Border.transform.SetAsLastSibling();
                // Apply New Border Position
                Border.transform.position = BorderPos;
                // Apply New Border Rotation
                Border.transform.localRotation = BorderRot;
                // Apply New Border Scale
                Border.transform.localScale = BorderScale;
                // Color New Border Object
                ColorManager.Instance.ColorObject(Border, BorderColor);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Update Cells
            try
            {
                foreach (GridCell C in Cells)
                {
                    C.UpdateCell();
                }
            }
            catch (System.InvalidOperationException)
            {
                return;
            }
        }

        // Is An Integer Number Even?
        bool IsEvenNumber(int Num)
        {
            float FloatNum = (float)Num;
            return Num == 0 || FloatNum % 2 == 0;
        }

        // Convert Grid Positions To World Positions
        public Vector3 GridCoordsToWorldPos(Vector2Int Coords)
        {
            float HalfDimentions = ((float)Dimentions) / 2f;
            float X = Mathf.Clamp((float)Coords.x, -HalfDimentions, HalfDimentions);
            float Z = Mathf.Clamp((float)Coords.y, -HalfDimentions, HalfDimentions);
            return new Vector3(X, 0f, Z);
        }

        public bool IsValidCoord(Vector2Int Coord)
        {
            float HalfDimentions = ((float)Dimentions) / 2f;
            bool XInRange = (float)Coord.x <= HalfDimentions && (float)Coord.x >= -HalfDimentions;
            bool YInRange = (float)Coord.y <= HalfDimentions && (float)Coord.y >= -HalfDimentions;
            return XInRange && YInRange;
        }

        public GridCell AddCell(string CellName, string CellTag, Color CellCol, Vector2Int CellPos)
        {
            GridCell NewCell = new GridCell(CellName, CellTag, CellPos, CellCol);
            Cells.Add(NewCell);
            return NewCell;
        }
        public void RemoveCell(GridCell cell)
        {
            if(Cells.Contains(cell))
            {
                Cells.Remove(cell);
            }
        }
        public void RemoveAllCells()
        {
            foreach (GridCell C in Cells)
            {
                Destroy(C.OBJ);
            }
            Cells.Clear();
        }
        public GridCell GetGridCellFromCoord(Vector2Int Coord)
        {
            return Cells.Find(GridCell => GridCell.GridPosition == Coord);
        }
        public GridCell GetGridCellByName(string CellName)
        {
            return Cells.Find(GridCell => GridCell.CellName == CellName);
        }
        public bool CellExistsAtCoord(Vector2Int Coord)
        {
            return GetGridCellFromCoord(Coord) != null;
        }

        public Vector2Int RandomCoord(bool EmptyOnly)
        {
            int HalfDimentions = Dimentions / 2;
            int X = Random.Range(-HalfDimentions, HalfDimentions + 1);
            int Y = Random.Range(-HalfDimentions, HalfDimentions + 1);
            Vector2Int FinalCoord = new Vector2Int(X, Y);

            if(CellExistsAtCoord(FinalCoord) && EmptyOnly)
            {
                return RandomCoord(true);
            }

            return FinalCoord;
        }
    }
}