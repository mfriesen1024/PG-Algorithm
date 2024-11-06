using UnityEngine;
using Colour = UnityEngine.Color;

namespace Assets
{
    internal class Block : MonoBehaviour
    {
        private BlockType type;

        public BlockType Type { get => type; set { type = value; OnSetType(); } }

        private void OnSetType()
        {
            var renderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            Material standardShaderMaterial = renderer.material;

            switch (type)
            {
                case BlockType.Dirt: renderer.material.color = Colour.HSVToRGB(0.166f, 0.75f, 0.5f); break;
                case BlockType.Grass: renderer.material.color = Colour.green; break;
                case BlockType.Water:
                    Colour c = Colour.blue;
                    c.a = 0.3f;
                    renderer.material.color = c;

                    // Yoinked this from somewhere to make it transparent.
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;

                    if (TryGetComponent(out BoxCollider b)) { b.enabled = false; }
                    break;
            }
        }
    }
}
