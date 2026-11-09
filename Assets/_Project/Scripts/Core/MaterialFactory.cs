using UnityEngine;

namespace ArtTherapy.Core
{
    public static class MaterialFactory
    {
        static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");
        static readonly int GlossinessId = Shader.PropertyToID("_Glossiness");

        public static Material Create(Color colour, float smoothness)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { color = colour };
            SetSmoothness(material, smoothness);
            return material;
        }
        public static void SetSmoothness(Material material, float smoothness)
        {
            material.SetFloat(SmoothnessId, smoothness);
            material.SetFloat(GlossinessId, smoothness);
        }
    }
}
