using LoxVMod.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoxVMod
{
    [CreateAssetMenu(menuName ="new SaveData", fileName ="saveData")]
    public class SOSaveData : ScriptableObject
    {
        public EventVMRunState.VMRunState runState;
        public string filename;
    }
}
