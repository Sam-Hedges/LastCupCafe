using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcadeGame
{
    public class TutorialSystem : MonoBehaviour
    {

        [SerializeField] GameObject[] uiTutorial;
         TMP_Text messageTitleVar;
         TMP_Text messageVar;

        [SerializeField] TMP_Text messageTitleGB;
        [SerializeField] TMP_Text messageGB;

        void Update()
        {
            
        }

        public void SetMessage()
        {
            messageTitleGB.text = messageTitleVar.text;
            messageGB.text = messageVar.text;
        }

        public void TutorialMessage()
        {
            foreach (var i in uiTutorial)
            {
                i.SetActive(true);
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }

        public void ExitMessage()
        {
            foreach (var i in uiTutorial)
            {
                i.SetActive(false);
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 0f;
        }

        /// <summary>
        /// Method to return to main menu
        /// </summary>
        public void Exit()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
    }

}
