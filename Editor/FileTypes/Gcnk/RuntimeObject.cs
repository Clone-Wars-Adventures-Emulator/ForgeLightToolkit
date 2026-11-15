using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Gcnk {
    [Serializable]
    public class RuntimeObject {
        private readonly int _version;

        public int ObjectId; // TODO

        public string FileName;
        public string Unknown3;

        public Vector4 Position;
        public Quaternion Rotation;

        public float Scale;

        public string MaterialName;
        public string TintAlias;

        public Vector4 Unknown9;

        public int SpawnContext;
        public int Unknown11;

        public List<byte[]> Unknown12 = new();

        public RuntimeObject(int version) {
            _version = version;
        }

        public override string ToString() {
            return $"{nameof(ObjectId)}: {ObjectId}, " +
                   $"{nameof(FileName)}: {FileName}, " +
                   $"{nameof(Position)}: {Position}, " +
                   $"{nameof(Rotation)}: {Rotation}, " +
                   $"{nameof(Scale)}: {Scale}, " +
                   $"{nameof(MaterialName)}: {MaterialName}, " +
                   $"{nameof(TintAlias)}: {TintAlias}, " +
                   $"{nameof(Unknown9)}: ({Unknown9.x}, {Unknown9.y}, {Unknown9.z}), " +
                   $"{nameof(SpawnContext)}: {SpawnContext}, " +
                   $"{nameof(Unknown11)}: {Unknown11}";
        }

        public void Deserialize(Reader reader) {
            ObjectId = reader.ReadInt32();

            FileName = reader.ReadNullTerminatedString();
            Unknown3 = reader.ReadNullTerminatedString();

            Position = AxisConversion.Position(reader.ReadVector4());
            Rotation = AxisConversion.Rotation(reader.ReadVector4());

            Scale = reader.ReadSingle();

            // none of the CWA Tiles are version 6, so we dont care to check if this is right
            if (_version >= 6) {
                MaterialName = reader.ReadNullTerminatedString();
                TintAlias = reader.ReadNullTerminatedString();

                if (!string.IsNullOrEmpty(TintAlias)) {
                    Unknown9 = reader.ReadVector4();
                }
            }

            reader.Skip(4);

            if (_version > 4) {
                SpawnContext = reader.ReadInt32();
            } else {
                var unknown = reader.ReadNullTerminatedString();
                Debug.Log($"Unknown RuntimeObject Value: {unknown}");
            }

            Unknown11 = reader.ReadInt32();

            if (_version > 2) {
                var unknownCount = reader.ReadInt32();

                if (unknownCount > 0) {
                    for (var i = 0; i < unknownCount; i++) {
                        var unknown12Size = reader.ReadInt32();

                        Unknown12.Add(reader.ReadBytes(unknown12Size * 4));
                    }
                }
            }
        }
    }
}
