using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.Managers
{
    public class MouseManager : Manager<MouseManager>
    {
        [Space]
        public bool HideMouseWhilePlaying = true;

        // Start is called before the first frame update
        void Start()
        {
            Init(this);
        }

        // Update is called once per frame
        void Update()
        {
            bool LockMouse = HideMouseWhilePlaying && InGame();
            UpdateMouseVisible(!LockMouse);
        }
        bool InGame()
        {
            if(GameManager.IsInstanced)
            {
                return !GameManager.Instance.IsPaused;
            }

            return false;
        }
        void UpdateMouseVisible(bool ShowMouse)
        {
            Cursor.visible = ShowMouse;
            Cursor.lockState = (ShowMouse) ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}