using UnityEngine;

namespace ArtTherapy.Core
{
    public static class MaterialFactory
    {
        static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");
        static readonly int GlossinessId = Shader.PropertyToID("_Glossiness");

        static Material template;

        // Every runtime material copies this asset, so its shader is always included in builds.
        public static void Init(Material baseMaterial) => template = baseMaterial;

        public static Material Create(Color colour, float smoothness)
        {
            var material = new Material(template) { color = colour };
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
