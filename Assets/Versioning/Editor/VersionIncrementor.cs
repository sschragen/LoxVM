// Minor adjustments by Arshd and then tsny and finaly RKar
// Version Incrementor Script for Unity by Francesco Forno (Fornetto Games)
// Inspired by http://forum.unity3d.com/threads/automatic-version-increment-script.144917/

#if UNITY_EDITOR
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

[InitializeOnLoad]
class VersionIncrementor : IPreprocessBuildWithReport
{
    [MenuItem("Build/Increase Current Build Version")]
    private static void IncreaseBuild()
    {
        IncrementVersion(new int[] { 0, 0, 1 });
    }

    [MenuItem("Build/Increase Minor Version")]
    private static void IncreaseMinor()
    {
        IncrementVersion(new int[] { 0, 1, 0 });
    }

    [MenuItem("Build/Increase Major Version")]
    private static void IncreaseMajor()
    {
        IncrementVersion(new int[] { 1, 0, 0 });
    }

    static VersionIncrementor()
    {
        //If you want the scene to be fully loaded before your startup operation, 
        // for example to be able to use Object.FindObjectsOfType, you can defer your 
        // logic until the first editor update, like this:
        EditorApplication.update += RunOnce;
    }

    static void RunOnce()
    {
        EditorApplication.update -= RunOnce;
        IncreaseBuild();
        
        XElement t = XElement.Load("./Assets/About/About.xml");
        t.Element("Version").Value = PlayerSettings.bundleVersion;
        t.Save("./Assets/About/About.xml");

        Debug.Log("new Build Version : " + PlayerSettings.bundleVersion.ToString());
    }

    static void IncrementVersion(int[] versionIncr)
    {
        string[] lines = PlayerSettings.bundleVersion.Split('.');

        for (int i = lines.Length - 1; i >= 0; i--)
        {
            bool isNumber = int.TryParse(lines[i], out int numberValue);

            if (isNumber && versionIncr.Length - 1 >= i)
            {
                versionIncr[i] += numberValue;
                
            } 
        }

        PlayerSettings.bundleVersion = versionIncr[0] + "." + versionIncr[1] + "." + versionIncr[2];

        

    } 

    public static string GetLocalVersion()
    {
        return PlayerSettings.bundleVersion.ToString();
    }
    /*
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        IncreaseBuild();
    }
    */
    public void OnPreprocessBuild(BuildReport report)
    {
        IncreaseBuild();
    }

    public int callbackOrder { get { return 0; } }
}
#endif