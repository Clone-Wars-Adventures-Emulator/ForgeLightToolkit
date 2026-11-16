using ForgeLightToolkit.Editor.FileTypes.Dma;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ForgeLightToolkit.Editor.FileTypes.Dme {
    public class DmeFile : ScriptableObject {
        [HideInInspector] public DmaFile DmaFile;

        public Bounds Bounds;

        [HideInInspector] public List<MeshEntry> Meshes = new();
        [HideInInspector] public List<DirectoryEntry> Directory = new();
        [HideInInspector] public List<BonePalletMapping> BonePalletMapping = new();

        // bone related stuff, all of these are together, but for some reason they are not part of a struct...
        [HideInInspector] public List<Matrix4x4> InverseGlobalBindTransforms = new();
        [HideInInspector] public List<(Vector3 min, Vector3 max)> BoneBounds = new();
        [HideInInspector] public List<uint> BoneIdentifierHashes = new();
        
        public bool Load(string filePath) {
            name = Path.GetFileNameWithoutExtension(filePath);

            var reader = new Reader(File.OpenRead(filePath));

            var magic = new string(reader.ReadChars(4));
            if (magic != "DMOD") {
                return false;
            }

            var version = reader.ReadInt32();

            DmaFile = CreateInstance<DmaFile>();
            var dmaFileData = reader.ReadBytes(reader.ReadInt32());
            if (!DmaFile.Load(filePath, dmaFileData)) {
                return false;
            }

            Bounds.SetMinMax(reader.ReadVector3(), reader.ReadVector3());

            var meshCount = DmaFile.MaterialEntries.Count;
            if (version >= 3) {
                meshCount = reader.ReadInt32();
            }

            if (meshCount != DmaFile.MaterialEntries.Count) {
                return false;
            }

            for (var i = 0; i < meshCount; i++) {
                var meshEntry = new MeshEntry {
                    MaterialIndex = reader.ReadInt32(),
                    Unknown2_DirectoryEntryCount = reader.ReadInt32(),

                    Unknown3 = reader.ReadInt32(),
                    Unknown4 = reader.ReadInt32(),

                    VertexSize = reader.ReadInt32(),
                    VertexBufferCount = reader.ReadInt32(),

                    IndexSize = reader.ReadInt32(),
                    IndexBufferCount = reader.ReadInt32()
                };

                meshEntry.VertexBuffer = reader.ReadBytes(meshEntry.VertexBufferCount * meshEntry.VertexSize);
                meshEntry.IndexBuffer = reader.ReadBytes(meshEntry.IndexBufferCount * meshEntry.IndexSize);

                if (meshEntry.MaterialIndex < DmaFile.MaterialEntries.Count) {
                    var materialEntry = DmaFile.MaterialEntries[meshEntry.MaterialIndex];

                    if (!meshEntry.CreateMesh(name, materialEntry, i)) {
                        return false;
                    }

                    Meshes.Add(meshEntry);
                }
            }

            int dirEntries = reader.ReadInt32();
            for (int i = 0; i < dirEntries; i++) {
                var dirEntry = new DirectoryEntry {
                    DrawMask = reader.ReadUInt32(),
                    BoneMappingStart = reader.ReadUInt32(),
                    BoneMappingCount = reader.ReadUInt32(),
                    BonePalletOffset = reader.ReadUInt32(),
                    Unknown = reader.ReadUInt32(),
                    VertexRangeStart = reader.ReadUInt32(),
                    VertexRangeCount = reader.ReadUInt32(),
                    IndexRangeStart = reader.ReadUInt32(),
                    IndexRangeCount = reader.ReadUInt32(),
                };
                Directory.Add(dirEntry);
            }

            int bonePalletCount = reader.ReadInt32();
            for (int i = 0; i < bonePalletCount; i++) {
                var map = new BonePalletMapping() {
                    BoneIndex = reader.ReadInt16(),
                    Value = reader.ReadInt16(),
                };
                BonePalletMapping.Add(map);
            }

            int skeleEleCount = reader.ReadInt32();
            for (int i = 0; i < skeleEleCount; i++) {
                var m00 = reader.ReadSingle();
                var m01 = reader.ReadSingle();
                var m02 = reader.ReadSingle();
                var m03 = reader.ReadSingle();
                var m10 = reader.ReadSingle();
                var m11 = reader.ReadSingle();
                var m12 = reader.ReadSingle();
                var m13 = reader.ReadSingle();
                var m20 = reader.ReadSingle();
                var m21 = reader.ReadSingle();
                var m22 = reader.ReadSingle();
                var m23 = reader.ReadSingle();

                var matrix = new Matrix4x4() {
                    m00 = m00,
                    m01 = m01,
                    m02 = m02,
                    m03 = m03,
                    m10 = m10,
                    m11 = m11,
                    m12 = m12,
                    m13 = m13,
                    m20 = m20,
                    m21 = m21,
                    m22 = m22,
                    m23 = m23,
                    m33 = 1,
                };
                InverseGlobalBindTransforms.Add(matrix);
            }
            for (int i = 0; i < skeleEleCount; i++) {
                var min = reader.ReadVector3();
                var max = reader.ReadVector3();
                BoneBounds.Add((min, max));
            }
            for (int i = 0; i < skeleEleCount; i++) {
                BoneIdentifierHashes.Add(reader.ReadUInt32());
            }

            // TODO: There is technically more data here, but im going to ignore it

            return true;
        }
    }

    [Serializable]
    public class BonePalletMapping {
        public short BoneIndex;
        public short Value;
    }

    [Serializable]
    public class DirectoryEntry {
        public uint DrawMask;
        public uint BoneMappingStart;
        public uint BoneMappingCount;
        public uint BonePalletOffset;
        public uint Unknown;
        public uint VertexRangeStart;
        public uint VertexRangeCount;
        public uint IndexRangeStart;
        public uint IndexRangeCount;
    }
}
