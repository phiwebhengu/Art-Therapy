using ArtTherapy.Core;
using ArtTherapy.Traditions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArtTherapy.Pottery
{
    public class PotShaper : MonoBehaviour
    {
        public const int MaxRings = 24;

        [SerializeField] float sensitivity = 0.0015f;
        [SerializeField] float falloff = 0.1f;
        [SerializeField] float wheelSpeed = 90f;

        CraftSession session;
        PotMesh pot;
        FormMethod formMethod;
        bool shaping;
        float grabHeight;
        float outwardSign;

        public string Hint => formMethod == FormMethod.Wheel
            ? $"Space: pull the wall up ({pot.RingCount}/{MaxRings})\nDrag on the pot: push the wall out or in"
            : $"Space: add a coil ({pot.RingCount}/{MaxRings})\nDrag on the pot: push the wall out or in";

        public void Init(CraftSession session, PotMesh pot)
        {
            this.session = session;
            this.pot = pot;
        }

        public void SetTradition(CraftTradition tradition) => formMethod = tradition.formMethod;

        void Update()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;
            if (mouse == null || keyboard == null) return;

            if (formMethod == FormMethod.Wheel)
                pot.transform.Rotate(0f, wheelSpeed * Time.deltaTime, 0f);

            if (keyboard.spaceKey.wasPressedThisFrame && pot.RingCount < MaxRings)
            {
                session.RecordUndo();
                pot.AddRing();
            }

            if (mouse.leftButton.wasPressedThisFrame && session.TryGetPotHit(out RaycastHit hit))
            {
                session.RecordUndo();
                shaping = true;
                grabHeight = pot.transform.InverseTransformPoint(hit.point).y;

                // Dragging away from the pot's centre line widens it, towards it narrows it.
                Vector3 centre = session.Camera.WorldToScreenPoint(pot.transform.TransformPoint(0f, grabHeight, 0f));
                outwardSign = mouse.position.ReadValue().x >= centre.x ? 1f : -1f;
            }

            if (!mouse.leftButton.isPressed)
                shaping = false;

            float dragX = mouse.delta.ReadValue().x;
            if (shaping && dragX != 0f)
                pot.Push(grabHeight, dragX * outwardSign * sensitivity, falloff);
        }

        void OnDisable() => shaping = false;
    }
}
