using Snake3D.Audio;
using UnityEngine;
using Snake3D.Interface;
using Snake3D.Visuals;
using Snake3D.Grids;
using TMPro;

namespace Snake3D.Managers
{
    public class ShopManager : Manager<ShopManager>
    {
        [Space]
        public PrevNextMenu SkinSelection;
        DemoSnake demoSnake;

        [Header("Texts")]
        public TMP_Text MoneyText;
        public TMP_Text PriceText;

        [Header("Buttons")]
        public GameObject BuyButton;
        public GameObject SelectButton;
        public GameObject SelectedText;

        // Start is called before the first frame update
        void Start()
        {
            Init(this);

            AudioManager.Instance.SetMusicTrack("Snakey");

            SkinSelection.Value = ProgressManager.Instance.progress.SelectedSkinIndex;
            SkinSelection.MinValue = 0;
            SkinSelection.MaxValue = SkinManager.Instance.Skins.Length - 1;

            GridCell DemoSnakeCell = GridManager.Instance.AddCell("DemoSnake", "DemoPlayer", Color.white, Vector2Int.zero);
            demoSnake = DemoSnakeCell.AddBehaviour(new DemoSnake()) as DemoSnake;
        }

        // Update is called once per frame
        void Update()
        {
            // Useful Variables
            bool BoughtDisplayedSkin = ProgressManager.Instance.progress.SkinsUnlocked[SkinSelection.Value];
            bool SelectedDisplayedSkin = SkinSelection.Value == ProgressManager.Instance.progress.SelectedSkinIndex;

            // Texts
            // Money Text
            MoneyText.text = "Money:" + ProgressManager.Instance.progress.Money;
            // Price text
            int Price = SkinManager.Instance.GetSkin(SkinSelection.Value).Price;
            PriceText.text = "Price:" + Price;
            PriceText.color = (SkinManager.Instance.CanBuySkin(SkinSelection.Value)) ? Color.white : Color.red;
            PriceText.gameObject.SetActive(!BoughtDisplayedSkin);

            // Buttons
            // Buy Button
            BuyButton.SetActive(!BoughtDisplayedSkin);
            // Select Button
            SelectButton.SetActive(BoughtDisplayedSkin && !SelectedDisplayedSkin);
            // Selected Text
            SelectedText.SetActive(BoughtDisplayedSkin && SelectedDisplayedSkin);
        }

        public void UpdateDisplaySnake()
        {
            demoSnake.UpdateSnake();
        }

        public void Buy()
        {
            if(SkinManager.Instance.CanBuySkin(SkinSelection.Value))
            {
                ProgressManager.Instance.progress.Money -= SkinManager.Instance.GetSkin(SkinSelection.Value).Price;
                ProgressManager.Instance.progress.SkinsUnlocked[SkinSelection.Value] = true;
                AudioManager.Instance.InteractWithSFX("Buy", SoundEffectBehaviour.Play);
            }
            else
            {
                AudioManager.Instance.InteractWithSFX("No Money", SoundEffectBehaviour.Play);
            }
        }
        public void Select()
        {
            ProgressManager.Instance.progress.SelectedSkinIndex = SkinSelection.Value;
            AudioManager.Instance.InteractWithSFX("Select Skin", SoundEffectBehaviour.Play);
        }

        public void BackToMenu()
        {
            PlaySelectSound();
            FadeManager.Instance.FadeTo("Menu");
        }
    }
}