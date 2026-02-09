using Assets.Scripts.Objects.Pipes;
using LoxVMod;
using ULox;

public class LoxUIElementClass : UserTypeInternal
{
    public static readonly Value SharedLoxDeviceClassValue = Value.New(new LoxUIElementClass(connectedDevices, console));
    public override InstanceInternal MakeInstance() => CreateInstance();
    public static LoxUIElementInstance CreateInstance() => new(SharedLoxDeviceClassValue.val.asClass);

    private static ConnectedDevices connectedDevices;
    private static ConsoleData console;

    public LoxUIElementClass(ConnectedDevices _connectedDevices, ConsoleData _console) : base(new HashedString("UIElement"), UserType.Native)
    {
        connectedDevices = _connectedDevices;
        console = _console;

        this.AddMethodsToClass(
            (ClassTypeCompilette.InitMethodName.String, Value.New(InitInstance, 1, 1)),
            (nameof(ToString), Value.New(ToString, 1, 0)),
            (nameof(GetName), Value.New(GetName, 1, 0))

        );
    }
    private NativeCallResult InitInstance(Vm vm)
    {
        var instance = vm.GetArg(0);
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
}

