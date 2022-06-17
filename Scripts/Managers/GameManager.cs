using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;
using Snake3D.Audio;
using Snake3D.Grids;
using Snake3D.Gameplay;
using UnityEngine;
using TMPro;

namespace Snake3D.Managers
{
    public class GameManager : Manager<GameManager>
    {
        [Space]
        public bool ClassicMode;
        [HideInInspector]
        public bool IsPaused;

        [Header("Player")]
        public float MovementDelay;
        [System.Serializable]
        public class Keybind
        {
            public KeyCode MainKey;
            public KeyCode AltKey;
            public Vector2Int Direction;

            public bool IsPressed()
            {
                return Input.GetKeyDown(MainKey) || Input.GetKeyDown(AltKey);
            }
        }
        public Keybind[] Keybinds;
        public GameObject TipsObject;
        float MovementTimer;
        [HideInInspector]
        public Snake snake;
        bool SnakeMoved;

        [Header("Food")]
        public float ExtraFoodSpawnDelay;
        public float CollectTime;
        public int ExtraFoodSpawnScoreGap;
        int PrevScore;
        int FoodCount;
        public class ExtraFoodSpawnRequest
        {
            float SpawnTimer;

            public ExtraFoodSpawnRequest()
            {
                SpawnTimer = GameManager.Instance.ExtraFoodSpawnDelay;
            }

            public void UpdateRequest()
            {
                if(!GameManager.Instance.IsPaused) { SpawnTimer -= Time.deltaTime; }

                if(SpawnTimer <= 0f)
                {
                    AudioManager.Instance.InteractWithSFX("Extra Food Spawn", SoundEffectBehaviour.Play);
                    GameManager.Instance.SpawnFood();
                    GameManager.Instance.RemoveExtraFoodSpawnRequest(this);
                }
            }
        }
        List<ExtraFoodSpawnRequest> ExtraFoodSpawnRequests;

        [Header("Bombs")]
        [Min(0)]
        public int StartBombSpawnScore;
        public float BombExplodeTime;
        [Space]
        public float MaxBombSpawnDelay;
        public float MinBombSpawnDelay;
        public float MinMaxBombSpawnDelay;
        public int BombSpawnCountUntilMaxDifficulty;
        float BombSpawnTimer;
        int BombCount;

        [Header("Score")]
        public TMP_Text ScoreText;
        [HideInInspector]
        public int Score;
        public TMP_Text HIScoreText;
        [HideInInspector]
        public int HIScore;
        public GameObject NewHIText;
        bool HIScoreBeat;
        int MoneyBonus;

        [Header("Lives")]
        public GameObject[] HeartSprites;
        public GameObject HeartsContainer;
        [HideInInspector]
        public int Lives;
        bool[] FlashedHearts;
        [HideInInspector]
        public bool IsDead;

        [Header("Mini Menu")]
        public GameObject MiniMenu;
        public TMP_Text MiniMenuTitle;
        public TMP_Text MoneyBonusText;
        public TMP_Text MoneyText;
        public GameObject ResumeButton;

        private void Awake() {
            ClassicMode = ProgressManager.Instance.ClassicGameMode;
        }

        // Start is called before the first frame update
        void Start()
        {
            Init(this);

            AudioManager.Instance.InteractWithSFX("Standby", SoundEffectBehaviour.Play);
            AudioManager.Instance.MuteMusic();

            MovementTimer = MovementDelay;

            Lives = HeartSprites.Length;
            FlashedHearts = new bool[HeartSprites.Length];

            GridCell SnakeCell = GridManager.Instance.AddCell("Snake Head", "Player", ProgressManager.Instance.GetSelectedSkin().HeadColor, Vector2Int.zero);
            snake = SnakeCell.AddBehaviour(new Snake()) as Snake;
            StartCoroutine(FXManager.Instance.FlashObject(TipsObject, 0.25f, int.MaxValue, null, null));

            BombSpawnTimer = Random.Range(MinBombSpawnDelay, MaxBombSpawnDelay);

            ExtraFoodSpawnRequests = new List<ExtraFoodSpawnRequest>();

            NewHIText.SetActive(false);

            HIScore = ProgressManager.Instance.progress.HIScore;
        }

        // Update is called once per frame
        void Update()
        {
            #region Game

            if(!IsPaused)
            {
                foreach (Keybind K in Keybinds)
                {
                    if(K.IsPressed())
                    {
                        snake.TrySetDirection(K.Direction);
                        if(!SnakeMoved)
                        {
                            SnakeMoved = true;
                            Destroy(TipsObject);
                            AudioManager.Instance.InteractWithSFX("First Move", SoundEffectBehaviour.Play);
                            AudioManager.Instance.InteractWithSFX("Standby", SoundEffectBehaviour.Stop);
                            AudioManager.Instance.SetMusicTrack("Snakey");
                            SpawnExtraFood();
                        }
                        break;
                    }
                }

                MovementTimer -= Time.deltaTime;
                if(MovementTimer <= 0f)
                {
                    MovementTimer = MovementDelay;
                    snake.MoveSnake();
                }
            }

            #endregion
            #region Score

            if(Score > HIScore)
            {
                HIScore = Score;
                if(!HIScoreBeat)
                {
                    HIScoreBeat = true;
                    AudioManager.Instance.InteractWithSFX("New HI Score", SoundEffectBehaviour.Play);
                    NewHIText.SetActive(true);
                }
            }
            ScoreText.text = "Score:" + Score.ToString("0");
            HIScoreText.text = "High Score:" + HIScore.ToString("0");

            ProgressManager.Instance.progress.HIScore = HIScore;

            #endregion
            #region Bombs

            if(!ClassicMode)
            {
                if(Score >= StartBombSpawnScore && !IsPaused)
                {
                    BombSpawnTimer -= Time.deltaTime;
                    if(BombSpawnTimer <= 0f)
                    {
                        AudioManager.Instance.InteractWithSFX("Bomb Spawn", SoundEffectBehaviour.Play);
                        SpawnBomb();
                    }
                }
            }

            #endregion
            #region Food

            if(Score >= PrevScore + ExtraFoodSpawnScoreGap && !ClassicMode)
            {
                PrevScore = Score;
                SpawnExtraFood();
            }

            if(ExtraFoodSpawnRequests.Count > 0)
            {
                try
                {
                    foreach (ExtraFoodSpawnRequest EFSR in ExtraFoodSpawnRequests)
                    {
                        EFSR.UpdateRequest();
                    }
                }
                catch(System.InvalidOperationException)
                {}
            }

            #endregion
            #region Hearts

            HeartsContainer.SetActive(!ClassicMode);

            for (int H = 0; H < HeartSprites.Length; H++)
            {
                if(Lives < H + 1 && !FlashedHearts[H])
                {
                    int Index = H;
                    StartCoroutine(FXManager.Instance.FlashObject(HeartSprites[H], 0.125f, 8, new UnityAction(delegate { FlashedHearts[H] = true; }), new UnityAction( delegate
                    { 
                        HeartSprites[Index].SetActive(false);
                    })));
                }
            }

            #endregion
            #region Mini Menu

            if(Input.GetKeyDown(KeyCode.Escape) && !IsPaused)
            {
                IsPaused = true;
                PlaySelectSound();
                SetMiniMenu();
            }

            // Mini Menu Elements
            // Mini Menu Title
            MiniMenuTitle.text = (IsDead) ? "Game Over!" : "Pause";
            MiniMenuTitle.color = (IsDead) ? Color.red : Color.white;
            // Money Bonus Text
            MoneyBonusText.gameObject.SetActive(IsDead);
            MoneyBonusText.text = "Money Bonus:" + MoneyBonus;
            // Money Text
            MoneyText.gameObject.SetActive(IsDead);
            MoneyText.text = "Current Money:" + ProgressManager.Instance.progress.Money;
            // Resume Button
            ResumeButton.SetActive(!IsDead);

            #endregion        
        }

        public void SpawnExtraFood()
        {
            ExtraFoodSpawnRequests.Add(new ExtraFoodSpawnRequest());
        }
        public void SpawnFood()
        {
            Vector2Int SpawnCoord = GridManager.Instance.RandomCoord(true);
            GridManager.Instance.AddCell("Food_" + FoodCount, "Food", Color.yellow, SpawnCoord).AddBehaviour(new Food());
            FXManager.Instance.SpawnFX("Coin", GridManager.Instance.GridCoordsToWorldPos(SpawnCoord));
            FoodCount++;
        }
        public void RemoveExtraFoodSpawnRequest(ExtraFoodSpawnRequest request)
        {
            ExtraFoodSpawnRequests.Remove(request);
        }

        void SpawnBomb()
        {
            BombSpawnTimer = Random.Range(MinBombSpawnDelay, MaxBombSpawnDelay);
            if(MaxBombSpawnDelay > MinMaxBombSpawnDelay)
            {
                MaxBombSpawnDelay -= (MaxBombSpawnDelay - MinMaxBombSpawnDelay) / BombSpawnCountUntilMaxDifficulty;
            }

            Vector2Int SpawnCoord = GridManager.Instance.RandomCoord(true);
            GridManager.Instance.AddCell("Bomb_" + BombCount, "Bomb", Color.red, SpawnCoord).AddBehaviour(new Bomb());
            FXManager.Instance.SpawnFX("Bomb", GridManager.Instance.GridCoordsToWorldPos(SpawnCoord));
            BombCount++;
        }

        public void TakeDamage()
        {
            Lives--;
            if(Lives == 0)
            {
                Die();
            }
            else
            {
                AudioManager.Instance.InteractWithSFX("Take Damage", SoundEffectBehaviour.Play);
            }
        }
        public void Die()
        {
            IsDead = true;
            IsPaused = true;

            MoneyBonus = Score / 2;
            if(HIScoreBeat) { MoneyBonus += 10; }
            ProgressManager.Instance.progress.Money += MoneyBonus;

            AudioManager.Instance.MuteMusic();
            AudioManager.Instance.InteractWithSFX("Die", SoundEffectBehaviour.Play);
            snake.Kill();
        }

        public void SetMiniMenu()
        {
            MiniMenu.SetActive(IsPaused);

            SoundEffectBehaviour soundEffectBehaviour = (IsPaused) ? SoundEffectBehaviour.Pause : SoundEffectBehaviour.Resume;
            AudioManager.Instance.InteractWithAllSFX(soundEffectBehaviour);
            AudioManager.Instance.InteractWithMusic(soundEffectBehaviour);
        }
        public void Resume()
        {
            IsPaused = false;
            PlaySelectSound();
            SetMiniMenu();
        }
        public void Retry()
        {
            PlaySelectSound();
            FadeManager.Instance.FadeTo(SceneManager.GetActiveScene().name);
        }
        public void Menu()
        {
            PlaySelectSound();
            FadeManager.Instance.FadeTo("Menu");
        }
    }
}