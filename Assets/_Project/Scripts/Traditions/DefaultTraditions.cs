using UnityEngine;

namespace ArtTherapy.Traditions
{
    // Placeholder traditions so the prototype runs with zero setup.
    // Each one still needs review by someone from that tradition before it ships, Gonna need to bother Chu lol.
    public static class DefaultTraditions
    {
        public static CraftTradition[] Create() => new[] { Ukhamba(), BlueAndWhite(), Freeform() };

        static CraftTradition Ukhamba()
        {
            var tradition = ScriptableObject.CreateInstance<CraftTradition>();
            tradition.name = "Ukhamba";
            tradition.displayName = "Ukhamba";
            tradition.origin = "Zulu, Southern Africa";
            tradition.sourceNote = "Coil-built without a wheel, burnished, decorated with raised amasumpa and beadwork colours.";
            tradition.formMethod = FormMethod.Coil;
            tradition.startingProfile = new[] { 0.22f, 0.3f, 0.36f, 0.4f, 0.42f, 0.43f };
            tradition.finishHint = "Rub the pot to burnish it";
            tradition.rawColour = new Color(0.66f, 0.4f, 0.27f);
            tradition.finishedColour = new Color(0.22f, 0.13f, 0.09f);
            tradition.finishedSmoothness = 0.85f;
            tradition.patternStyle = PatternStyle.Raised;
            tradition.bandHeight = 0.66f;
            tradition.palette = new[]
            {
                new PatternSwatch("Clay (amasumpa)", new Color(0.4f, 0.24f, 0.16f)),
                new PatternSwatch("White", new Color(0.95f, 0.94f, 0.9f)),
                new PatternSwatch("Black", new Color(0.08f, 0.08f, 0.08f)),
                new PatternSwatch("Red", new Color(0.78f, 0.12f, 0.12f)),
                new PatternSwatch("Blue", new Color(0.13f, 0.32f, 0.75f)),
                new PatternSwatch("Green", new Color(0.15f, 0.55f, 0.25f)),
                new PatternSwatch("Yellow", new Color(0.95f, 0.78f, 0.15f)),
                new PatternSwatch("Pink", new Color(0.93f, 0.45f, 0.62f)),
            };
            return tradition;
        }

        static CraftTradition BlueAndWhite()
        {
            var tradition = ScriptableObject.CreateInstance<CraftTradition>();
            tradition.name = "BlueAndWhite";
            tradition.displayName = "Blue-and-white porcelain";
            tradition.origin = "China, East Asia";
            tradition.sourceNote = "Wheel-thrown porcelain, painted with cobalt blue under a clear glaze.";
            tradition.formMethod = FormMethod.Wheel;
            tradition.startingProfile = new[] { 0.18f, 0.24f, 0.3f, 0.33f, 0.33f, 0.3f };
            tradition.finishHint = "Brush on the clear glaze";
            tradition.rawColour = new Color(0.82f, 0.81f, 0.77f);
            tradition.finishedColour = new Color(0.95f, 0.96f, 0.98f);
            tradition.finishedSmoothness = 0.92f;
            tradition.patternStyle = PatternStyle.Painted;
            tradition.bandHeight = 0.75f;
            tradition.palette = new[]
            {
                new PatternSwatch("Cobalt", new Color(0.08f, 0.2f, 0.55f)),
                new PatternSwatch("Pale cobalt", new Color(0.4f, 0.55f, 0.8f)),
            };
            return tradition;
        }

        static CraftTradition Freeform()
        {
            var tradition = ScriptableObject.CreateInstance<CraftTradition>();
            tradition.name = "Freeform";
            tradition.displayName = "Freeform";
            tradition.origin = "Your own";
            tradition.sourceNote = "No tradition, no rules. The player's own expression.";
            tradition.formMethod = FormMethod.Coil;
            tradition.finishHint = "Rub the pot to polish it";
            tradition.rawColour = new Color(0.7f, 0.6f, 0.5f);
            tradition.finishedColour = new Color(0.55f, 0.45f, 0.38f);
            tradition.finishedSmoothness = 0.6f;
            tradition.patternStyle = PatternStyle.Raised;
            tradition.bandHeight = 0.5f;
            tradition.palette = new[]
            {
                new PatternSwatch("White", Color.white),
                new PatternSwatch("Black", new Color(0.08f, 0.08f, 0.08f)),
                new PatternSwatch("Red", new Color(0.85f, 0.2f, 0.2f)),
                new PatternSwatch("Orange", new Color(0.95f, 0.55f, 0.15f)),
                new PatternSwatch("Yellow", new Color(0.95f, 0.85f, 0.2f)),
                new PatternSwatch("Green", new Color(0.25f, 0.65f, 0.3f)),
                new PatternSwatch("Teal", new Color(0.15f, 0.6f, 0.6f)),
                new PatternSwatch("Blue", new Color(0.2f, 0.35f, 0.85f)),
                new PatternSwatch("Purple", new Color(0.55f, 0.3f, 0.75f)),
            };
            return tradition;
        }
    }
}
