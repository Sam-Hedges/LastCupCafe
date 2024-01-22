using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeGame
{
    public class GraphicsSettings : MonoBehaviour
    {
        public Toggle fullScreenToggle;
        public Toggle vSyncToggle;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ApplyFullScreen()
        {
            if (fullScreenToggle.isOn)
            {
                Screen.fullScreen = true;
            }
            else
            {
                Screen.fullScreen = false;
            }
        }

        public void ApplyVsync()
        {
            if (vSyncToggle.isOn)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);

            Debug.Log("" + QualitySettings.currentLevel);
        }
    }
}
