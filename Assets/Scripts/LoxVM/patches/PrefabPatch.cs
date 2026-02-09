using System;
using System.Collections.ObjectModel;
using Assets.Scripts.Objects;
using HarmonyLib;
using UnityEngine;


namespace LoxVMod
{
    [HarmonyPatch]
    public class PrefabPatch
    {
        public static ReadOnlyCollection<GameObject> prefabs { get; set; }
        [HarmonyPatch(typeof(Prefab), "LoadAll")]
        public static void Prefix()
        {
            try
            { 
                Debug.Log("Prefab Patch started");
                foreach (var gameObject in prefabs)
                {
                    Debug.Log ("found Type.name : "+gameObject.name);
                    Debug.Log("------");
                    Component[] comp = gameObject.GetComponents(typeof(Component));
                    foreach (var component in comp)
                    {
                        if (component!=null)
                            Debug.Log("Name : " + component.name + " Type : "+component.GetType().ToString());
                        else 
                            Debug.LogWarning("Component is null");
                    }
                    Debug.Log("------");
                    Thing thing = gameObject.GetComponent<Thing>();
                    // Additional patching goes here, like setting references to materials(colors) or tools from the game
                    if (thing != null)
                    {
                        Debug.Log(gameObject.name + " added to WorldManager");
                        WorldManager.Instance.SourcePrefabs.Add(thing);
                    }
                    else
                    {
                        Debug.Log(gameObject.name + " is not a Thing");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Debug.LogException(ex);
            }
        }
    }
}
