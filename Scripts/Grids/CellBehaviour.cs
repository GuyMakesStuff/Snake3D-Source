using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake3D.Managers;

namespace Snake3D.Grids
{
    public class CellBehaviour
    {
        public bool Enabled;
        public GridCell ParentCell;

        public virtual void BehavoiourStart(GridCell Parent)
        {
            Enabled = true;
            ParentCell = Parent;
        }
        public virtual void BehavoiourUpdate()
        {
            if(!Enabled) { return; }

            CheckCollisions();
        }
        protected virtual void OnCellCollide(GridCell Other)
        {

        }

        public void CheckCollisions()
        {
            if(GridManager.Instance.CellExistsAtCoord(ParentCell.GridPosition))
            {
                OnCellCollide(GridManager.Instance.GetGridCellFromCoord((ParentCell.GridPosition)));
            }
        }

        public void DestroyCell()
        {
            Enabled = false;
            ParentCell.Destroy();
        }
    }
}