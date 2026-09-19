using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ForgeLightToolkit.Editor {
    public interface IShaderNameSelector {
        private static readonly List<IShaderNameSelector> selectors = new();

        [InitializeOnLoadMethod]
        private static void InitSelectors() {
            selectors.Clear();
            var isnst = typeof(IShaderNameSelector);

            HashSet<Type> goodTypes = new();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if (type.GetInterfaces().Contains(isnst)) {
                        goodTypes.Add(type);
                    }
                }
            }


            foreach (var type in goodTypes) {
                selectors.Add((IShaderNameSelector) Activator.CreateInstance(type));
            }

            selectors.Sort((a, b) => a.Weight.CompareTo(b.Weight));
        }

        internal static string GetUnityShaderForMaterial(string forgeLightMaterialName) {
            foreach (var selector in selectors) {
                var ret = selector.SelectShaderForForgeLightMaterialName(forgeLightMaterialName);
                if (ret != null) {
                    return ret;
                }
            }

            return $"FLTK/Built-In/{forgeLightMaterialName}";
        }

        // Higher runs later
        public int Weight { get; }
        /// <summary>
        /// Given a Forge Light material name, select the name of the Unity shader to use for any materials.
        /// </summary>
        /// <param name="forgeLightName">The Forge Light shader name, such as DualTextureRigid</param>
        /// <returns>The name of the unity shader to load, or null if no override is defined by this selector</returns>
        public string SelectShaderForForgeLightMaterialName(string forgeLightName);
    }
}
