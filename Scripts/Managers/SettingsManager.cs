using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake3D.Managers;
using Snake3D.Audio;
using Snake3D.IO;
using System;

namespace Snake3D.Managers
{
    public class SettingsManager : Manager<SettingsManager>
    {
        [Serializable]
        public class SliderSetting
        {
            public string Name;
            public float Value;
            public float MinValue;
            public float MaxValue;
        }
        [Serializable]
        public class DropdownSetting
        {
            public string Name;
            public int Value;
            public List<string> Options;
        }
        [Serializable]
        public class ToggleSetting
        {
            public string Name;
            public bool Value;
        }

        [Serializable]
        public class Settings : SaveFile
        {
            [Space]
            public SliderSetting[] SliderSettings;
            public DropdownSetting[] DropdownSettings;
            public ToggleSetting[] ToggleSettings;
        }
        
        [Space]
        public Settings settings;
        bool SettingsLoadedSuccessfuly;
        Resolution[] Resolutions;
        int PrevResIndex;

        // Start is called before the first frame update
        void Awake()
        {
            // Initialize Manager Object
            Init(this);

            // Load The Players Settings
            Settings LoadedSettings = Saver.Load(settings) as Settings;
            SettingsLoadedSuccessfuly = LoadedSettings != null;
            if(SettingsLoadedSuccessfuly) { settings = LoadedSettings; }
            settings.Save();

            // Initialize Resolutions Option
            InitRes();

            // Instantly Apply Loaded Settings When The Game Is Booted Up
            ApplySettings();
        }
        void InitRes()
        {
            // Store The Resolution Setting In A Variable
            DropdownSetting ResSetting = FindDropdownSetting("Resolution");
            // Clear All Previous Options From The Resolution Setting.
            // We Want The Options To Automatically Be All Of The Available Screen Resolutions.
            ResSetting.Options.Clear();
            // Get All Available Screen Resolutions.
            Resolutions = Screen.resolutions;
            // The Index Of The Current Screen Resolution On The Resolutions Array.
            int CurResIndex = 0;
            for (int R = 0; R < Resolutions.Length; R++)
            {
                // The Screen Resolution From The Resolutions Array At The Index R.
                Resolution Res = Resolutions[R];
                // The Current Screen Resolution.
                Resolution CurRes = Screen.currentResolution;

                // Convert The Resolution To A String Variable.
                string Res2String = Res.width + "x" + Res.height + "@" + Res.refreshRate + "Hz";
                // Add The Converted String Variable To The Options List.
                ResSetting.Options.Add(Res2String);

                // If The Resolution At Index R Is The Current Screen Resolution,
                // Set The Current Screen Resolution Index To R.
                if(Res.width == CurRes.width && Res.height == CurRes.height && Res.refreshRate == CurRes.refreshRate)
                {
                    CurResIndex = R;
                }
            }

            // If The Player Has Not Set A Resolution Of Their Own,
            // Set The Value Of The Resolution Setting To The Index Of The Current Screen Resolution On The Resolutions Array.
            if(!SettingsLoadedSuccessfuly) { ResSetting.Value = CurResIndex; }
            PrevResIndex = -PrevResIndex;
        }

        // Update is called once per frame
        void Update()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            // Sound Settings
            // Music Volume
            AudioManager.Instance.SetChannelVolume("Music", FindSliderSetting("MusicVol").Value);
            // Sound Effects Volume
            AudioManager.Instance.SetChannelVolume("SFX", FindSliderSetting("SFXVol").Value);

            // Video Settings
            // Quality
            QualitySettings.SetQualityLevel(FindDropdownSetting("Quality").Value);
            // Fullscreen
            Screen.fullScreen = FindToggleSetting("FS").Value;
            // Resolution (Takes A Little More Fuss)
            int ResIndex = FindDropdownSetting("Resolution").Value;
            if(ResIndex != PrevResIndex)
            {
                PrevResIndex = ResIndex;
                Resolution Res = Resolutions[ResIndex];
                Screen.SetResolution(Res.width, Res.height, Screen.fullScreen, Res.refreshRate);
            }
            // Mouse Lock
            MouseManager.Instance.HideMouseWhilePlaying = FindToggleSetting("ML").Value;

            // Game Settings
            // Classic Game Mode
            ProgressManager.Instance.ClassicGameMode = FindToggleSetting("Classic").Value;

            // Save Settings
            settings.Save();
        }

        // Get The Slider Setting With The Given Name
        public SliderSetting FindSliderSetting(string Name)
        {
            return Array.Find(settings.SliderSettings, SliderSetting => SliderSetting.Name == Name);
        }
        // Get The Dropdown Setting With The Given Name
        public DropdownSetting FindDropdownSetting(string Name)
        {
            return Array.Find(settings.DropdownSettings, DropdownSetting => DropdownSetting.Name == Name);
        }
        // Get The Toggle Setting With The Given Name
        public ToggleSetting FindToggleSetting(string Name)
        {
            return Array.Find(settings.ToggleSettings, ToggleSetting => ToggleSetting.Name == Name);
        }
    }
}