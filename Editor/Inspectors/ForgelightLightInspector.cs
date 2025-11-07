using ForgeLightToolkit.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ForgeLightToolkit.Editor.Inspectors {
    [CustomEditor(typeof(ForgelightLight))]
    [CanEditMultipleObjects]
    public class ForgelightLightInspector : UnityEditor.Editor {
        public VisualTreeAsset Inspector;

        public override VisualElement CreateInspectorGUI() {
            if (Application.isPlaying) {
                return new();
            }

            VisualElement root = new();

            Inspector.CloneTree(root);

            var resetBtnEle = root.Q("reset");
            if (resetBtnEle is Button resetBtn) {
                resetBtn.clicked += resetTargettedLights;
            }

            return root;
        }

        private void resetTargettedLights() {
            Dictionary<ForgelightWorld, Dictionary<string, ForgelightLight>> worldMapping = new();

            foreach (var worldTarget in targets) {
                if (worldTarget is ForgelightLight light) {
                    var world = light.GetComponentInParent<ForgelightWorld>();

                    if (!worldMapping.TryGetValue(world, out var dict)) {
                        dict = new();
                        worldMapping.Add(world, dict);
                    }
                    if (dict.ContainsKey(light.uniqueLightId)) {
                        Debug.LogError($"Multiple ForgelightLight objects have the same id ({light.uniqueLightId}), {light.name} will not be reset", light);
                    } else {
                        dict.Add(light.uniqueLightId, light);
                    }
                }
            }

            foreach (var world in worldMapping.Keys) {
                var worldName = world.worldName;
                var gzneFile = LoadWorldWindow.FindSingularGzne(worldName);
                if (gzneFile == null) {
                    continue;
                }

                var toResetDict = worldMapping[world];
                var chunks = LoadWorldWindow.FindTilesForWorld(gzneFile);
                foreach (var chunk in chunks) {
                    foreach (var tile in chunk.Tiles) {
                        for (int i = 0; i < tile.RawLights.Count; i++) {
                            var rawLight = tile.RawLights[i];
                            string lightId = ForgelightLight.CreateUniqueLightId(tile.Coords.x, tile.Coords.y, i, rawLight.ColorName);
                            if (toResetDict.Remove(lightId, out var toUpdate)) {
                                LoadWorldWindow.SetLightProperties(toUpdate.GetComponent<Light>(), rawLight);
                            }
                        }
                    }
                }
            }
        }
    }
}
