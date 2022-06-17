using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.Managers
{
    public class SkinManager : Manager<SkinManager>
    {
        [System.Serializable]
        public class Skin
        {
            public int Price;
            public Color HeadColor;
            public Color BodyColor;
        }
        [Space]
        public Skin[] Skins;

        // Start is called before the first frame update
        void Awake()
        {
            Init(this);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public Skin GetSkin(int Index)
        {
            return Skins[Index];
        }

        public bool CanBuySkin(int SkinIndex)
        {
            return ProgressManager.Instance.progress.Money >= Skins[SkinIndex].Price;
        }
    }
}