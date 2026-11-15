using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Gzne {
    [Serializable]
    public class EcoData {
        public int Index;

        public string Name;
        public string Texture;

        public int Scale;

        public List<FloraData> FloraData = new();

        public void Deserialize(Reader reader) {
            Index = reader.ReadInt32();

            Name = reader.ReadNullTerminatedString();
            Texture = reader.ReadNullTerminatedString();

            Scale = reader.ReadInt32();

            // This will always be 0 in CWA
            var floraDataCount = reader.ReadInt32();
            for (var i = 0; i < floraDataCount; i++) {
                var floraData = new FloraData();

                floraData.Deserialize(reader);

                FloraData.Add(floraData);
            }
        }
    }
}
