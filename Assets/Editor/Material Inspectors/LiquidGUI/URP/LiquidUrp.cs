using UnityEngine;

using ShaderID = Unity.Rendering.Universal.ShaderUtils.ShaderID;
using BaseGuiType = UnityEditor.BaseShaderGUI;

namespace UnityEditor
{
    public class LiquidUrp : BaseShaderGUI
    {
        private BaseGuiType _shaderGUI;
        
        private ShaderID _shaderID = ShaderID.Unknown;


        public override void ValidateMaterial(Material material) {
            _shaderGUI = CreateInstance(material.shader);
            _shaderGUI.ValidateMaterial(material);
        }

        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties) {
            var material = materialEditorIn.target as Material;
            if (material == null) return;
            _shaderGUI = CreateInstance(material.shader);
            _shaderGUI.OnGUI(materialEditorIn, properties);
        }

        private BaseGuiType CreateInstance(Shader shader) {
            var shaderID = Unity.Rendering.Universal.ShaderUtils.GetShaderID(shader);
            if (_shaderGUI != null && _shaderID == shaderID) return _shaderGUI;
            _shaderID = shaderID;

            return new LiquidCodeGuiUrp();
        }
    }
}