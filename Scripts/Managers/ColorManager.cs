using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.Managers
{
    public class ColorManager : Manager<ColorManager>
    {
        [Space]
        public GridManager gridManager;
        public Material BaseMat;
        Dictionary<Color, Material> Mats;

        // Start is called before the first frame update
        void Awake()
        {
            // Initialize Manager Object
            Init(this);

            Mats = new Dictionary<Color, Material>();
            Mats.Clear();

            gridManager.InitGrid();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ColorObject(GameObject OBJ, Color color)
        {
            if(!Mats.ContainsKey(color))
            {
                AddColor(color);
            }

            OBJ.GetComponent<MeshRenderer>().sharedMaterial = Mats[color];
        }
        void AddColor(Color Col)
        {
            Material Mat = new Material(BaseMat);
            Mat.name = "Color_" + Col.GetHashCode();
            Mat.color = Col;
            Mats.Add(Col, Mat);
        }
    }
}