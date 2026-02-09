using Assets.Scripts.UI;
using UnityEngine;

namespace LoxVMod
{
    public class CustomWireframe : Wireframe
    {   
        public void Awake()
        {
            Renderer meshRenderer = GetComponent<Renderer>();
            meshRenderer.material.shader = Shader.Find("Unlit/AlphaSelfIllum");
            meshRenderer.material.color = Color.green;
        }
    }
}
