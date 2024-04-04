using GUIStreamline;
using UnityEngine;

namespace UnityEditor
{
    // This class acts as the main GUI script that determines what sub gui is needed (eg. URP, HDRP, Legacy, Shadergraph, etc)
    public class LiquidGui : ShaderGUI
    {
        // Abstract class to derive from for defining custom GUI for shader properties and for extending the material preview.
        private ShaderGUI _shaderGUI;

        // When the user loads a Material using this ShaderGUI into memory or changes a value in the Inspector, the Editor calls this method.
        public override void ValidateMaterial(Material material)
        {
            // The null-coalescing assignment operator ??= assigns the value of its right-hand operand to its left-hand operand
            // only if the left-hand operand evaluates to null. 
            _shaderGUI ??= CreateInstance(material);
            _shaderGUI.ValidateMaterial(material);
        }

        // To define a custom shader GUI use the methods of materialEditor to controls for the properties array.
        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
        {
            var material = materialEditorIn.target as Material;
            if (material == null) return;
            _shaderGUI ??= CreateInstance(material);
            _shaderGUI.OnGUI(materialEditorIn, properties);
        }

        
        private static ShaderGUI CreateInstance(Material material)
        {
            return new LiquidUrp();

            /*
            if (TagUtils.IsUrp(material))
            {
                return new ChromaUrp();
            }

            return new ChromaCodeGui();
            */
            
        }
    }
}
