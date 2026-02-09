using System;
using System.Collections;
using System.Collections.Generic;
using ULox;
using UnityEngine;

namespace LoxVMod.Events
{
    public class EventVMCommand : MonoBehaviour
    {
        public enum VMCommand
        {
            Start,
            Stop,
            Pause,
            Compile,
        }
        private FastList<Action<VMCommand>> _listenerVMCommand;
        public void Awake()
        {
            _listenerVMCommand = new();
        }
        public void Trigger(VMCommand ev)
        {
            foreach (var listener in _listenerVMCommand)
                listener.Invoke(ev);
        }
        public void AddListener(Action<VMCommand> push) => _listenerVMCommand.Add(push);
        public void RemoveListener(Action<VMCommand> pop) => _listenerVMCommand.Remove(pop);
    }
}
