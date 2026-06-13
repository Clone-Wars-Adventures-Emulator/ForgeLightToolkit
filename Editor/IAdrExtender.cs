using ForgeLightToolkit.Editor.FileTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ForgeLightToolkit.Editor {
    public interface IAdrExtender {
        private static readonly List<IAdrExtender> extenders = new();

        [InitializeOnLoadMethod]
        public static void InitExtenders() {
            extenders.Clear();
            var iaet = typeof(IAdrExtender);

            HashSet<Type> goodTypes = new();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if (type.GetInterfaces().Contains(iaet)) {
                        goodTypes.Add(type);
                    }
                }
            }


            foreach (var type in goodTypes) {
                extenders.Add((IAdrExtender) Activator.CreateInstance(type));
            }

            extenders.Sort((a, b) => a.Weight.CompareTo(b.Weight));
        }

        internal static void ApplyExtenstionsToPrefab(GameObject prefabInstance, AdrFile adrFile) {
            foreach (var extender in extenders) {
                extender.ExtendAdrPrefab(prefabInstance, adrFile);
            }
        }

        // Higher runs later
        public int Weight { get; }
        /// <summary>
        /// Extend the prefabInstance GameObject with data specific to the given adr file just before it is saved as a prefab
        /// </summary>
        /// <param name="prefabInstance">The GameObject instance that is about to be saved as a prefab that should be extended with custom data</param>
        /// <param name="adrFile">The adr data instance from the FLTK parsers</param>
        public void ExtendAdrPrefab(GameObject prefabInstance, AdrFile adrFile);
    }
}
