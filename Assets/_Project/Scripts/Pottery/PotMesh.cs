using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArtTherapy.Pottery
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class PotMesh : MonoBehaviour
    {
        public const float RingHeight = 0.05f;
        public const float MinRadius = 0.12f;
        public const float MaxRadius = 0.8f;

        [SerializeField] int segments = 48;
        [SerializeField] float wallThickness = 0.035f;

        readonly List<float> radii = new();
        Mesh mesh;

        public event Action Changed;

        public MeshCollider Collider { get; private set; }
        public IReadOnlyList<float> Radii => radii;
        public int RingCount => radii.Count;
        public float Height => (radii.Count - 1) * RingHeight;

        void Awake()
        {
            mesh = new Mesh { name = "Pot" };
            GetComponent<MeshFilter>().sharedMesh = mesh;
            Collider = GetComponent<MeshCollider>();
        }

        public void SetRadii(IEnumerable<float> values)
        {
            radii.Clear();
            radii.AddRange(values);
            Rebuild();
        }

        public void AddRing()
        {
            radii.Add(radii[^1]);
            Rebuild();
        }

        public float GetRadiusAtHeight(float y)
        {
            float t = Mathf.Clamp(y / RingHeight, 0f, radii.Count - 1);
            int i = Mathf.Min((int)t, radii.Count - 2);
            return Mathf.Lerp(radii[i], radii[i + 1], t - i);
        }

        // Pushes the wall in or out around a height, fading off smoothly so it feels like hands on clay.
        public void Push(float y, float amount, float falloff)
        {
            for (int i = 0; i < radii.Count; i++)
            {
                float distance = (i * RingHeight - y) / falloff;
                float weight = Mathf.Exp(-distance * distance);
                radii[i] = Mathf.Clamp(radii[i] + amount * weight, MinRadius, MaxRadius);
            }

            Rebuild();
        }

        void Rebuild()
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            int top = radii.Count - 1;

            int outerStart = vertices.Count;
            for (int i = 0; i < radii.Count; i++)
                AddRing(vertices, radii[i], i * RingHeight);

            int innerStart = vertices.Count;
            for (int i = 0; i < radii.Count; i++)
                AddRing(vertices, InnerRadius(i), Mathf.Max(i * RingHeight, wallThickness));

            int rimOuter = AddRing(vertices, radii[top], Height);
            int rimInner = AddRing(vertices, InnerRadius(top), Height);

            int baseCentre = vertices.Count;
            vertices.Add(Vector3.zero);
            int baseRing = AddRing(vertices, radii[0], 0f);

            int floorCentre = vertices.Count;
            vertices.Add(new Vector3(0f, wallThickness, 0f));
            int floorRing = AddRing(vertices, InnerRadius(0), wallThickness);

            for (int i = 0; i < top; i++)
            {
                for (int s = 0; s < segments; s++)
                {
                    int next = (s + 1) % segments;
                    int row = i * segments;
                    AddQuad(triangles, outerStart + row + s, outerStart + row + segments + s, outerStart + row + segments + next, outerStart + row + next);
                    AddQuad(triangles, innerStart + row + s, innerStart + row + next, innerStart + row + segments + next, innerStart + row + segments + s);
                }
            }

            for (int s = 0; s < segments; s++)
            {
                int next = (s + 1) % segments;
                AddQuad(triangles, rimOuter + s, rimInner + s, rimInner + next, rimOuter + next);
                triangles.AddRange(new[] { baseCentre, baseRing + s, baseRing + next });
                triangles.AddRange(new[] { floorCentre, floorRing + next, floorRing + s });
            }

            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Collider.sharedMesh = null;
            Collider.sharedMesh = mesh;

            Changed?.Invoke();
        }

        float InnerRadius(int ring) => Mathf.Max(radii[ring] - wallThickness, 0.02f);

        int AddRing(List<Vector3> vertices, float radius, float y)
        {
            int start = vertices.Count;
            for (int s = 0; s < segments; s++)
            {
                float angle = s * Mathf.PI * 2f / segments;
                vertices.Add(new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius));
            }

            return start;
        }

        static void AddQuad(List<int> triangles, int a, int b, int c, int d)
        {
            triangles.AddRange(new[] { a, b, c, a, c, d });
        }
    }
}
