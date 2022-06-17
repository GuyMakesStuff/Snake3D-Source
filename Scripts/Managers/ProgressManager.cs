using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake3D.IO;

namespace Snake3D.Managers
{
    public class ProgressManager : Manager<ProgressManager>
    {
        [System.Serializable]
        public class Progress : SaveFile
        {
            [Space]
            public int HIScore;
            public int Money;
            public int SelectedSkinIndex;
            public bool[] SkinsUnlocked;
        }
        public Progress progress;
        
        [Space]
        public SettingsManager SettingsMGR;
        public bool ClassicGameMode;

        // Start is called before the first frame update
        void Start()
        {
            Init(this);

            Progress LoadedProgress = Saver.Load(progress) as Progress;
            if(LoadedProgress != null) { progress = LoadedProgress; }
            else { ResetProgress(); }

            FadeManager.Instance.FadeTo("Menu");
        }

        // Update is called once per frame
        void Update()
        {
            SettingsMGR.enabled = true;

            progress.Save();
        }

        public void ResetProgress()
        {
            Progress NewProgress = new Progress();
            NewProgress.FileName = progress.FileName;
            NewProgress.SkinsUnlocked = new bool[SkinManager.Instance.Skins.Length];
            NewProgress.SkinsUnlocked[0] = true;

            progress = NewProgress;
        }

        public SkinManager.Skin GetSelectedSkin()
        {
            return SkinManager.Instance.GetSkin(progress.SelectedSkinIndex);
        }
    }
}