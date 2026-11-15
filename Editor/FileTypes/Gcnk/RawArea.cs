using System;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Gcnk {
    [Serializable]
    // Annoyingly, while CWA uses areas, they arent serialized in the tiles
    public class RawArea {
        public string Name;

        public int AreaType;

        public string Unknown3;

        // probably pos and rot
        public Vector4 Unknown4;
        public Vector4 Unknown5;

        public float Unknown6;

        public Vector3 Unknown7;

        public void Deserialize(Reader reader) {
            Name = reader.ReadNullTerminatedString();

            AreaType = reader.ReadInt32();

            Unknown3 = reader.ReadNullTerminatedString();
            Debug.Log($"Unknown3: {Unknown3}");

            // possibly min and max? will need to investigate (cant verify, no data)
            Unknown4 = reader.ReadVector4();
            Unknown5 = reader.ReadVector4();

            // one source has this as a u32 of bit flags, but given that CWA doesnt have data, cant verify
            Unknown6 = reader.ReadSingle();

            Unknown7 = reader.ReadVector3();
        }
    }
}
