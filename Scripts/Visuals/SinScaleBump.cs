using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.Visuals
{
    public class SinScaleBump : MonoBehaviour
    {
        public float SinRange;
        public float SinSpeed;
        float t;
        Vector3 BaseScale;

        void Start()
        {
            BaseScale = transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            t += SinSpeed * Time.deltaTime;
            float X = Mathf.Sin(t * 90f) * SinRange;
            transform.localScale = BaseScale + (Vector3.one * X);
        }
    }
}