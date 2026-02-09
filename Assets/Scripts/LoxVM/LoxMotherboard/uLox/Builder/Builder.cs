
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace ULox
{
    public class Builder 
    {
        private FastStack<TokenisedScript> _queue;
        private IPlatform _platform;
        private HashSet<string> _knownScript;
        private Program program;

        public Builder (IPlatform platform)
        {
            _queue = new FastStack<TokenisedScript>();
            _platform = platform;
            _knownScript = new HashSet<string>();
        }

        private List<string> GetDependancies (TokenisedScript tscript)
        {
            List<string> list = new List<string>();
            bool isUsing = false;
            for (var i = 0; i < tscript.Tokens.Count; i++)
            {
                switch (tscript.Tokens[i].TokenType)
                {
                    case TokenType.USING:
                        isUsing = true; 
                        break;
                    case TokenType.END_STATEMENT:
                        isUsing = false;
                        break;
                    case TokenType.STRING:
                        if (isUsing) list.Add(tscript.Tokens[i].Literal);
                        break;
                    default:
                        break;
                }
            }
            return list;
        }

        private void BuildTheQueue(Script script)
        {
            _platform.Warn($"╠ Chain builder for : {script.Name}.{script.Ext} ... ");
            TokenisedScript ts = program.Scanner.Scan(script);
            _queue.Push(ts);
            
            List<string> list = GetDependancies(ts);
            foreach (string dependancy in list)
            {
                script = new Script(dependancy, _platform.LoadFile(dependancy), "loxlib");
                BuildTheQueue(script);
            }
            
        }
        private void CompileTheQueue(Vm vm)
        {
            while (_queue.Count > 0)
            {
                TokenisedScript _tscript = _queue.Pop();
                var existing = program.CompiledScripts.Find(x => x.ScriptHash == _tscript.SourceScript.ScriptHash);
                if (existing == null)
                {
                    _platform.Warn($"╠ Compiling : {_tscript.SourceScript.Name}.{_tscript.SourceScript.Ext} to go {_queue.Count}... ");
                    CompiledScript _compiledScript = program.Compiler.Compile(_tscript);
                    new Optimiser().Optimise(_compiledScript);

                    program.CompiledScripts.Add(_compiledScript);
                }
                else
                {
                    _platform.Warn($"╠ skipping : {_tscript.SourceScript.Name}.{_tscript.SourceScript.Ext}");
                }
            }
        }

        public Program Build(Vm vm,Script script)
        {
            var _mainName = script.Name;
            var _mainExt  = script.Ext;
            program = new Program();

            BuildTheQueue(script);
            CompileTheQueue(vm);
             
            return program;
        }
    }
}
