using System;
using System.Collections.Generic;
using Assets.Scripts.Serialization;
using HarmonyLib;

namespace LoxVMod
{
    [HarmonyPatch]
    public class SaveDataPatch
    {
        [HarmonyPatch(typeof(XmlSaveLoad), "AddExtraTypes")]
        public static void Prefix(ref List<Type> extraTypes)
        {
            extraTypes.Add(typeof(LoxMotherboardSaveData));
        }
    }
}
