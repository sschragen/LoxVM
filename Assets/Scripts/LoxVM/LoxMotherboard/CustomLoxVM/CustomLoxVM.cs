using Cysharp.Threading.Tasks;
using LoxVMod.Events;
using ULox;
using UnityEngine;
using static WorldManager;

namespace LoxVMod
{
    public class CustomLoxVM : MonoBehaviour
    {
        public SOSaveData saveData;
        [SerializeField] ConsoleData console;
        [SerializeField] ScriptDictionary scriptDictionary;
        //[SerializeField] UIHandler buttonHandler;
        [SerializeField] EventVMRunState eventRunState;
        [SerializeField] EventVMCommand eventCommand;

        private Engine engine;
        private Vm vm;
        private Builder builder;
        private Program program;
        private IPlatform platform;

        private Value gameUpdateFunction = Value.Null();
        private Value gameStartFunction = Value.Null();

        public ConnectedDevices connectedDevices;

        //private string Filename { get { return saveData.filename; } }

#region MonoBehaviour
        void Awake()
        {            
            connectedDevices = new ConnectedDevices();
            
        }
        public void Start()
        {
            //eventRunState.TriggerEvent(EventVMRunState.VMRunState.Stopped);
        }
        private void OnEnable()
        {
            eventCommand.AddListener(OnCommandEvent);
        }
        private void OnDisable()
        {
            eventCommand.RemoveListener(OnCommandEvent);
        }
        private void Update()
        {
            //CallFunction(gameUpdateFunction).Preserve();
        }

        #endregion
        #region LoxVM

        public void OnCommandEvent(EventVMCommand.VMCommand cmd)
        {
            switch (cmd)
            {
                case EventVMCommand.VMCommand.Start:
                    if (string.IsNullOrEmpty(saveData.filename))
                    {
                        console.Error($"File {saveData.filename} not found.");
                    }
                    else
                    {
                        switch (saveData.runState)
                        {
                            case EventVMRunState.VMRunState.Stopped:
                                console.Log($"Program {saveData.filename} started.");
                                eventRunState.Trigger(EventVMRunState.VMRunState.Running);
                                VMRUN(true).Forget();
                                break;
                            case EventVMRunState.VMRunState.Paused:
                                console.Log($"Program {saveData.filename} resumed.");
                                eventRunState.Trigger(EventVMRunState.VMRunState.Running);
                                break;
                            case EventVMRunState.VMRunState.Running:
                                console.Log($"Program {saveData.filename} already started.");
                                break;
                        }
                    }
                    break;
                case EventVMCommand.VMCommand.Stop:
                    engine.Context.Vm.interpreterState = InterpreterState.STOPPED;
                    eventRunState.Trigger(EventVMRunState.VMRunState.Stopped);
                    console.Log("Program terminated.");
                    break;
                case EventVMCommand.VMCommand.Pause:
                    engine.Context.Vm.interpreterState = InterpreterState.PAUSED;
                    eventRunState.Trigger(EventVMRunState.VMRunState.Paused);
                    console.Log("Program paused.");
                    break;
                case EventVMCommand.VMCommand.Compile:
                    VMRUN(false).Forget();
                    break;


            }
        }
        public void ResetConsole()
        {
            console.Clear();
            console.Log ("LoxVM says welcome ...");
        }

        private async UniTask<InterpreterResult> VMRUN(bool doRun)
        {
            Debug.Log("Task VmRun started");
            
            try
            {
                platform = new LoxVMPlatform(connectedDevices, scriptDictionary, console);
                builder = new(platform);
                vm = new();
                Script script = new(saveData.filename, scriptDictionary.GetProgrammScript(saveData.filename), "lox");
                //vm.Clear();
                platform.Warn($"╔ Builder v1.0 started building {script.Name}.lox ... ");

                //await UniTask.SwitchToThreadPool();
                program = builder.Build(vm, script);
                //await UniTask.SwitchToMainThread();

                platform.Warn($"╚ Builder has finished building {script.Name}.lox");

                engine = new Engine(
                    new Context(
                        program,
                        vm,
                        platform
                    )
                );
            }
            catch (CompilerException e)
            {
                platform.Exception("╔ " + e.Message);
                platform.Exception("╚ " + e.Extension);
                eventRunState.Trigger(EventVMRunState.VMRunState.Stopped);
                return InterpreterResult.ERROR;
            }
            catch (UloxException e)
            {
                platform.Exception(e.Message);
                eventRunState.Trigger(EventVMRunState.VMRunState.Stopped);
                return InterpreterResult.ERROR;
            }
            Debug.Log("Compile done.");
            try 
            {
                await UniTask.SwitchToThreadPool();
                vm.PrepareTypes(program.TypeInfo);
                eventRunState.Trigger(EventVMRunState.VMRunState.Running);
                console.Log("Initializing program");

                //await UniTask.SwitchToThreadPool();
                foreach (CompiledScript cs in program.CompiledScripts)
                {
                    await vm.Interpret(cs.TopLevelChunk);
                }
                //await UniTask.SwitchToMainThread();
                //eventRunState.Trigger(EventVMRunState.VMRunState.Idle);

                engine.Context.Vm.Globals.Get(new HashedString("Start"), out gameStartFunction);
                engine.Context.Vm.Globals.Get(new HashedString("Update"), out gameUpdateFunction);
            

                if (!gameStartFunction.IsNull())
                {
                    Debug.Log("Calling 'Start'.");
                    //eventRunState.Trigger(EventVMRunState.VMRunState.Running);
                    //await UniTask.SwitchToThreadPool();
                    await vm.PushCallFrameAndRun(gameStartFunction, 0);
                    //await UniTask.SwitchToMainThread();
                    //eventRunState.Trigger(EventVMRunState.VMRunState.Idle);
                    Debug.Log("Calling 'Start' done.");
                }

                console.Log("Program initialized.");

                if (!doRun) eventRunState.Trigger(EventVMRunState.VMRunState.Paused);

                if (!gameUpdateFunction.IsNull())
                {
                    while (saveData.runState != EventVMRunState.VMRunState.Stopped)
                    {
                        if (saveData.runState != EventVMRunState.VMRunState.Paused)
                        {
                            //Debug.Log("Calling 'Update'.");
                            await vm.PushCallFrameAndRun(gameUpdateFunction, 0);
                            //Debug.Log("Calling 'Update' done.");
                        }
                        
                        await UniTask.DelayFrame(5);
                    }
                }
            }
            catch (UloxException e)
            {
                platform.Exception(e.Message);
                eventRunState.Trigger(EventVMRunState.VMRunState.Stopped);
                return InterpreterResult.ERROR;
            }

            console.Log("Program finalized.");
            eventRunState.Trigger(EventVMRunState.VMRunState.Stopped);
            return InterpreterResult.OK;
        }

#endregion
    }
}