using LoxVMod.Events;
using UnityEngine;
using UnityEngine.UI;

namespace LoxVMod
{
    public class UIHandler : MonoBehaviour
    {
        public Button btn_Start;
        public Button btn_Pause;
        public Button btn_Stop;
        public MSVMDropdown scriptDropdown;
        
        public EventVMCommand eventCommand;
        public EventVMRunState eventRunState;

        private void OnEnable()
        {
            eventRunState.AddListener(OnSetBTNState);
        }
        private void OnDisable()
        {
            eventRunState.RemoveListener(OnSetBTNState);
        }

        public void OnBtnStart()
        {
            eventCommand.Trigger(EventVMCommand.VMCommand.Start);
        }
        public void OnBtnStop()
        {
            eventCommand.Trigger(EventVMCommand.VMCommand.Stop);
        }
        public void OnBtnPause()
        {
            eventCommand.Trigger(EventVMCommand.VMCommand.Pause);
        }
        private void OnSetBTNState(EventVMRunState.VMRunState runState)
        {
            switch (runState)
            {
                case EventVMRunState.VMRunState.Running:
                    btn_Start.interactable = false;
                    btn_Pause.interactable = true;
                    btn_Stop.interactable = true;
                    scriptDropdown.dropdown.interactable = false;
                    break;
                case EventVMRunState.VMRunState.Stopped:
                    btn_Start.interactable = true;
                    btn_Pause.interactable = false;
                    btn_Stop.interactable = false;
                    scriptDropdown.dropdown.interactable = true;
                    break;
                case EventVMRunState.VMRunState.Paused:
                    btn_Start.interactable = true;
                    btn_Pause.interactable = false;
                    btn_Stop.interactable = true;
                    scriptDropdown.dropdown.interactable = false;
                    break;
                case EventVMRunState.VMRunState.Idle:
                    btn_Start.interactable = false;
                    btn_Pause.interactable = true;
                    btn_Stop.interactable = true;
                    scriptDropdown.dropdown.interactable = false;
                    break;
            }
        }
    }
}
