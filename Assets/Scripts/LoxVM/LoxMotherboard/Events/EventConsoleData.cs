using System;
using ULox;
using UnityEngine;

namespace LoxVMod.Events
{
    public class EventConsoleData : MonoBehaviour
    {
        private FastList<Action<ConsoleData>> _listenerVMConsole;
        public void Awake()
        {
            _listenerVMConsole = new();
        }
        public void TriggerEvent(ConsoleData ev)
        {
            foreach (var listener in _listenerVMConsole)
                listener.Invoke(ev);
        }
        public void AddListener(Action<ConsoleData> push) => _listenerVMConsole.Add(push);
        public void RemoveListener(Action<ConsoleData> pop) => _listenerVMConsole.Remove(pop);
    }
}
