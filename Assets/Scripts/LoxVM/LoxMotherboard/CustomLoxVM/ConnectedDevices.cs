using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Motherboards;
using Assets.Scripts.Objects.Pipes;
using Objects.RoboticArm;
using System;
using System.Collections.Generic;
using ULox;
using UnityEngine;

namespace LoxVMod  
{
    [Serializable]
    public class ScriptRuntimeException : UloxException
    {
        public ScriptRuntimeException(string message) : base(message) { }
    }

    public class ConnectedDevices 
    {
        public Dictionary<long,ILogicable> deviceDict = new();
        public List<ILogicable> deviceList = new();
        public int Count 
        {  
            get { return deviceDict.Count; } 
        }     
#region constructors
        public ConnectedDevices() 
        {
            Update(new List<ILogicable>());
        }
        public ConnectedDevices(List<ILogicable> list)
        {
            this.deviceList = list;
            Update(list);
        }
#endregion
#region Methoden
        
        public void Update (List<ILogicable> list)
        {
            deviceDict.Clear();
            deviceList = list;
            foreach (var device in deviceList)
            {
                deviceDict.Add(device.ReferenceId,device);
            }
        }
        public string GetDisplayName(long id)
        {
            ILogicable device = deviceDict[id];
            if (device != null)
            {
                return device.DisplayName;
            }                
            else throw new ScriptRuntimeException("Device {id} ist not in network");            
        }
        public string GetTypeName (long id)
        {
            ILogicable device = deviceDict[id];

            if (device != null)
            {
                return device.GetAsThing.GetPrefabName();
                //return device.ToString();
            }
            else throw new ScriptRuntimeException("Device {id} ist not in network");
        }
        public FastList<long> GetAllDevices()
        {
            FastList<long> list = new();
            foreach (var device in deviceList)
            {
                list.Add(device.ReferenceId);
            }
            return list;
        }
        public FastList<long> GetDevicesByType(string type)
        {
            FastList<long> list = new();
            //int hType = Animator.StringToHash(type);
            foreach (ILogicable device in deviceList)
            {
                if (device.GetAsThing.GetPrefabName() == type)
                    list.Add(device.ReferenceId);
            }
            return list;
        }
        public FastList<long> GetDevicesByName (string name)
        {
            FastList<long> list = new();
            int hName = Animator.StringToHash(name);
            foreach (ILogicable device in deviceList)
            {
                if (device.GetNameHash() == hName)
                    list.Add(device.ReferenceId);
            }
            return list;
        }
        public FastList<long> GetDevivesByTypeName (string type, string name)
        {
            FastList<long> list = new();

            //int hName = Animator.StringToHash(name);
            //int hType = Animator.StringToHash(type);

            foreach (ILogicable device in deviceList)
            {                
                if ((device.DisplayName == name) & (device.GetAsThing.GetPrefabName() == type) )
                {
                    list.Add(device.ReferenceId);
                }
            }
            return list;
        }
        #endregion
#region Slots
        public double TotalSlots (long Id)
        {
            ILogicable device = deviceDict[Id];
            if (device != null)
            {
                return device.TotalSlots;
            }
            else throw new ScriptRuntimeException($"Device with referenceID [{Id}] is not connected.");
        }
        public string ReadSlotType(long Id, long slotNo)
        {
            ILogicable device = deviceDict[Id];
            if (device != null)
            {
                if (device.TotalSlots > slotNo)
                {
                    return device.GetSlot((int)slotNo).ToString();
                }
                else throw new ScriptRuntimeException($"TotalSlots {device.TotalSlots} > slotNo {slotNo}");
            }
            else throw new ScriptRuntimeException($"Device with referenceID [{Id}] is not connected.");
        }
        public double ReadSlot (long Id,long slotNo, string LogicSlotTypeName)
        {
            ILogicable device = deviceDict[Id];
            if (device != null)
            {
                if (device.TotalSlots > slotNo)
                {
                    Slot s = device.GetSlot((int)slotNo);
                    LogicSlotType type = (LogicSlotType)Enum.Parse(typeof(LogicSlotType), LogicSlotTypeName);
                    if (device.CanLogicRead(type, (int)slotNo))
                    {
                        return device.GetLogicValue(type, (int)slotNo);
                    }
                    else throw new ScriptRuntimeException($"Slot [{slotNo}].[{LogicSlotTypeName}] is not readable.");
                }
                else throw new ScriptRuntimeException($"This Device has only a maximum of {device.TotalSlots} Slots");
            }
            else throw new ScriptRuntimeException($"This Device [{Id}] is not connected.");
        }
#endregion
#region LogicTypes
        public Dictionary<string, double> ReadAllLogicTypes(long Id)
        {
            Dictionary<string, double> AllLogicTypes = new();

            ILogicable device = deviceDict[Id];
            if (device != null)
            {
                foreach (string LogicTypeName in Enum.GetNames(typeof(LogicType)))
                {
                    LogicType type = (LogicType)Enum.Parse(typeof(LogicType), LogicTypeName);
                    if (device.CanLogicWrite(type) || device.CanLogicRead(type))
                    {
                        AllLogicTypes.Add(LogicTypeName, device.GetLogicValue(type));
                    }
                }
                return AllLogicTypes;
            }
            else throw new ScriptRuntimeException($"This Device [{Id}] is not connected.");
        }
        public int WriteAllLogicTypes (long Id,Dictionary<string, double> table)
        {
            int ret = 0;
            ILogicable device = deviceDict[Id];
            foreach (var logic in table)
            {
                LogicType type = (LogicType)Enum.Parse(typeof(LogicType), logic.Key);
                if (device.CanLogicWrite(type))
                {
                    SetLogicValue(Id, logic.Key, logic.Value);
                    ret++;
                }                    
            }
            return ret;
        }
        public double GetLogicValue (long Id,string LogicTypeCode)
        {
            ILogicable device = deviceDict[Id];
            if (device != null)
            {
                //Debug.LogWarning($"Type of Device : {device.GetAsThing.GetPrefabName()}");
                if (Enum.IsDefined(typeof(LogicType), LogicTypeCode))
                {
                    LogicType type = (LogicType)Enum.Parse(typeof(LogicType), LogicTypeCode);
                    return device.GetLogicValue(type);
                }
                else if ((device.GetAsThing.GetPrefabName() == "StructureLarreDockHydroponics")
                        && (LogicTypeCode == "TargetReferenceId"))
                {     
                        var Arm = device as RoboticArmDockHydroponics;
                    //Debug.LogWarning($"Target ID : {Arm.TargetLogicable.ReferenceId}");
                        return Arm.TargetLogicable.ReferenceId;
                    
                }
                else throw new ScriptRuntimeException($"IncorrectLogicType [{device.DisplayName}:{LogicTypeCode}]");
            }            
            else throw new ScriptRuntimeException($"This Device [{Id}] is not connected.");
        }
        public void SetLogicValue(long Id, string LogicTypeCode, double value)
        {
            ILogicable device = deviceDict[Id];
            if (device != null)
            {
                if (Enum.IsDefined(typeof(LogicType), LogicTypeCode))
                {
                    LogicType type = (LogicType)Enum.Parse(typeof(LogicType), LogicTypeCode);
                    if (device.CanLogicWrite(type))
                    {
                        device.SetLogicValue(type, value);
                    }
                    else throw new ScriptRuntimeException($"Cannot write to LogicType [{device.DisplayName}:{LogicTypeCode}]");
                }
                else throw new ScriptRuntimeException($"IncorrectLogicType [{device.DisplayName}:{LogicTypeCode}]");
            }
            else throw new ScriptRuntimeException($"This Device [{Id}] is not connected.");
        }
#endregion
#region helper
        public long HASH(string s)
        {
            return Animator.StringToHash(s);
        }
        new public string ToString()
        {
            string str = new("");
            foreach (var device in deviceDict)
            {
                str += "RefId : " + device.Value.ReferenceId +
                        " Type : " + device.Value.GetPrefabHash() +
                        " Name : " + device.Value.DisplayName + "\n";
            }
            return str;
        }
        #endregion
    }
}
