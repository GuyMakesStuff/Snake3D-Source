using Snake3D.Audio;
using Snake3D.Grids;
using UnityEngine.Events;
using Snake3D.Managers;
using UnityEngine;

namespace Snake3D.Gameplay
{
    public class Food : CellBehaviour
    {
        float CollectTimer;
        bool Missed;

        public override void BehavoiourStart(GridCell Parent)
        {
            base.BehavoiourStart(Parent);

            CollectTimer = GameManager.Instance.CollectTime;
        }

        public override void BehavoiourUpdate()
        {
            base.BehavoiourUpdate();

            if(!GameManager.Instance.ClassicMode)
            {
                if(!GameManager.Instance.IsPaused) { CollectTimer -= Time.deltaTime; }
                FXManager.Instance.DisplayCounter(ParentCell.OBJ.transform, CollectTimer);
                if(CollectTimer <= 0f && !Missed)
                {
                    Missed = true;

                    GameManager.Instance.TakeDamage();

                    GameManager.Instance.StartCoroutine(FXManager.Instance.FlashObject(ParentCell.OBJ, 0.125f / 2f, 16, new UnityAction( delegate { GameManager.Instance.IsPaused = true; }), new UnityAction( delegate 
                    {
                        GameManager.Instance.IsPaused = false;
                        GameManager.Instance.SpawnExtraFood();
                        DestroyCell();
                    })));
                }
            }
        }

        protected override void OnCellCollide(GridCell Other)
        {
            base.OnCellCollide(Other);
            if(Other.Tag == "Player")
            {
                GameManager.Instance.SpawnFood();
                GameManager.Instance.snake.AddBodyPart();
                GameManager.Instance.Score++;
                FXManager.Instance.SpawnFX("Coin", GridManager.Instance.GridCoordsToWorldPos(ParentCell.GridPosition));
                AudioManager.Instance.InteractWithSFX("Food Collect", SoundEffectBehaviour.Play);
                DestroyCell();
            }
        }
    }
}