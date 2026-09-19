using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Dma {
    [Serializable]
    public class MaterialEntry {
        /// <summary>
        /// This maps into one of the NameHash fields of MaterialInfo.Instance.MaterialDefinitions
        /// </summary>
        public uint Hash;

        public string Name;

        public List<ParameterEntry> ParameterEntries = new();

        public void Deserialize(Reader reader, string contextMaterial) {
            Hash = reader.ReadUInt32();

            var mds = MaterialInfo.Instance?.MaterialDefinitions.Where(md => md.NameHash == Hash).ToArray();
            if ((mds?.Length ?? 0) == 0) {
                Debug.Log($"Could not find a MaterialDefinition for the hash {Hash} in {contextMaterial}");
            } else {
                // There should only be one, but im not going to run an assertion for that...
                Name = mds[0].Name;
            }

            var parameterCount = reader.ReadInt32();

            for (var i = 0; i < parameterCount; i++) {
                var parameterEntry = new ParameterEntry();

                parameterEntry.Deserialize(reader);

                ParameterEntries.Add(parameterEntry);
            }
        }
    }
}
