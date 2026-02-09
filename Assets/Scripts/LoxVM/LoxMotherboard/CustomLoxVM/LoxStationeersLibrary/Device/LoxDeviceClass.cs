using Assets.Scripts.Objects.Pipes;
using LoxVMod;
using ULox;

public class LoxDeviceClass : UserTypeInternal
{
    public static readonly Value SharedLoxDeviceClassValue = Value.New(new LoxDeviceClass(connectedDevices, console));
    public override InstanceInternal MakeInstance() => CreateInstance();
    public static LoxDeviceInstance CreateInstance() => new(SharedLoxDeviceClassValue.val.asClass);

    private static ConnectedDevices connectedDevices;
    private static ConsoleData console;

    public LoxDeviceClass(ConnectedDevices _connectedDevices, ConsoleData _console) : base(new HashedString("Device"), UserType.Native)
    {
        connectedDevices = _connectedDevices;
        console = _console;

        this.AddMethodsToClass(
            (ClassTypeCompilette.InitMethodName.String, Value.New(InitInstance, 1, 1)),
            (nameof (ToString), Value.New(ToString, 1, 0)),
            (nameof(GetName), Value.New(GetName, 1, 0)),
            (nameof(GetType), Value.New(GetType, 1, 0)),
            (nameof(GetLogic), Value.New(GetLogic, 1, 1)),
            (nameof(SetLogic), Value.New(SetLogic, 1, 2)),
            (nameof(TotalSlots), Value.New(TotalSlots, 1,0)),
            (nameof(ReadSlotType), Value.New(ReadSlotType, 1, 1)),
            (nameof(ReadSlot), Value.New(ReadSlot, 1, 2)),
            (nameof(GetTarget), Value.New(GetTarget,1,0)),
            (nameof(CheatSeed), Value.New(CheatSeed,0,1))
        );
    }
    private NativeCallResult InitInstance(Vm vm)
    {
        var instance = vm.GetArg(0) ;
        var instanceData = instance.val.asInstance as LoxDeviceInstance;
        if (vm.GetArg(1).type == ValueType.Double)
        {
            var value = vm.GetArg(1);
            instanceData.referenceId = value;
            vm.SetNativeReturn(0, instance);
            return NativeCallResult.SuccessfulExpression;
        }
        else
        {
            vm.ThrowRuntimeException("Type of Argument must be <number>.");
            return NativeCallResult.Failure;
        }
    }
    private NativeCallResult ToString(Vm vm)
    {
        var instance = vm.GetArg(0);
        var instanceData = instance.val.asInstance as LoxDeviceInstance;
        long id = instanceData.referenceId.val.asLong;

        ILogicable dev = connectedDevices.deviceDict[id];
        string ret = $"This is a Device with Name : {dev.DisplayName}, Type : {connectedDevices.GetTypeName(id)} and referenceId : {dev.ReferenceId}";

        vm.SetNativeReturn(0, Value.New(ret));
        return NativeCallResult.SuccessfulExpression;    
    }
    private NativeCallResult GetName(Vm vm)
    {
        var instance = vm.GetArg(0);
        var instanceData = instance.val.asInstance as LoxDeviceInstance;
        long id = instanceData.referenceId.val.asLong;

        vm.SetNativeReturn(0, Value.New(connectedDevices.deviceDict[id].DisplayName));
        return NativeCallResult.SuccessfulExpression;
    }
    private NativeCallResult GetType(Vm vm)
    {
        var instance = vm.GetArg(0);
        var instanceData = instance.val.asInstance as LoxDeviceInstance;
        long id = instanceData.referenceId.val.asLong;

        vm.SetNativeReturn(0, Value.New(connectedDevices.GetTypeName(id)));
        return NativeCallResult.SuccessfulExpression;
    }
    private NativeCallResult GetTarget(Vm vm)
    {
        var instance = vm.GetArg(0);
        var instanceData = instance.val.asInstance as LoxDeviceInstance;
        long id = instanceData.referenceId.val.asLong;
        long tId = (long)connectedDevices.GetLogicValue(id, "TargetReferenceId");
        var targetDevice = LoxDeviceClass.CreateInstance();
        targetDevice.Set(Value.New(tId));
        vm.SetNativeReturn(0, Value.New(targetDevice));

        return NativeCallResult.SuccessfulExpression;     
    }
    private NativeCallResult GetLogic(Vm vm)
    {
        if (vm.GetArg(1).type == ValueType.String)
        {
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceInstance;
            long id = instanceData.referenceId.val.asLong;

            var logic = vm.GetArg(1).val.asString.String;

            vm.SetNativeReturn(0, Value.New(connectedDevices.GetLogicValue(id, logic)));
            return NativeCallResult.SuccessfulExpression;
        }
        else
        {
            vm.ThrowRuntimeException("Type of Argument must be <string>.");
            return NativeCallResult.Failure;
        }
    }
    private NativeCallResult SetLogic (Vm vm)
    {
        if ((vm.GetArg(1).type == ValueType.String) && (vm.GetArg(2).type == ValueType.Double))
        {
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceInstance;
            long id = instanceData.referenceId.val.asLong;
            var logic = vm.GetArg(1).val.asString.String;
            var newValue = vm.GetArg(2).val.asDouble;
            connectedDevices.SetLogicValue(id, logic, newValue);
            vm.SetNativeReturn(0, Value.Null());
            return NativeCallResult.SuccessfulExpression;
        }
        else
        {
            vm.ThrowRuntimeException("Type of Argument must be <string>,<number>.");
            return NativeCallResult.Failure;
        }
    }
    private NativeCallResult TotalSlots(Vm vm)
    {
        var instance = vm.GetArg(0);
        var instanceData = instance.val.asInstance as LoxDeviceInstance;
        long id = instanceData.referenceId.val.asLong;

        vm.SetNativeReturn(0, Value.New(connectedDevices.TotalSlots(id)));
        return NativeCallResult.SuccessfulExpression;
    }
    private NativeCallResult ReadSlotType(Vm vm)
    {
        if (vm.GetArg(1).type == ValueType.Double)
        { 
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceInstance;
            long id = instanceData.referenceId.val.asLong;
            long slotNo = vm.GetArg(1).val.asLong;
            vm.SetNativeReturn(0, Value.New(connectedDevices.ReadSlotType(id,slotNo)));
            return NativeCallResult.SuccessfulExpression;
        }
        else
        {
            vm.ThrowRuntimeException("Type of Argument must be <number>.");
            return NativeCallResult.Failure;
        }
}
    private NativeCallResult ReadSlot(Vm vm)
    {
        if ((vm.GetArg(1).type == ValueType.Double) && (vm.GetArg(2).type == ValueType.String))
        {
            var instance = vm.GetArg(0);
            var instanceData = instance.val.asInstance as LoxDeviceInstance;
            long id = instanceData.referenceId.val.asLong;
            long slotNo = vm.GetArg(1).val.asLong;
            string slotName = vm.GetArg(2).val.asString.String;
            vm.SetNativeReturn(0, Value.New(connectedDevices.ReadSlot(id, slotNo, slotName)));
            return NativeCallResult.SuccessfulExpression;
        }
        else
        {
            vm.ThrowRuntimeException("Type of Argument must be <number>,<string>.");
            return NativeCallResult.Failure;
        }
    }
    private NativeCallResult CheatSeed(Vm vm)
    {
        var instance = vm.GetArg(0);
        var instanceData = instance.val.asInstance as LoxDeviceInstance;
        long id = instanceData.referenceId.val.asLong;
        HydroponicsTrayDevice traydev = connectedDevices.deviceDict[id].GetAsThing as HydroponicsTrayDevice;


        //traydev.Plant = new Assets.Scripts.Objects.Items.;

        return NativeCallResult.SuccessfulExpression;
    }
}
