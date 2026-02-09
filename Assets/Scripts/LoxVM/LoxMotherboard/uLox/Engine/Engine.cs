using Cysharp.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

namespace ULox
{
    //TODO: feels like his could just be the context...
    public sealed class Engine
    {
        public Context Context { get; }
        private readonly BuildQueue _buildQueue = new();

        public Engine(Context executionContext)
        {
            Context = executionContext;
            Context.Vm.Engine = this;
            Context.Vm.platform = Context.Platform;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BuildProgram(Script script)
        {
            _buildQueue.Enqueue(script);
            ResolveAndBuild();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResolveAndBuild()
        {
            while (_buildQueue.HasItems)
            {
                var script = _buildQueue.Dequeue();
                Context.CompileScript(script);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LocateAndQueue(string filePath)
        {
            var ext = "loxlib";
            var source = Context.Platform.LoadFile(filePath);
            if(source == null)
            {
                Context.Platform.Print($"Error: Could not find script file at {filePath}");
                return;
            }
            var script = new Script(filePath, source, ext);
            _buildQueue.Enqueue(script);
        }
    }
}
