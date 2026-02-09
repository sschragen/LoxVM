using System.Linq;
using System.Runtime.CompilerServices;
using ULox;


namespace LoxVMod
{
    public class LoxDeviceListClass : UserTypeInternal
    {
        public static readonly Value SharedLoxConsoleClassValue 
            = Value.New(new LoxDeviceListClass(connectedDevices, console));
        public override InstanceInternal MakeInstance() 
            => CreateInstance();
        public static LoxDeviceListInstance CreateInstance() 
            => new(SharedLoxConsoleClassValue.val.asClass);

        private static ConnectedDevices connectedDevices;
        private static ConsoleData console;

        public LoxDeviceListClass(ConnectedDevices _connectedDevices, ConsoleData _console) : base(new HashedString("DeviceList"), UserType.Native)
        {
            connectedDevices = _connectedDevices;
            console = _console;

            this.AddMethodsToClass(
                (ClassTypeCompilette.InitMethodName.String, Value.New(InitInstance, 1, 0)),
                (nameof(ToString), Value.New(ToString, 1, 0)),
                (nameof(Count), Value.New(Count, 1, 0)),
                (nameof(Add), Value.New(Add, 1, 1)),
                (nameof(Clear), Value.New(Clear, 1, 0)),
                (nameof(SetLogic), Value.New(SetLogic, 1, 2)),
                (nameof(GetLogic), Value.New(GetLogic, 1, 2))
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private NativeCallResult InitInstance(Vm vm)
        {
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceListInstance;
            
            vm.SetNativeReturn(0, instance);
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult InitInstanceCopy(Vm vm)
        {
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceListInstance;
            var value = vm.GetArg(1).val.asInstance as LoxDeviceListInstance;
                // Copy Constructor
            instanceData = value;

            vm.SetNativeReturn(0, instance);
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult ToString(Vm vm)
        {
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceListInstance;

            string ret = $"This is a DeviceList with {instanceData.Count()} Devices.";

            vm.SetNativeReturn(0, Value.New(ret));
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Count(Vm vm)
        {
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceListInstance;
            
            vm.SetNativeReturn(0, instanceData.Count());
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Add(Vm vm)
        {
            var instance = vm.GetArg(0);
            var val = vm.GetArg(1);
            var instanceData = instance.val.asInstance as LoxDeviceListInstance;

            instanceData.List.Add((LoxDeviceInstance)val.val.asObject);

            vm.SetNativeReturn(0, instanceData.Count());
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Clear(Vm vm)
        {
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceListInstance;

            instanceData.List.Clear();

            vm.SetNativeReturn(0, instanceData.Count());
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult SetLogic(Vm vm)
        {
            var instance = vm.GetArg(0);
            var logicType = vm.GetArg(1).val.asString.String;
            var logicValue = vm.GetArg(2).val.asDouble;
            var instanceData = instance.val.asInstance as LoxDeviceListInstance;
            foreach (var item in instanceData.List)
            {
                try
                { 
                    connectedDevices.SetLogicValue(item.referenceId.val.asLong, logicType, logicValue); 
                }
                catch 
                {
                    vm.ThrowRuntimeException($"LogicValue {connectedDevices.deviceDict[item.referenceId.val.asLong].DisplayName}:{logicType} ist not avaliable .");
                    return NativeCallResult.Failure;
                }
            }
            vm.SetNativeReturn(0, Value.Null());
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult GetLogic(Vm vm)
        {
            if ((vm.GetArg(1).type == ValueType.String) && (vm.GetArg(1).type == ValueType.Double))
            {
                var instance = vm.GetArg(0);
                var logicType = vm.GetArg(1).val.asString.String;
                var batchMode = (int)vm.GetArg(2).val.asDouble;
                var instanceData = instance.val.asInstance as LoxDeviceListInstance;
                double ret = 0;
                FastList<double> values = new FastList<double>();
                foreach (var item in instanceData.List)
                {
                    try
                    {
                        ret = connectedDevices.GetLogicValue(item.referenceId.val.asLong, logicType);
                    }
                    catch 
                    {
                        vm.ThrowRuntimeException($"LogicValue {connectedDevices.deviceDict[item.referenceId.val.asLong].DisplayName}:{logicType} ist not avaliable.");
                        return NativeCallResult.Failure;
                    }
                    finally 
                    { 
                        values.Add(ret);
                    }                
                }
                if (values.Count > 0)
                {
                    switch (batchMode)
                    {
                        case 0: // Average
                            {
                                ret = values.Sum() / values.Count();
                                break;
                            }
                        case 1: // Sum
                            {
                                ret = values.Sum();
                                break;
                            }
                        case 2: // Minimum
                            {
                                ret = values.Min();
                                break;
                            }
                        case 3: // Maximum
                            {
                                ret = values.Max();
                                break;
                            }
                        default:
                            {
                                vm.ThrowRuntimeException("DeviceList : BatchMode not supported.");
                                return NativeCallResult.Failure;
                            }
                    }
                }
                else ret = 0;

                vm.SetNativeReturn(0, Value.New(ret));
                return NativeCallResult.SuccessfulExpression;
            }
            {
                vm.ThrowRuntimeException("Type of Argument must be <string>,<number>.");
                return NativeCallResult.Failure;
            }
        }

    }
}
