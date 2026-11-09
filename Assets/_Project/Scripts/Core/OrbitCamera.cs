using ArtTherapy.Pottery;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArtTherapy.Core
{
    public class OrbitCamera : MonoBehaviour
    {
        [SerializeField] float orbitSpeed = 0.25f;
        [SerializeField] float zoomStep = 0.1f;
        [SerializeField] float minDistance = 0.8f;
        [SerializeField] float maxDistance = 4f;
        [SerializeField] float minPitch = -5f;
        [SerializeField] float maxPitch = 80f;

        PotMesh target;
        float yaw;
        float pitch = 25f;
        float distance = 2f;

        public void Init(PotMesh target) => this.target = target;

        void LateUpdate()
        {
            if (target == null) return;

            var mouse = Mouse.current;
            if (mouse != null)
            {
                if (mouse.rightButton.isPressed)
                {
                    Vector2 delta = mouse.delta.ReadValue();
                    yaw += delta.x * orbitSpeed;
                    pitch = Mathf.Clamp(pitch - delta.y * orbitSpeed, minPitch, maxPitch);
                }

                float scroll = mouse.scroll.ReadValue().y;
                if (scroll != 0f)
                    distance = Mathf.Clamp(distance * (1f - Mathf.Sign(scroll) * zoomStep), minDistance, maxDistance);
            }

            Vector3 focus = target.transform.position + Vector3.up * target.Height * 0.5f;
            transform.position = focus + Quaternion.Euler(pitch, yaw, 0f) * new Vector3(0f, 0f, -distance);
            transform.LookAt(focus);
        }
    }
}
