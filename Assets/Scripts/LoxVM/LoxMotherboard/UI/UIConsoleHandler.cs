using LoxVMod.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoxVMod
{
    public class UIConsoleHandler : MonoBehaviour
    {
        public EventConsoleData eventConsole;
        public TextMeshProUGUI TextMeshProUGUI;
        public Scrollbar scrollbar;

        private void OnEnable()
        {
            eventConsole.AddListener(onConsoleDataChanged);
        }
        private void OnDisable()
        {
            eventConsole.RemoveListener(onConsoleDataChanged);
        }
        public void onConsoleDataChanged(ConsoleData Sender)
        {
            TextMeshProUGUI.text = Sender.GetText();
            scrollbar.size = 1 / (Sender.Count + 10);
            scrollbar.value = 0;// data.Count;
        }
    }
}
