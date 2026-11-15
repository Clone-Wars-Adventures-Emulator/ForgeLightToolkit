using System;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Gcnk {
    [Serializable]
    // Unused in CWA, so this wont be explored further
    public class RawGroup {
        // probably group file name
        public string Unknown;

        // probably pos and rot
        public Vector4 Unknown2;
        public Vector4 Unknown3;

        public int Unknown4;

        public void Deserialize(Reader reader) {
            Unknown = reader.ReadNullTerminatedString();

            Unknown2 = reader.ReadVector4();

            Unknown3 = reader.ReadVector4();

            Unknown4 = reader.ReadInt32();
        }
    }
}
