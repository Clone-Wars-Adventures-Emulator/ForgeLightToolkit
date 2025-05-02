using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes {
    public class CdtFile : ScriptableObject {
        public enum EnumUnknownEnum : uint {
            Unknown = 0,
            Default = 0x01,
            _0x02 = 0x02,
            _0x03 = 0x03,
            _0x05 = 0x05,
            _0x0A = 0x0A,
            _0x26 = 0x26,
            _0x27 = 0x27,
            _0x28 = 0x28,
            _0x29 = 0x29,
            _0x2A = 0x2A,
            _0x2B = 0x2B,
            _0x2C = 0x2C,
            HighBidDefault = 0x80000001
        }

        public List<Vector3> verticies = new();
        public List<int> indices = new();
        public EnumUnknownEnum unknownEnum;
        public Mesh colliderMesh;

        public bool Load(string filePath, byte[] buffer) {
            name = Path.GetFileNameWithoutExtension(filePath);

            var reader = new Reader(buffer);

            return LoadInternal(reader);
        }

        public bool Load(string filePath) {
            name = Path.GetFileNameWithoutExtension(filePath);

            var reader = new Reader(File.OpenRead(filePath));

            return LoadInternal(reader);
        }

        private bool LoadInternal(Reader reader) {
            var magic = new string(reader.ReadChars(4));

            if (magic != "CDTA") {
                return false;
            }

            var version = reader.ReadUInt32();

            if (version != 1) {
                return false;
            }

            unknownEnum = (EnumUnknownEnum) reader.ReadUInt32();

            var numEntries = reader.ReadUInt32();
            if (numEntries != 1) {
                Debug.LogError($"Technically, the CDT file spec supports more than one entry ({name} has {numEntries}), but FLTK doesnt right now.");
                return false;
            }

            var alwaysZero = reader.ReadUInt32();

            var vertexCount = reader.ReadUInt32();
            for (int i = 0; i < vertexCount; i++) {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();

                verticies.Add(new Vector3(x, y, z));
            }

            var triangleCount = reader.ReadUInt32();
            for (int i = 0; i < triangleCount; i++) {
                indices.Add(reader.ReadUInt16());
                indices.Add(reader.ReadUInt16());
                indices.Add(reader.ReadUInt16());
            }

            colliderMesh = new Mesh();
            colliderMesh.SetVertices(verticies);
            colliderMesh.SetIndices(indices, MeshTopology.Triangles, 0);

            // TODO: possible future, read what ever bullet physics data goes right here

            return true;
        }
    }
}
