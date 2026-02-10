using System;
using ULox;

namespace LoxVMod
{
    public class LoxVMPlatform : IPlatform
    {
        private readonly ConnectedDevices connectedDevices;
        private readonly ScriptDictionary scriptDictionary;
        private readonly ConsoleData console;

        public LoxVMPlatform(ConnectedDevices _connectedDevices, ScriptDictionary _scriptDictionary, ConsoleData _console)
        {
            this.scriptDictionary = _scriptDictionary;
            this.console = _console;
            this.connectedDevices = _connectedDevices;
        }
        void IPlatformIO.Print(string message)
        {
            console.Log(message);
        }
        void IPlatformIO.Log(string message)
        {
            console.Log(message);
        }
        void IPlatformIO.Warn(string message)
        {
            console.Warn(message);
        }
        void IPlatformIO.Error(string message)
        {
            console.Error(message);
        }
        void IPlatformIO.Exception(string message)
        {
            console.Exception(message);
        }
        string[] IPlatformFiles.FindFiles(string inDirectory, string withPattern, bool recurse)
        {
            throw new NotImplementedException($"Platform.FindFiles : {inDirectory}|{withPattern}|{recurse}");
        }
        string IPlatformFiles.LoadFile(string filePath)
        {
            //UnityEngine.Debug.Log($"Try to get {filePath}");
            //scriptDictionary.Log();
            return scriptDictionary.GetLibraryScript(filePath);
            //throw new NotImplementedException($"Platform.LoadFile : {filePath}");
        }
        void IPlatformFiles.SaveFile(string filePath, string contents)
        {
            throw new NotImplementedException($"Platform.SaveFile : {filePath}|{contents}");
        }

        public void AddLibraries(Context executionContext)
        {
            executionContext.AddLibrary(new StdLibrary());
            executionContext.AddLibrary(new LoxStationeersLibrary(connectedDevices, console));
        }
        private float timeSlice = 0.02f;
        public float GetTimeSlice()
        {
            return timeSlice;
        }
        public void SetTimeSlice(float timeSlice)
        {
            this.timeSlice = timeSlice; 
        }
    }
}
