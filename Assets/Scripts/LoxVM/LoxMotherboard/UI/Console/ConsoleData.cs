using LoxVMod.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoxVMod
{
    public class ConsoleData : MonoBehaviour
    {
        public EventConsoleData eventConsole;

        public int maxLines = 100;

        public int Count { get => _lines.Count; }
        
        private List<string> _lines;

        public void Awake()
        {
            _lines = new List<string>();
        }
        private void Start()
        {
            eventConsole.TriggerEvent(this);
        }

        public string GetText()
        {
            string ret = string.Empty;
            int i = 1;
            foreach (var line in _lines)
            {
                ret += string.Format("<color=#808080>{0:D3}: {1}\n", i, line);
                i++;
            }
            return ret;
        }

        public void Clear ()
        { 
            _lines.Clear();
            eventConsole.TriggerEvent(this);
        }

        public void Write(string line, string color)
        {
            string[] splitString = line.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in splitString)
            {
                _lines.Add(color+item);
                if (_lines.Count > maxLines)
                {
                    _lines.RemoveAt(0);
                }
            }
            eventConsole.TriggerEvent(this);
        }
        public void Log(string text)
        {
            Write(text, ConsoleColors.Normal);
        }
        public void Error(string text)
        {
            Write(text, ConsoleColors.Error);
        }
        public void Warn(string text)
        {
            Write(text, ConsoleColors.Warn);
        }
        public void Exception(string text)
        {
            Write(text, ConsoleColors.Exception);
        }
    }
}
