using Assets.Scripts;
using Assets.Scripts.GridSystem;
using Assets.Scripts.Networking;
using Assets.Scripts.Networks;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Electrical;
using Assets.Scripts.Objects.Items;
using Cysharp.Threading.Tasks;
using LoxVMod.Events;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using static WorldManager;


namespace LoxVMod
{
    

    public class LoxMotherboard : Motherboard //, ITooltip //, /*ISourceCode,*/ IReferencable, IEvaluable
    {
        public char[] SourceCodeCharArray { get; set; }
        public int SourceCodeWritePointer { get; set; }

        [Header("Lox - Motherboard", order = 0)]
        public CustomLoxVM customLoxVM;
        public SOSaveData saveData;
        public EventVMCommand eventCommand;
        public EventVMRunState eventRunState;

        public TextMeshProUGUI UI_Version;

        //private AsciiString _sourceCode = AsciiString.Empty;
        private bool _DevicesChanged;

        public void SendUpdate()
        { 
            if (NetworkManager.IsClient)
            {
                //ISourceCode.SendSourceCodeToServer(_sourceCode, base.ReferenceId);
            }
            else if (NetworkManager.IsServer)
            {
                base.NetworkUpdateFlags |= 256;
            }
        }
        public override void Awake()
        {
            base.Awake();
            ElectricityManager.Register(this);
        }
        public override void Start()
        {
            base.Start();
            //Debug.Log("WrenMotherboard Start done.");
        }
        public override void OnFinishedLoad()
        {
            base.OnFinishedLoad();
        }
        new string ToTooltip()
        {
            return new string("Lox Motherboard ...");
        }
        public override void ProcessUpdate(RocketBinaryReader reader, ushort networkUpdateType)
        {
            base.ProcessUpdate(reader, networkUpdateType);
            if (Thing.IsNetworkUpdateRequired(256U, networkUpdateType))
            {
                this._DevicesChanged = reader.ReadBoolean();
                if (this._DevicesChanged)
                {
                    this.HandleDeviceListChange().Forget();
                }
            }
            if (Thing.IsNetworkUpdateRequired(512U, networkUpdateType))
            {
                //this.SetSourceCode(reader.ReadAscii());
            }
        }
        public override void BuildUpdate(RocketBinaryWriter writer, ushort networkUpdateType)
        {
            base.BuildUpdate(writer, networkUpdateType);
            if (Thing.IsNetworkUpdateRequired(256U, networkUpdateType))
            {
                writer.WriteBoolean(this._DevicesChanged);
            }
            if (Thing.IsNetworkUpdateRequired(512U, networkUpdateType))
            {
                //writer.WriteAscii(this._sourceCode);
            }
        }
        public override void OnDeviceListChanged()
        {
            base.OnDeviceListChanged();
            if (!this._DevicesChanged)
            {
                this._DevicesChanged = true;
                this.HandleDeviceListChange().Forget();
            }
        }
        private async UniTaskVoid HandleDeviceListChange()
        {
            // modeled after ProgrammableChipMotherboard.HandleDeviceListChange to hopefully minimize errors
            if (!GameManager.IsMainThread)
                await UniTask.SwitchToMainThread();
            var cancelToken = this.GetCancellationTokenOnDestroy();
            while (GameManager.GameState != GameState.Running)
                await UniTask.NextFrame(cancelToken);
            await UniTask.NextFrame(cancelToken);
            
            if (this.ParentComputer == null || !this.ParentComputer.AsThing().isActiveAndEnabled)
            {
                return;
            }
            this._DevicesChanged = false;
            if (customLoxVM != null)
            {
                customLoxVM.connectedDevices.Update(this.ParentComputer.DeviceList());
            }
        }
        public override void OnInsertedToComputer(IComputer computer)
        {
            base.OnInsertedToComputer(computer);
            if (!_DevicesChanged)
            {
                _DevicesChanged = true;
                ShowBootScreen().Forget();
                if (GameManager.RunSimulation)
                {
                    SendUpdate();
                }
            }
        }
        public override void OnRemovedFromComputer(IComputer computer)
        {
            base.OnRemovedFromComputer(computer);
            eventCommand.Trigger(EventVMCommand.VMCommand.Stop);
            Screens[0].SetActive(false);
            Screens[1].SetActive(false);
            SendUpdate();
        }        
        public async UniTaskVoid ShowBootScreen()
        {
            Screens[0].SetActive(false);
            Screens[1].SetActive(true);
            UI_Version.text = "Version 0.7.alpha";
            await UniTask.WaitForSeconds(5f);
            Screens[0].SetActive(true);
            Screens[1].SetActive(false);
            HandleDeviceListChange().Forget();
            customLoxVM.ResetConsole();
            switch (saveData.runState)
            {
                case EventVMRunState.VMRunState.Stopped:
                    eventRunState.Trigger(saveData.runState);
                    break;
                case EventVMRunState.VMRunState.Running:
                    eventRunState.Trigger(EventVMRunState.VMRunState.Stopped);
                    eventCommand.Trigger(EventVMCommand.VMCommand.Start);
                    break;
                case EventVMRunState.VMRunState.Paused:
                    eventRunState.Trigger(saveData.runState);
                    eventCommand.Trigger(EventVMCommand.VMCommand.Compile);
                    break;
                   
            }
        }

        public override void SerializeOnJoin(RocketBinaryWriter writer)
        {
            base.SerializeOnJoin(writer);
            //writer.WriteAscii(this._sourceCode);
            
        }
        public override void DeserializeOnJoin(RocketBinaryReader reader)
        {
            Debug.Log("DeserializeOnJoin 1");
            base.DeserializeOnJoin(reader);
            //this.SetSourceCode(reader.ReadAscii());
            Debug.Log("DeserializeOnJoin 2");
             
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override bool IsOperable
        {
            get
            {
                //return this._circuitHolders.Count > 0;
                return true;
            }
        }

        #region XML Handler
        public override ThingSaveData SerializeSave()
        {
            ThingSaveData result = new LoxMotherboardSaveData();
            this.InitialiseSaveData(ref result);
            return result;
        }

        public override void DeserializeSave(ThingSaveData savedData)
        {
            base.DeserializeSave(savedData);
            if (savedData is LoxMotherboardSaveData wrenMotherboardSaveData)
            {
                saveData.runState = wrenMotherboardSaveData.runState;
                saveData.filename = wrenMotherboardSaveData.filename;
            }
        }

        protected override void InitialiseSaveData(ref ThingSaveData savedData)
        {
            base.InitialiseSaveData(ref savedData);
            Debug.Log("base.saveinit done");
            if (savedData is not LoxMotherboardSaveData msMotherboardSaveData)
            {
                return;
            }
            msMotherboardSaveData.filename = saveData.filename;
            msMotherboardSaveData.runState = saveData.runState;
            Debug.Log("savedata generated");
        }
        #endregion

        /*
        public AsciiString GetSourceCode()
        {
            return this._sourceCode;
        }

        public void SetSourceCode(string sourceCode)
        {
            if (sourceCode == null)
            {
                sourceCode = string.Empty;
            }
            this._sourceCode = AsciiString.Parse(sourceCode);
            //SourceCode = new string(sourceCode);
        }
        */
    }
}
