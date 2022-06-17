using System.Collections;
using Snake3D.Audio;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Snake3D.Managers
{
    public class FadeManager : Manager<FadeManager>
    {
        [Space]
        public Animator FadePanel;
        [HideInInspector]
        public bool IsFaded;

        void Awake()
        {
            Init(this);
        }

        void Update()
        {
            FadePanel.SetBool("Is Faded", IsFaded);
        }

        public void FadeTo(string SceneName)
        {
            StartCoroutine(fadeTo(SceneName));
        }
        IEnumerator fadeTo(string scene)
        {
            IsFaded = true;
            AudioManager.Instance.InteractWithSFX("Fade In", SoundEffectBehaviour.Play);

            yield return new WaitForSeconds(0.5f);

            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            while(!operation.isDone)
            {
                yield return null;
            }

            IsFaded = false;
            AudioManager.Instance.InteractWithSFX("Fade Out", SoundEffectBehaviour.Play);
        }
    }
}