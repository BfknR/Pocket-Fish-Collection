using System.Collections.Generic;
using PocketFish.Domain.Progression;
using PocketFish.Game.Data;
using PocketFish.Game.Persistence;
using PocketFish.Game.Progression;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PocketFish.Editor
{
    /// <summary>
    /// Testing tool: edit the player's saved coins, aquariums and upgrade levels, and clear saves.
    /// Outside Play mode it edits what is saved on disk. In Play mode it edits the running game's progress.
    /// </summary>
    public sealed class SaveDataWindow : EditorWindow
    {
        // Same keys the stores use. Kept here so the stores don't have to expose them.
        private const string CatchLogKey = "catches.v1";
        private const string PondLegacyKey = "pond.v1";
        private const string PondKeyPrefix = "pond.v1.";

        private AquariumCatalog _catalog;
        private PlayerProgress _target;
        private int _coins;
        private int _owned;
        private int _selected;
        private int[][] _levels;
        private VisualElement _body;

        [MenuItem("PocketFish/Save Data")]
        private static void Open()
        {
            var window = GetWindow<SaveDataWindow>();
            window.titleContent = new GUIContent("Save Data");
            window.minSize = new Vector2(360, 400);
        }

        private void OnEnable() => EditorApplication.playModeStateChanged += OnPlayModeChanged;

        private void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayModeChanged;

        private void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode || change == PlayModeStateChange.EnteredEditMode) Rebuild();
        }

        public void CreateGUI()
        {
            if (_catalog == null) _catalog = FindCatalog();

            var catalogField = new ObjectField("Aquarium Catalog") { objectType = typeof(AquariumCatalog), value = _catalog };
            catalogField.RegisterValueChangedCallback(e =>
            {
                _catalog = (AquariumCatalog)e.newValue;
                Rebuild();
            });
            rootVisualElement.Add(catalogField);

            var scroll = new ScrollView();
            _body = scroll;
            rootVisualElement.Add(scroll);
            Rebuild();
        }

        private static AquariumCatalog FindCatalog()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:AquariumCatalog"))
            {
                return AssetDatabase.LoadAssetAtPath<AquariumCatalog>(AssetDatabase.GUIDToAssetPath(guid));
            }
            return null;
        }

        /// <summary>Reloads the values from the saved or live progress and redraws everything.</summary>
        private void Rebuild()
        {
            if (_body == null) return;
            _body.Clear();

            if (_catalog == null)
            {
                _body.Add(new HelpBox("Assign an Aquarium Catalog above.", HelpBoxMessageType.Info));
                return;
            }

            // In Play mode the running game owns the progress, so edit that. Otherwise edit the save.
            if (Application.isPlaying)
            {
                _target = ProgressSession.Get(_catalog);
            }
            else
            {
                _target = new PlayerProgress(_catalog.ToDomainList());
                ProgressStore.Load(_target);
            }
            ReadFromTarget();

            _body.Add(new HelpBox(Application.isPlaying
                ? "Play mode: Apply changes the running game's progress and saves it."
                : "Editing the saved data on disk. Changes show up the next time the game starts.",
                Application.isPlaying ? HelpBoxMessageType.Warning : HelpBoxMessageType.Info));

            BuildProgressSection();
            BuildQuickSection();
            BuildDangerSection();
        }

        private void ReadFromTarget()
        {
            _coins = _target.Coins;
            _owned = _target.OwnedCount;
            _selected = _target.SelectedIndex;
            _levels = new int[_target.Aquariums.Count][];
            for (int a = 0; a < _levels.Length; a++)
            {
                _levels[a] = new int[_target.Aquariums[a].Upgrades.Count];
                for (int u = 0; u < _levels[a].Length; u++) _levels[a][u] = _target.LevelOf(a, u);
            }
        }

        private void BuildProgressSection()
        {
            var box = Section("Progress");

            var coins = new IntegerField("Coins") { value = _coins };
            coins.RegisterValueChangedCallback(e => _coins = Mathf.Max(0, e.newValue));
            box.Add(coins);

            var coinButtons = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            foreach (int amount in new[] { 100, 1000, 10000 })
            {
                int add = amount;
                coinButtons.Add(Button($"+{add:N0}", () => { _coins += add; coins.SetValueWithoutNotify(_coins); }));
            }
            box.Add(coinButtons);

            int count = _target.Aquariums.Count;
            var owned = new SliderInt("Aquariums owned", 1, count) { value = _owned, showInputField = true };
            owned.RegisterValueChangedCallback(e => _owned = e.newValue);
            box.Add(owned);

            var selected = new SliderInt("Selected aquarium", 0, count - 1) { value = _selected, showInputField = true };
            selected.RegisterValueChangedCallback(e => _selected = e.newValue);
            box.Add(selected);

            for (int a = 0; a < count; a++)
            {
                int aquarium = a;
                var foldout = new Foldout { text = $"{a}: {_catalog.Aquariums[a].DisplayName}", value = a == _selected };
                for (int u = 0; u < _levels[a].Length; u++)
                {
                    int upgrade = u;
                    int max = _target.Aquariums[a].Upgrades[u].MaxLevel;
                    var slider = new SliderInt(_catalog.Aquariums[a].Upgrades[u].DisplayName, 0, max)
                    {
                        value = _levels[a][u],
                        showInputField = true
                    };
                    slider.RegisterValueChangedCallback(e => _levels[aquarium][upgrade] = e.newValue);
                    foldout.Add(slider);
                }
                box.Add(foldout);
            }

            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 6 } };
            row.Add(Button("Apply", Apply));
            row.Add(Button("Revert", Rebuild));
            box.Add(row);
        }

        private void BuildQuickSection()
        {
            var box = Section("Quick actions (apply immediately)");

            box.Add(Button("Max all upgrades in owned aquariums", () =>
            {
                for (int a = 0; a < _owned; a++)
                {
                    for (int u = 0; u < _levels[a].Length; u++) _levels[a][u] = _target.Aquariums[a].Upgrades[u].MaxLevel;
                }
                Apply();
            }));

            box.Add(Button("Unlock every aquarium (max all before the last)", () =>
            {
                _owned = _target.Aquariums.Count;
                for (int a = 0; a < _owned - 1; a++)
                {
                    for (int u = 0; u < _levels[a].Length; u++) _levels[a][u] = _target.Aquariums[a].Upgrades[u].MaxLevel;
                }
                Apply();
            }));

            box.Add(Button("Reset progress (coins, aquariums, upgrades)", () =>
            {
                _coins = 0;
                _owned = 1;
                _selected = 0;
                for (int a = 0; a < _levels.Length; a++) _levels[a] = new int[_levels[a].Length];
                Apply();
            }));
        }

        private void BuildDangerSection()
        {
            var box = Section("Clear saves");

            box.Add(Button("Clear pond saves (all aquariums)", () =>
            {
                if (!Confirm("Clear pond saves", "Deletes every pond's saved fish and spawn timer.")) return;
                PlayerPrefs.DeleteKey(PondLegacyKey);
                foreach (AquariumSpec aquarium in _target.Aquariums) PlayerPrefs.DeleteKey(PondKeyPrefix + aquarium.Id);
                PlayerPrefs.Save();
            }));

            box.Add(Button("Clear catch log (collection)", () =>
            {
                if (!Confirm("Clear catch log", "Deletes every recorded catch, so the Collection Book starts empty.")) return;
                PlayerPrefs.DeleteKey(CatchLogKey);
                PlayerPrefs.Save();
            }));

            box.Add(Button("Delete ALL saved data", () =>
            {
                if (!Confirm("Delete all saved data", "Deletes progress, ponds and the catch log.")) return;
                PlayerPrefs.DeleteKey(CatchLogKey);
                PlayerPrefs.DeleteKey(PondLegacyKey);
                foreach (AquariumSpec aquarium in _target.Aquariums) PlayerPrefs.DeleteKey(PondKeyPrefix + aquarium.Id);
                _coins = 0;
                _owned = 1;
                _selected = 0;
                for (int a = 0; a < _levels.Length; a++) _levels[a] = new int[_levels[a].Length];
                Apply();
            }));

            if (Application.isPlaying)
            {
                box.Add(new HelpBox("Pond and catch log saves are loaded when a scene starts. Reload the scene to see them cleared.", HelpBoxMessageType.Info));
            }
        }

        /// <summary>Writes the edited values to the progress, saves, and redraws.</summary>
        private void Apply()
        {
            var levels = new List<IReadOnlyList<int>>();
            foreach (int[] row in _levels) levels.Add(row);
            _target.Restore(_coins, _owned, _selected, levels);

            if (Application.isPlaying) ProgressSession.Save();
            else ProgressStore.Save(_target);

            Rebuild();
        }

        private static bool Confirm(string title, string message) => EditorUtility.DisplayDialog(title, message, "Delete", "Cancel");

        private VisualElement Section(string title)
        {
            var box = new VisualElement
            {
                style =
                {
                    marginTop = 8, marginLeft = 4, marginRight = 4, paddingTop = 6, paddingBottom = 6,
                    paddingLeft = 6, paddingRight = 6, borderTopLeftRadius = 4, borderTopRightRadius = 4,
                    borderBottomLeftRadius = 4, borderBottomRightRadius = 4,
                    backgroundColor = new Color(0f, 0f, 0f, 0.15f)
                }
            };
            box.Add(new Label(title) { style = { unityFontStyleAndWeight = FontStyle.Bold, marginBottom = 4 } });
            _body.Add(box);
            return box;
        }

        private static Button Button(string text, System.Action onClick) =>
            new Button(onClick) { text = text, style = { flexGrow = 1 } };
    }
}
