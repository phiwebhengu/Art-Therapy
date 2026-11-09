using System;
using System.Collections.Generic;
using ArtTherapy.Pattern;
using ArtTherapy.Pottery;
using ArtTherapy.Traditions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArtTherapy.Core
{
    public enum CraftMode { Form, Finish, Pattern }

    public class CraftSession : MonoBehaviour
    {
        const int MaxUndoSteps = 50;
        const float ShowcaseSpeed = 25f;

        readonly List<Snapshot> undoSteps = new();
        PotMesh pot;
        PotShaper shaper;
        PotFinisher finisher;
        PatternBand band;
        CraftTradition[] traditions;
        int traditionIndex;
        CraftMode mode;
        bool showcasing;

        public Camera Camera { get; private set; }

        CraftTradition Tradition => traditions[traditionIndex];

        public void Init(Camera camera, PotMesh pot, PotShaper shaper, PotFinisher finisher, PatternBand band, CraftTradition[] traditions)
        {
            Camera = camera;
            this.pot = pot;
            this.shaper = shaper;
            this.finisher = finisher;
            this.band = band;
            this.traditions = traditions;

            LoadTradition(0);
            SetMode(CraftMode.Form);
        }

        public bool TryGetPotHit(out RaycastHit hit)
        {
            hit = default;
            if (Mouse.current == null) return false;

            Ray ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            return pot.Collider.Raycast(ray, out hit, 100f);
        }

        public void RecordUndo()
        {
            undoSteps.Add(new Snapshot(new List<float>(pot.Radii), finisher.Progress, band.GetCells()));
            if (undoSteps.Count > MaxUndoSteps)
                undoSteps.RemoveAt(0);
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.fKey.wasPressedThisFrame)
                SetShowcase(!showcasing);

            if (showcasing)
            {
                pot.transform.Rotate(0f, ShowcaseSpeed * Time.deltaTime, 0f);
                return;
            }

            if (keyboard.tabKey.wasPressedThisFrame)
                SetMode((CraftMode)(((int)mode + 1) % Enum.GetValues(typeof(CraftMode)).Length));

            if (keyboard.tKey.wasPressedThisFrame)
                LoadTradition((traditionIndex + 1) % traditions.Length);

            if (keyboard.ctrlKey.isPressed && keyboard.zKey.wasPressedThisFrame)
                Undo();
        }
        void LoadTradition(int index)
        {
            traditionIndex = index;
            undoSteps.Clear();
            pot.transform.rotation = Quaternion.identity;

            shaper.SetTradition(Tradition);
            finisher.SetTradition(Tradition);
            band.SetTradition(Tradition);
            pot.SetRadii(Tradition.startingProfile);
        }

        void Undo()
        {
            if (undoSteps.Count == 0) return;

            Snapshot snapshot = undoSteps[^1];
            undoSteps.RemoveAt(undoSteps.Count - 1);

            pot.SetRadii(snapshot.Radii);
            finisher.Progress = snapshot.Finish;
            band.SetCells(snapshot.Cells);
        }

        void SetMode(CraftMode newMode)
        {
            mode = newMode;
            shaper.enabled = mode == CraftMode.Form;
            finisher.enabled = mode == CraftMode.Finish;
            band.enabled = mode == CraftMode.Pattern;
        }

        void SetShowcase(bool value)
        {
            showcasing = value;

            if (showcasing)
            {
                shaper.enabled = false;
                finisher.enabled = false;
                band.enabled = false;
            }
            else
            {
                SetMode(mode);
            }
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(16f, 16f, 420f, 240f), GUI.skin.box);
            GUILayout.Label($"{Tradition.displayName} ({Tradition.origin})   T: next tradition");
            GUILayout.Label(showcasing ? "Showcase (F to return)" : $"Mode: {mode} (Tab to switch)");

            if (!showcasing)
            {
                GUILayout.Label(mode switch
                {
                    CraftMode.Form => shaper.Hint,
                    CraftMode.Finish => finisher.Hint,
                    _ => band.Hint,
                });
            }

            GUILayout.Label("Right-drag: orbit   Scroll: zoom   Ctrl+Z: undo   F: showcase");
            GUILayout.EndArea();
        }

        readonly struct Snapshot
        {
            public readonly List<float> Radii;
            public readonly float Finish;
            public readonly int[] Cells;

            public Snapshot(List<float> radii, float finish, int[] cells)
            {
                Radii = radii;
                Finish = finish;
                Cells = cells;
            }
        }
    }
}
