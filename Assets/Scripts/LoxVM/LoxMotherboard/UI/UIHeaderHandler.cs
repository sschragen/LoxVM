using LoxVMod.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoxVMod
{
   
    public class UIHeaderHandler : MonoBehaviour
    {
        public EventVMRunState gameEvent;
        public GameObject Header;
        public List<GameObject> Screens;
        public int screenNo = 0;

        private Color colGreen  = new(0.247f, 0.607f, 0.223f, 0.454f);
        private Color colRed    = new(0.905f, 0.007f, 0.000f, 0.454f);
        private Color colYellow = new(1.000f, 0.737f, 0.105f, 0.454f);
        private Color colPurple = new(1.000f, 0.000f, 0.000f, 0.454f);

        void Start()
        {
            ActivateScreen(screenNo);
        }

        private void OnEnable()
        {
            gameEvent.AddListener(OnSetBTNState);
        }
        private void OnDisable()
        {
            gameEvent.RemoveListener(OnSetBTNState);
        }
        private void OnSetBTNState(EventVMRunState.VMRunState runState)
        {
            var titleBackground = Header.GetComponent<Image>();
            switch (runState)
            {
                case EventVMRunState.VMRunState.Running:
                    titleBackground.color = colGreen;
                    break;
                case EventVMRunState.VMRunState.Stopped:
                    titleBackground.color = colRed;
                    break;
                case EventVMRunState.VMRunState.Paused:
                    titleBackground.color = colYellow;
                    break;
                case EventVMRunState.VMRunState.Idle:
                    titleBackground.color = colGreen;
                    break;
            }
        }

        private void ActivateScreen(int screenNo)
        {
            this.screenNo = screenNo;
            foreach (GameObject Screen in Screens)
            {
                Screen.SetActive(false);
            }
            Screens[screenNo].SetActive(true);
        }

        public void NextScreen()
        {
            screenNo++;
            if (screenNo == Screens.Count)
                screenNo = 0;
            ActivateScreen(screenNo);
        }
    }
}
