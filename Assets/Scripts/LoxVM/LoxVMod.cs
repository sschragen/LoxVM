using BepInEx.Configuration;
using HarmonyLib;
using StationeersMods.Interface;
using System;
using System.IO;
// https://crc32.online/
namespace LoxVMod
{

    [StationeersMod("LoxVMod", "LoxVMod", "0.7.x")]
    public class LoxVMod : ModBehaviour
    {
        private ConfigEntry<bool> configBool;

        public override void OnLoaded(ContentHandler contentHandler)
        {
            UnityEngine.Debug.Log("LoxVMod says: Hello World!");
            /*
            string destx86_64  = Environment.CurrentDirectory + "\\rocketstation_Data\\Plugins\\x86_64\\";
            string destManaged = Environment.CurrentDirectory + "\\rocketstation_Data\\Managed\\";
            string src = Path.GetDirectoryName(GetType().Assembly.Location) + "\\GameData\\Plugins\\Wren\\";
            if (!File.Exists(destx86_64+ "wren_unity.dll")) 
                File.Copy(src + "wren_unity.so", destx86_64 + "wren_unity.dll");

            if (!File.Exists(destManaged + "WrenSharp.Unity.dll"))
                File.Copy(src + "WrenSharp.Unity.dll", destManaged + "WrenSharp.Unity.dll");

            if (!File.Exists(destManaged + "WrenSharp.Lib.Unity.dll"))
                File.Copy(src + "WrenSharp.Lib.Unity.dll", destManaged + "WrenSharp.Lib.Unity.dll");

            UnityEngine.Debug.Log("WrenEngine is now in place and ready.");
            */
            //var monkey = BundleManager.GetAssetBundle("monkey").LoadAsset<Mesh>("monkey");

            //Config example
            configBool = Config.Bind("Input",
                    "Boolean",
                    true,
                    "Boolean description");

            Harmony harmony = new Harmony("LoxVMod");
            PrefabPatch.prefabs = contentHandler.prefabs;
            harmony.PatchAll();
            foreach (var item in contentHandler.prefabs)
            {
                UnityEngine.Debug.Log("LoxVMod " + item.ToString());
            }
            UnityEngine.Debug.Log("LoxVMod Loaded with " + contentHandler.prefabs.Count + " prefab(s)");
        }
    }
}

