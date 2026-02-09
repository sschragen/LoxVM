using System;

namespace ULox
{
    public class RuntimeUloxException : UloxException
    {
        //TODO:only used by vm, move there?
        public RuntimeUloxException(string msg, int currentInstruction, string locationName, string valueStack, string callStack)
            : base(

                  $"{msg}{Environment.NewLine}"+
                  $"   at ip:'{currentInstruction}'{Environment.NewLine}"+
                  $"   in {locationName}.{Environment.NewLine}" +
                  $"===   Stack   ==={Environment.NewLine}{valueStack}{Environment.NewLine}" +
                  $"=== CallStack ==={Environment.NewLine}{callStack}{Environment.NewLine}")
        {
        }
    }
}
