using System;
using GUIStreamline;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace UnityEditor
{
    public class LiquidCodeGuiUrp : BaseShaderGUI
    {
        private readonly StreamlineDrawers _drawers = new StreamlineDrawers();
        
        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties) {
            if (materialEditorIn == null) throw new ArgumentNullException("materialEditorIn");

            materialEditor = materialEditorIn;
            Material material = materialEditor.target as Material;

            // The check for SG is needed because in Unity 2021.1- this class is used for SG.
            if (!material.shader.IsShaderGraphAsset()) {
                FindProperties(properties);
            }

            if (m_FirstTimeApply) {
                OnOpenGUI(material, materialEditorIn);
                m_FirstTimeApply = false;
            }

            StreamlinePropertyDrawer.DrawProperties(properties, materialEditor, _drawers, DefaultPropertyDrawer.Draw);

            // Draw the default shader properties.
            EditorGUILayout.Space(10);
            materialEditor.RenderQueueField();
            materialEditor.EnableInstancingField();
            materialEditor.DoubleSidedGIField();
            // The check for SG is needed because in Unity 2021.1- this class is used for SG.
            if (!material.shader.IsShaderGraphAsset()) {
                DrawEmissionProperties(material, true);
            }
        }
    }
}
