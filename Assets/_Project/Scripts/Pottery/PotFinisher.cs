using ArtTherapy.Core;
using ArtTherapy.Traditions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArtTherapy.Pottery
{
    // Rubbing the pot moves it from raw clay to the tradition's finish: burnish, glaze or polish.
    public class PotFinisher : MonoBehaviour
    {
        [SerializeField] float rate = 0.0004f;

        CraftSession session;
        Material material;
        CraftTradition tradition;
        float progress;
        bool rubbing;

        public string Hint => $"{tradition.finishHint} ({progress:P0})";

        public float Progress
        {
            get => progress;
            set
            {
                progress = Mathf.Clamp01(value);
                Apply();
            }
        }

        public void Init(CraftSession session, Material material)
        {
            this.session = session;
            this.material = material;
        }

        public void SetTradition(CraftTradition tradition)
        {
            this.tradition = tradition;
            Progress = 0f;
        }

        void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame && session.TryGetPotHit(out _))
            {
                session.RecordUndo();
                rubbing = true;
            }

            if (!mouse.leftButton.isPressed)
                rubbing = false;

            if (rubbing && session.TryGetPotHit(out _))
                Progress += mouse.delta.ReadValue().magnitude * rate;
        }

        void OnDisable() => rubbing = false;

        void Apply()
        {
            if (material == null || tradition == null) return;

            material.color = Color.Lerp(tradition.rawColour, tradition.finishedColour, progress);
            MaterialFactory.SetSmoothness(material, Mathf.Lerp(0.1f, tradition.finishedSmoothness, progress));
        }
    }
}
