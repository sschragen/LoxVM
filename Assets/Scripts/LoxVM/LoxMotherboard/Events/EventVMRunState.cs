using System;
using ULox;
using UnityEngine;

namespace LoxVMod.Events
{
    public class EventVMRunState : MonoBehaviour
    {
        public SOSaveData saveData;
        public enum VMRunState
        {
            Stopped,
            Running,
            Paused,
            Idle,
        }
        private FastList<Action<VMRunState>> _listenerVMState;

        public void Awake()
        {
            _listenerVMState = new();
        }

        public void Trigger(VMRunState ev)
        {
            saveData.runState = ev;
            foreach (var listener in _listenerVMState)
                listener.Invoke(ev);
        }
        public void AddListener(Action<VMRunState> push)
        {
            _listenerVMState.Add(push);
            push.Invoke(saveData.runState);
        }
        //=> _listenerVMState.Add(push);
        public void RemoveListener(Action<VMRunState> pop) => _listenerVMState.Remove(pop);
    }
}
