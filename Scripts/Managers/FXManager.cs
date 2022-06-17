using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

namespace Snake3D.Managers
{
    public class FXManager : Manager<FXManager>
    {
        [Space]
        public Transform EffectsContainer;
        [System.Serializable]
        public class Effect
        {
            public string Name;
            public GameObject Prefab;
        }
        public Effect[] Effects;
        public GameObject CounterPrefab;
        List<Transform> CountedObjects;
        List<GameObject> Counters;

        void Start()
        {
            Init(this);

            if(EffectsContainer == null) { EffectsContainer = new GameObject("FX").transform; }

            CountedObjects = new List<Transform>();
            Counters = new List<GameObject>();
        }

        void Update()
        {
            for (int C = 0; C < CountedObjects.Count; C++)
            {
                if(CountedObjects[C] == null)
                {
                    CountedObjects.RemoveAt(C);
                    Destroy(Counters[C]);
                    Counters.RemoveAt(C);
                }
            }
        }

        public void SpawnFX(string EffectName, Vector3 Position)
        {
            Effect effect = Array.Find(Effects, Effect => Effect.Name == EffectName);
            if(effect == null)
            {
                Debug.LogError("Effect " + EffectName + " Not Found!");
                return;
            }

            GameObject NewEffect = Instantiate(effect.Prefab, Position, effect.Prefab.transform.rotation, EffectsContainer);
            Destroy(NewEffect, NewEffect.GetComponent<ParticleSystem>().main.duration);
        }

        public void DisplayCounter(Transform OBJ, float Num)
        {
            if(!CountedObjects.Contains(OBJ))
            {
                CountedObjects.Add(OBJ);
                GameObject NewCounter = Instantiate(CounterPrefab, OBJ.position, Quaternion.identity, OBJ);
                NewCounter.name = OBJ.gameObject.name + "_Counter";
                Counters.Add(NewCounter);
            }
            else
            {
                GameObject counter = Counters.Find(GameObject => GameObject.name == OBJ.gameObject.name + "_Counter");
                counter.GetComponentInChildren<TMP_Text>().text = Num.ToString("0.0");
            }
        }

        public IEnumerator FlashObject(GameObject OBJ, float FlashIntervals, int FlashCount, UnityAction FirstAction, UnityAction LastAction)
        {
            int FlashTimes = 0;
            bool Flashed = false;
            if(FirstAction != null) { FirstAction(); }

            while (FlashTimes < FlashCount)
            {
                if(OBJ == null) { break; }

                OBJ.SetActive(Flashed);
                Flashed = !Flashed;
                yield return new WaitForSeconds(FlashIntervals);
                FlashTimes++;
            }

            if(OBJ != null)
            {
                Flashed = true;
                OBJ.SetActive(Flashed);
            }

            if(LastAction != null) { LastAction(); }
        }
    }
}