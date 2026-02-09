using System;
using ULox;

namespace LoxVMod
{

    public sealed class LoxUIElementInstance : InstanceInternal
    {
        public Value referenceId = new Value();
        public LoxUIElementInstance(UserTypeInternal fromClass)
            : base(fromClass)
        {
        }
        public LoxUIElementInstance(long refId)
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
