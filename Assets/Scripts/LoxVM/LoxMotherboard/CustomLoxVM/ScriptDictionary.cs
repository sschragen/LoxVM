using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ULox;
using UnityEngine;

namespace LoxVMod
{
    public class ScriptDictionaryEvent
    {
        public ScriptDictionaryEvent(string text) { Text = text; }
        public string Text { get; } // readonly
    }
    public class ScriptDictionary : MonoBehaviour
    {
        public delegate void ScriptDictionaryEventHandler(object sender, ScriptDictionaryEvent e);
        public event ScriptDictionaryEventHandler Changed;

        private string workingDir;
        private string libDir;
        private string programmExtension = ".lox";
        private string libraryExtension = ".loxlib";
        private string fileSearchExtension = "*.lox*";        

        private FileSystemWatcher _watcher;
        private Dictionary<string, FileInfo> programmDict = new Dictionary<string, FileInfo>();
        private Dictionary<string, FileInfo> libraryDict = new Dictionary<string, FileInfo>();
#region MonoBehaviour
        public void Awake()
        {
            workingDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Stationeers\\scripts\\Lox\\";
            libDir = workingDir + "Library\\";
            System.IO.Directory.CreateDirectory(workingDir);
            System.IO.Directory.CreateDirectory(libDir);

            RebuildDictionary();
            Debug.Log("ScriptDictionary : Awake done");
        }

        public void Start()
        {
            createFileWatcher(workingDir);
            Changed?.Invoke(this, new ScriptDictionaryEvent("Start"));
            Debug.Log("ScriptDictionary : Start done");
        }
#endregion
#region Filewatcher
        private void createFileWatcher(string folderPath)
        {
            _watcher = new FileSystemWatcher(folderPath);
            _watcher.NotifyFilter = 
                  NotifyFilters.LastWrite 
                | NotifyFilters.CreationTime 
                | NotifyFilters.Size
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;
            _watcher.Filter = fileSearchExtension;
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            _watcher.Changed += OnFileChanged;
            Debug.Log("ScriptDictionary : Filewatcher created");
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Debug.Log("onFileChanged is called");
            RebuildDictionary();
            Debug.Log("ScriptDictionary : File changed");
        }

        public void RebuildDictionary()
        {
            FileInfo[] fileList;
            DirectoryInfo dir = new DirectoryInfo(workingDir); ;
            EnumerationOptions enumerationOptions = new EnumerationOptions() {
                RecurseSubdirectories = true
            };

            programmDict.Clear();
            libraryDict.Clear();

            fileList = dir.GetFiles($"*{programmExtension}", enumerationOptions);
            foreach (FileInfo f in fileList)
            {
                if (!programmDict.ContainsKey(Path.GetFileNameWithoutExtension(f.Name.ToLower())))
                    programmDict.Add(Path.GetFileNameWithoutExtension(f.Name.ToLower()), f);
                else throw new ScriptRuntimeException($"Program Script {f.Name.ToLower()} already exists");
            }

            fileList = dir.GetFiles($"*{libraryExtension}", enumerationOptions);
            foreach (FileInfo f in fileList)
            {
                if (!libraryDict.ContainsKey(Path.GetFileNameWithoutExtension(f.Name.ToLower())))
                    libraryDict.Add(Path.GetFileNameWithoutExtension(f.Name.ToLower()), f);
                else throw new ScriptRuntimeException($"Library Script {f.Name.ToLower()} already exists");
            }
            Debug.Log("Scriptfile changed ... Invoke event");
            Changed?.Invoke(this, new ScriptDictionaryEvent("Next"));
        }
#endregion
#region public Interface
        public bool isInProgrammDict (string name)
        {
            return programmDict.ContainsKey(name.ToLower());
        }
        public bool isInLibraryDict(string name)
        {
            return libraryDict.ContainsKey(name.ToLower());
        }
        public List<string> getProgrammList()
        {
            return programmDict.Keys.ToList(); ;
        }
        public string GetProgrammScript(string name)
        {            
            if (isInProgrammDict(name))
            {
                return File.ReadAllText(programmDict[name.ToLower()].FullName);
            }
            else throw new UloxException( $"Program {name.ToLower()} NOT found");
        }
        public string GetLibraryScript(string name)
        {
            if (isInLibraryDict(name))
            {
                return File.ReadAllText(libraryDict[name.ToLower()].FullName);
            }
            else throw new UloxException( $"Library {name.ToLower()} NOT found");
        }
        public void Log()
        {
            Debug.Log("Library Dict : ");
            foreach (var item in libraryDict)
                Debug.Log($"Key : {item.Key} Fullname : {item.Value.FullName}");
            Debug.Log("Program Dict : ");
            foreach (var item in programmDict)
                Debug.Log($"Key : {item.Key} Fullname : {item.Value.FullName}");
        }
#endregion
    }
}
