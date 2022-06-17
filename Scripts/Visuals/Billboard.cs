using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.Visuals
{
    public class Billboard : MonoBehaviour
    {
        Transform MainCam;
        public bool ReverseDirection;

        // Start is called before the first frame update
        void Start()
        {
            MainCam = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 Pos = transform.position;
            Vector3 LookAtTarget = (ReverseDirection) ? Pos - transform.forward : Pos + transform.forward;
            transform.LookAt(LookAtTarget);
        }
    }
}