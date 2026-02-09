using System.Runtime.CompilerServices;
using ULox;

namespace LoxVMod
{
    public class LoxDeviceListInstance : InstanceInternal, INativeCollection
    {
        public FastList<LoxDeviceInstance> List = new FastList<LoxDeviceInstance>();
        public LoxDeviceListInstance(UserTypeInternal fromClass)
            : base(fromClass)
        {
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Value Get(Value ind)
        {
            return Value.New(List[(int)ind.val.asDouble]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Value ind, Value val)
        {
            if (IsReadOnly)
                throw new UloxException($"Attempted to Set index '{ind}' to '{val}', but list is read only.");

            List[(int)ind.val.asDouble] = (LoxDeviceInstance)val.val.asInstance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Value Count()
        {
            return Value.New(List.Count);
        }
    }
}
