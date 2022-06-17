using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Snake3D.Audio;
using UnityEngine;

namespace Snake3D.Managers
{
    public class MenuManager : Manager<MenuManager>
    {
        [System.Serializable]
        public class Setting
        {
            public string Label;
            public enum SettingType { Slider, Dropdown, Toggle }
            public SettingType type;
            public string PropertyName;
            [HideInInspector]
            public GameObject Object;
            [HideInInspector]
            public RectTransform rectTransform;

            public void CreateObject(Vector2 Pos, Vector2 AnchorMin, Vector2 AnchorMax)
            {
                Object = Instantiate(MenuManager.Instance.ReferenceSettings[(int)type], Vector3.zero, Quaternion.identity, MenuManager.Instance.SettingsContainer);
                Object.SetActive(true);
                rectTransform = Object.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.up;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.pivot = Vector2.one / 2f;
                rectTransform.anchoredPosition = Pos;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 285f);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 81f);

                InitSetting();
            }

            public void InitSetting()
            {
                switch (type)
                {
                    case SettingType.Slider:
                    {
                        Slider slider = Object.transform.Find("ValueSlider").GetComponent<Slider>();
                        slider.minValue = SettingsManager.Instance.FindSliderSetting(PropertyName).MinValue;
                        slider.maxValue = SettingsManager.Instance.FindSliderSetting(PropertyName).MaxValue;
                        slider.value = SettingsManager.Instance.FindSliderSetting(PropertyName).Value;
                        break;
                    }
                    case SettingType.Dropdown:
                    {
                        TMP_Dropdown dropdown = Object.transform.Find("ValueDropdown").GetComponent<TMP_Dropdown>();
                        dropdown.ClearOptions();
                        dropdown.AddOptions(SettingsManager.Instance.FindDropdownSetting(PropertyName).Options);
                        dropdown.value = SettingsManager.Instance.FindDropdownSetting(PropertyName).Value;
                        dropdown.RefreshShownValue();
                        dropdown.onValueChanged.AddListener(new UnityAction<int>(delegate { AudioManager.Instance.PlaySelectSound(); }));
                        break;
                    }
                    case SettingType.Toggle:
                    {
                        Toggle toggle = Object.transform.Find("ValueToggle").GetComponent<Toggle>();
                        toggle.isOn = SettingsManager.Instance.FindToggleSetting(PropertyName).Value;
                        toggle.onValueChanged.AddListener(new UnityAction<bool>(delegate { AudioManager.Instance.PlaySelectSound(); }));
                        break;
                    }
                }
            }

            public void UpdateSetting()
            {
                rectTransform.Find("SettingLabel").GetComponent<TMP_Text>().text = Label;

                switch (type)
                {
                    case SettingType.Slider:
                    {
                        Slider slider = Object.transform.Find("ValueSlider").GetComponent<Slider>();
                        SettingsManager.Instance.FindSliderSetting(PropertyName).Value = slider.value;
                        break;
                    }
                    case SettingType.Dropdown:
                    {
                        TMP_Dropdown dropdown = Object.transform.Find("ValueDropdown").GetComponent<TMP_Dropdown>();
                        SettingsManager.Instance.FindDropdownSetting(PropertyName).Value = dropdown.value;
                        break;
                    }
                    case SettingType.Toggle:
                    {
                        Toggle toggle = Object.transform.Find("ValueToggle").GetComponent<Toggle>();
                        SettingsManager.Instance.FindToggleSetting(PropertyName).Value = toggle.isOn;
                        break;
                    }
                }
            }
        }
        [Header("Settings")]
        public Setting[] Settings;
        public GameObject[] ReferenceSettings;
        public RectTransform SettingsContainer;
        public float SettingsGapping;

        private void Start() {
            // Initialize Manager Object
            Init(this);

            // Play Music
            AudioManager.Instance.SetMusicTrack("Patiance");

            // Create Settings Menu
            float SettingHeight = SettingsContainer.rect.height / (SettingsContainer.rect.height / SettingsGapping);
            float YPos = -SettingHeight;
            for (int S = 0; S < Settings.Length; S++)
            {
                bool OddNum = (S % 2) != 0;
                if(!OddNum && S != 0) { YPos -= SettingHeight; }
                float XPos = (!OddNum) ? -175f : 175f;
                float AnchorX = (!OddNum) ? 1f : 0f;
                float AnchorY = SettingsContainer.rect.height / YPos;
                Vector2 Anchors = new Vector2(AnchorX, AnchorY);
                
                Settings[S].CreateObject(new Vector2(XPos, YPos), Anchors, Anchors);
            }
        }

        private void Update() {
            foreach (Setting S in Settings)
            {
                S.UpdateSetting();
            }
        }

        public void ResetProgress()
        {
            ProgressManager.Instance.ResetProgress();
            AudioManager.Instance.InteractWithSFX("Reset", SoundEffectBehaviour.Play);
        }

        public void LoadScene(string SceneName)
        {
            FadeManager.Instance.FadeTo(SceneName);
        }
        public void QuitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
            #endif
        }
    }
}