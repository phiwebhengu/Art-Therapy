using ArtTherapy.Pattern;
using ArtTherapy.Pottery;
using ArtTherapy.Traditions;
using UnityEngine;

namespace ArtTherapy.Core
{
    public class CraftBootstrap : MonoBehaviour
    {
        static readonly Color FloorColour = new(0.42f, 0.33f, 0.2f);

        [Tooltip("Leave empty to use the built-in placeholder traditions.")]
        [SerializeField] CraftTradition[] traditions;

        void Awake()
        {
            if (traditions == null || traditions.Length == 0)
                traditions = DefaultTraditions.Create();

            Camera camera = Camera.main != null ? Camera.main : new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();

            if (FindAnyObjectByType<Light>() == null)
            {
                var sun = new GameObject("Sun").AddComponent<Light>();
                sun.type = LightType.Directional;
                sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = Vector3.one * 0.3f;
            floor.GetComponent<Renderer>().sharedMaterial = MaterialFactory.Create(FloorColour, 0.05f);

            var pot = new GameObject("Pot").AddComponent<PotMesh>();
            Material clay = MaterialFactory.Create(Color.white, 0.1f);
            pot.GetComponent<MeshRenderer>().sharedMaterial = clay;

            var shaper = gameObject.AddComponent<PotShaper>();
            var finisher = gameObject.AddComponent<PotFinisher>();
            var band = gameObject.AddComponent<PatternBand>();
            var session = gameObject.AddComponent<CraftSession>();

            var orbit = camera.GetComponent<OrbitCamera>();
            if (orbit == null)
                orbit = camera.gameObject.AddComponent<OrbitCamera>();

            orbit.Init(pot);
            shaper.Init(session, pot);
            finisher.Init(session, clay);
            band.Init(session, pot);
            session.Init(camera, pot, shaper, finisher, band, traditions);
        }
    }
}
