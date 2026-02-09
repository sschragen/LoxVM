using System;
using System.Collections;
using System.Collections.Generic;
using ULox;
using UnityEngine;

namespace LoxVMod
{
    
    public sealed class LoxDeviceInstance : InstanceInternal
    {
        public Value referenceId = new Value();
        public LoxDeviceInstance(UserTypeInternal fromClass)
            : base(fromClass)
        {
        }
        public LoxDeviceInstance(long refId)
        {
            referenceId = Value.New((Int64)refId);
        }
        public Value Count()
        {
            return Value.New(1);
        }

        public Value Get(Value ind)
        {
            return referenceId;
        }

        public void Set(Value val)
        {
            referenceId = val;
        }
    }
}
