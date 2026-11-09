using System;
using UnityEngine;

namespace ArtTherapy.Traditions
{
    public enum FormMethod { Coil, Wheel }

    public enum PatternStyle { Raised, Painted }

    [Serializable]
    public struct PatternSwatch
    {
        public string name;
        public Color colour;

        public PatternSwatch(string name, Color colour)
        {
            this.name = name;
            this.colour = colour;
        }
    }

    // One craft tradition as data. Adding a culture means adding an asset, not code.
    [CreateAssetMenu(menuName = "Art Therapy/Craft Tradition", fileName = "CraftTradition")]
    public class CraftTradition : ScriptableObject
    {
        [Header("Identity")]
        public string displayName;
        public string origin;
        [TextArea] public string sourceNote;

        [Header("Form")]
        public FormMethod formMethod;
        public float[] startingProfile = { 0.25f, 0.32f, 0.36f, 0.38f, 0.38f, 0.37f };

        [Header("Finish")]
        public string finishHint = "Rub the pot to finish it";
        public Color rawColour = new(0.66f, 0.4f, 0.27f);
        public Color finishedColour = new(0.3f, 0.16f, 0.1f);
        [Range(0f, 1f)] public float finishedSmoothness = 0.8f;

        [Header("Pattern")]
        public PatternStyle patternStyle;
        [Range(0f, 1f)] public float bandHeight = 0.66f;
        public PatternSwatch[] palette;
    }
}
