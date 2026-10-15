using System;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Gzne {
    [Serializable]
    public class FloraData {
        public Vector3 Position;

        public byte Unknown;

        public string Unknown2;

        public void Deserialize(Reader reader) {
            Position = reader.ReadVector3();

            Unknown = reader.ReadByte();

            Unknown2 = reader.ReadNullTerminatedString();
        }
    }
}
