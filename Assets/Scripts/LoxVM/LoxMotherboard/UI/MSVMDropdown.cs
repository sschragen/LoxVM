using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace LoxVMod
{
    public class MSVMDropdown : MonoBehaviour
    {
        [SerializeField] SOSaveData saveData;
        [SerializeField] public TMP_Dropdown dropdown;
        [SerializeField] public ScriptDictionary dict;
        [SerializeField] public int value
        { 
            get 
            { return dropdown.value; }
            set
            {
                if (value >= 0)
                {
                    dropdown.value = value;
                }
                else dropdown.value = 0;
            }
        }

#region MonoBehaviour
        private void OnEnable()
        {
            dict.Changed += OnDictChanged;
        }
        private void OnDisable()
        {
            dict.Changed -= OnDictChanged;
        }

        public void Start()
        {
            UpdateDropDownList();
            dropdown.value = dropdown.options.FindIndex(option => option.text == saveData.filename);
            dropdown.RefreshShownValue();            
        }
#endregion
        private void OnDictChanged(object Sender, ScriptDictionaryEvent ev) 
        {
            Debug.Log("OnDictChanged ....");
            UpdateDropDownList();
            dropdown.value = dropdown.options.FindIndex(option => option.text == saveData.filename);
            if (dropdown.value > 0)
                dropdown.RefreshShownValue();
        }

        public void OnValueChanged()
        {
            saveData.filename = dropdown.captionText.text;
        }
        
        private void UpdateDropDownList()
        {
            dropdown.options.Clear();
            dropdown.options.Add(new TMP_Dropdown.OptionData(""));
            foreach (var programm in dict.getProgrammList())
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(programm, null));
            }
        }
    }
}
