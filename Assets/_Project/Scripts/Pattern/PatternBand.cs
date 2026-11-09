using System;
using ArtTherapy.Core;
using ArtTherapy.Pottery;
using ArtTherapy.Traditions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArtTherapy.Pattern
{
    // A band of marks around the pot. Each tradition decides the palette and whether marks are raised or painted.
    public class PatternBand : MonoBehaviour
    {
        const int Empty = -1;
        const int MaxPaletteKeys = 9;

        [SerializeField] int rows = 3;
        [SerializeField] int columns = 36;
        [SerializeField] float rowSpacing = 0.045f;
        [SerializeField] float markSize = 0.04f;

        CraftSession session;
        PotMesh pot;
        int[] cells;
        Renderer[] marks;
        Material[] materials = Array.Empty<Material>();
        PatternSwatch[] palette = Array.Empty<PatternSwatch>();
        PatternStyle style;
        float bandHeight;
        int selected;
        bool painting;

        public string Hint =>
            $"Click or drag on the band to place\n1-{Mathf.Min(palette.Length, MaxPaletteKeys)}: palette   0: erase\nSelected: {SelectedName}";

        string SelectedName => selected == Empty ? "Erase" : palette[selected].name;

        public void Init(CraftSession session, PotMesh pot)
        {
            this.session = session;
            this.pot = pot;

            cells = new int[rows * columns];
            Array.Fill(cells, Empty);

            marks = new Renderer[cells.Length];
            for (int i = 0; i < marks.Length; i++)
            {
                var mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mark.name = $"Mark_{i:000}";
                Destroy(mark.GetComponent<Collider>());
                mark.transform.SetParent(pot.transform, false);
                marks[i] = mark.GetComponent<Renderer>();
                marks[i].enabled = false;
            }

            pot.Changed += Refresh;
        }

        public void SetTradition(CraftTradition tradition)
        {
            foreach (Material material in materials)
                Destroy(material);

            style = tradition.patternStyle;
            bandHeight = tradition.bandHeight;
            palette = tradition.palette;
            materials = new Material[palette.Length];
            for (int i = 0; i < palette.Length; i++)
                materials[i] = MaterialFactory.Create(palette[i].colour, style == PatternStyle.Painted ? 0.9f : 0.6f);

            selected = 0;
            Array.Fill(cells, Empty);

            if (pot.RingCount > 1)
                Refresh();
        }

        public int[] GetCells() => (int[])cells.Clone();

        public void SetCells(int[] values)
        {
            values.CopyTo(cells, 0);
            for (int i = 0; i < cells.Length; i++)
                UpdateMark(i);
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            for (int i = 0; i < Mathf.Min(palette.Length, MaxPaletteKeys); i++)
            {
                if (keyboard[Key.Digit1 + i].wasPressedThisFrame)
                    selected = i;
            }

            if (keyboard.digit0Key.wasPressedThisFrame)
                selected = Empty;

            if (mouse.leftButton.wasPressedThisFrame && session.TryGetPotHit(out _))
            {
                session.RecordUndo();
                painting = true;
            }

            if (!mouse.leftButton.isPressed)
                painting = false;

            if (painting && session.TryGetPotHit(out RaycastHit hit) && TryGetCell(hit.point, out int cell) && cells[cell] != selected)
            {
                cells[cell] = selected;
                UpdateMark(cell);
            }
        }

        void OnDisable() => painting = false;

        void OnDestroy()
        {
            if (pot != null)
                pot.Changed -= Refresh;
        }

        void Refresh()
        {
            bool painted = style == PatternStyle.Painted;
            Vector3 scale = painted ? new Vector3(markSize * 1.3f, markSize * 0.12f, markSize * 1.3f) : Vector3.one * markSize;
            float lift = painted ? 0.002f : markSize * 0.2f;

            for (int row = 0; row < rows; row++)
            {
                float y = RowHeight(row);
                float radius = pot.GetRadiusAtHeight(y) + lift;

                for (int column = 0; column < columns; column++)
                {
                    int i = row * columns + column;
                    float angle = ColumnAngle(row, column);
                    var outward = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

                    Transform mark = marks[i].transform;
                    mark.localPosition = outward * radius + Vector3.up * y;
                    mark.localRotation = Quaternion.FromToRotation(Vector3.up, outward);
                    mark.localScale = scale;
                    UpdateMark(i);
                }
            }
        }

        bool TryGetCell(Vector3 worldPoint, out int cell)
        {
            cell = Empty;
            Vector3 local = pot.transform.InverseTransformPoint(worldPoint);

            if (new Vector2(local.x, local.z).magnitude < pot.GetRadiusAtHeight(local.y) - 0.02f)
                return false;

            int row = Mathf.RoundToInt((local.y - RowHeight(0)) / rowSpacing);
            if (row < 0 || row >= rows)
                return false;

            float angle = Mathf.Atan2(local.z, local.x);
            if (angle < 0f)
                angle += Mathf.PI * 2f;

            int column = Mathf.RoundToInt(angle * columns / (Mathf.PI * 2f) - RowOffset(row));
            column = (column % columns + columns) % columns;

            cell = row * columns + column;
            return true;
        }

        float RowHeight(int row) => pot.Height * bandHeight + (row - (rows - 1) * 0.5f) * rowSpacing;
        float RowOffset(int row) => row % 2 * 0.5f;

        float ColumnAngle(int row, int column) => (column + RowOffset(row)) * Mathf.PI * 2f / columns;

        void UpdateMark(int i)
        {
            bool filled = cells[i] != Empty && cells[i] < materials.Length;
            marks[i].enabled = filled;

            if (filled)
                marks[i].sharedMaterial = materials[cells[i]];
        }
    }
}
