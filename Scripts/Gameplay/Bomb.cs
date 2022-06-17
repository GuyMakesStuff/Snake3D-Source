using Snake3D.Audio;
using Snake3D.Grids;
using UnityEngine.Events;
using Snake3D.Managers;
using UnityEngine;

namespace Snake3D.Gameplay
{
    public class Bomb : CellBehaviour
    {
        float ExplodeTimer;
        bool Hit;

        public override void BehavoiourStart(GridCell Parent)
        {
            base.BehavoiourStart(Parent);

            ExplodeTimer = GameManager.Instance.BombExplodeTime;
            GameManager.Instance.StartCoroutine(Parent.FlashColor(Color.white * 0.1f, 0.2f, int.MaxValue));
        }

        public override void BehavoiourUpdate()
        {
            base.BehavoiourUpdate();

            if(!GameManager.Instance.IsPaused) { ExplodeTimer -= Time.deltaTime; }
            FXManager.Instance.DisplayCounter(ParentCell.OBJ.transform, ExplodeTimer);
            if(ExplodeTimer <= 0f)
            {
                FXManager.Instance.SpawnFX("Explotion", GridManager.Instance.GridCoordsToWorldPos(ParentCell.GridPosition));
                AudioManager.Instance.InteractWithSFX("Bomb Explode", SoundEffectBehaviour.Play);
                DestroyCell();
            }
        }

        protected override void OnCellCollide(GridCell Other)
        {
            base.OnCellCollide(Other);
            if(Other.Tag == "Player" && !Hit)
            {
                Hit = true;

                ParentCell.OBJ.SetActive(false);
                FXManager.Instance.SpawnFX("Explotion", GridManager.Instance.GridCoordsToWorldPos(ParentCell.GridPosition));
                AudioManager.Instance.InteractWithSFX("Bomb Explode", SoundEffectBehaviour.Play);

                GameManager.Instance.TakeDamage();

                GameManager.Instance.StartCoroutine(FXManager.Instance.FlashObject(GameManager.Instance.snake.ParentCell.OBJ, 0.125f / 2f, 16, new UnityAction( delegate { GameManager.Instance.IsPaused = true; }), new UnityAction( delegate 
                {
                    GameManager.Instance.IsPaused = false;
                    DestroyCell();
                })));
                
                DestroyCell();
            }
        }
    }
}