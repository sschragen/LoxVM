using ULox;

namespace LoxVMod
{
    public class LoxConsoleClass : UserTypeInternal
    {
        public static readonly Value SharedLoxConsoleClassValue = Value.New(new LoxConsoleClass(console));
        public override InstanceInternal MakeInstance() => CreateInstance();
        public static LoxConsoleInstance CreateInstance() => new(SharedLoxConsoleClassValue.val.asClass);

        private static ConsoleData console;
        public LoxConsoleClass(ConsoleData _console) : base(new HashedString("Console"), UserType.Native)
        {
            console = _console;

            this.AddMethodsToClass(
                (nameof(Log), Value.New(Log, 1, 1)),
                (nameof(Warn), Value.New(Warn, 1, 1)),
                (nameof(Error), Value.New(Error, 1, 1)),
                (nameof(Exception), Value.New(Exception, 1, 1)),
                (nameof(Clear), Value.New(Clear, 1, 0))
            );
        }

        private NativeCallResult Clear(Vm vm)
        {
            console.Clear();
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Log(Vm vm)
        {
            var s = vm.GetArg(1);
            console.Log(s.ToString());
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Warn(Vm vm)
        {
            var s = vm.GetArg(1);
            console.Warn(s.ToString());
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Error(Vm vm)
        {
            var s = vm.GetArg(1);
            console.Error(s.ToString());
            return NativeCallResult.SuccessfulExpression;
        }
        private NativeCallResult Exception(Vm vm)
        {
            var s = vm.GetArg(1);
            console.Exception(s.ToString());
            return NativeCallResult.SuccessfulExpression;
        }
    }
}
