using UnityEngine.Events;
using System.Collections.Generic;
using Snake3D.Audio;
using Snake3D.Grids;
using Snake3D.Managers;
using UnityEngine;

namespace Snake3D.Gameplay
{
    public class Snake : CellBehaviour
    {
        public Vector2Int Direction;
        List<GridCell> SnakeParts;
        bool Turn;

        public override void BehavoiourStart(GridCell Parent)
        {
            base.BehavoiourStart(Parent);
            SnakeParts = new List<GridCell>();
            SnakeParts.Add(ParentCell);
        }

        public void MoveSnake()
        {
            if(!Enabled) { return; }

            Vector2Int NextPosition = ParentCell.GridPosition + Direction;
            if(!GridManager.Instance.IsValidCoord(NextPosition) && !GameManager.Instance.IsDead) { GameManager.Instance.Die(); return; }
            CheckCrash(NextPosition);
        }
        void CheckCrash(Vector2Int NextPos)
        {
            Vector2Int Dir = NextPos - ParentCell.GridPosition;
            if(!BodyPartNearby(Dir)) { MoveBody(); }
            else { GameManager.Instance.Die(); }
        }

        void MoveBody()
        {
            ParentCell.MoveCell(Direction);
            for (int Sp = 1; Sp < SnakeParts.Count; Sp++)
            {
                SnakeParts[Sp].SetCellPosition(SnakeParts[Sp - 1].PrevGridPosition);
            }
        }

        bool BodyPartNearby(Vector2Int Dir)
        {
            Vector2Int NextPos = ParentCell.GridPosition + Dir;
            bool HasCellAtNextPos = GridManager.Instance.CellExistsAtCoord(NextPos);
            if(HasCellAtNextPos)
            {
                GridCell NearbyCell = GridManager.Instance.GetGridCellFromCoord(NextPos);
                bool NearCellIsSnakePart = NearbyCell.Tag == "SnakeBody";
                return NearCellIsSnakePart;
            }

            return false;
        }

        public void TrySetDirection(Vector2Int NewDir)
        {
            Vector2Int FixedBackDir = ParentCell.PrevGridPosition - ParentCell.GridPosition;
            if(SnakeParts.Count > 1 && FixedBackDir == NewDir) { return; }

            bool InalidBackDir = (SnakeParts.Count > 1) && NewDir == -Direction;
            if(NewDir != Direction && !InalidBackDir)
            {
                Direction = NewDir;

                int TurnIndex = (Turn) ? 2 : 1;
                AudioManager.Instance.InteractWithSFX("Turn " + TurnIndex, SoundEffectBehaviour.Play);
                Turn = !Turn;
            }
        }

        public void AddBodyPart()
        {
            GridCell NewBodyPart = GridManager.Instance.AddCell("SnakeBod_" + SnakeParts.Count, "SnakeBody", ProgressManager.Instance.GetSelectedSkin().BodyColor, SnakeParts[SnakeParts.Count - 1].GridPosition);
            SnakeParts.Add(NewBodyPart);
        }

        public void Kill()
        {
            foreach (GridCell C in SnakeParts)
            {
                ColorManager.Instance.ColorObject(C.OBJ, Color.red);
            }

            GameManager.Instance.StartCoroutine(FXManager.Instance.FlashObject(ParentCell.OBJ, 0.175f, 11, null, new UnityAction( delegate 
            {
                GridManager.Instance.RemoveAllCells();
                GameManager.Instance.SetMiniMenu();
            } )));
            
            for (int Sp = 1; Sp < SnakeParts.Count; Sp++)
            {
                GameManager.Instance.StartCoroutine(FXManager.Instance.FlashObject(SnakeParts[Sp].OBJ, 0.175f, 11, null, null));
            }
        }
    }
}