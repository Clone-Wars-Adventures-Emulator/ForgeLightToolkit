using ForgeLightToolkit.Editor.FileTypes.Dma;
using ForgeLightToolkit.Editor.XmlData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Dme {
    public class DmeFile : ScriptableObject {
        [HideInInspector] public DmaFile DmaFile;

        public Bounds Bounds;

        public List<SubMesh> submeshes = new();

        public List<MeshEntry> MeshEntries = new();
        public List<SubMeshDefinition> Definitions = new();
        public List<BonePalletMapping> BonePalletMapping = new();

        // bone related stuff, all of these are together, but for some reason they are not part of a struct...
        public List<Matrix4x4> InverseGlobalBindTransforms = new();
        public List<(Vector3 min, Vector3 max)> BoneBounds = new();
        public List<uint> BoneIdentifierHashes = new();
        
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
                    SubMeshCount = reader.ReadInt32(),

                    Unknown3 = reader.ReadInt32(),
                    Unknown4 = reader.ReadInt32(),

                    VertexSize = reader.ReadInt32(),
                    VertexCount = reader.ReadInt32(),

                    IndexSize = reader.ReadInt32(),
                    IndexCount = reader.ReadInt32()
                };

                meshEntry.VertexBuffer = reader.ReadBytes(meshEntry.VertexCount * meshEntry.VertexSize);
                meshEntry.IndexBuffer = reader.ReadBytes(meshEntry.IndexCount * meshEntry.IndexSize);
                MeshEntries.Add(meshEntry);
            }

            // interesting serialization choice here tbh
            int defEntries = reader.ReadInt32();
            for (int i = 0; i < defEntries; i++) {
                var dirEntry = new SubMeshDefinition {
                    DrawMask = reader.ReadUInt32(),
                    BoneMappingStart = reader.ReadUInt32(),
                    BoneMappingCount = reader.ReadUInt32(),
                    BonePalletOffset = reader.ReadUInt32(),
                    Unknown = reader.ReadUInt32(),
                    VertexRangeStart = reader.ReadUInt32(),
                    VertexCount = reader.ReadUInt32(),
                    IndexRangeStart = reader.ReadUInt32(),
                    IndexCount = reader.ReadUInt32(),
                };
                Definitions.Add(dirEntry);
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

                // TODO: need to check if this is actually 3x3 then xyz translate
                var matrix = new Matrix4x4() {
                    m00 = m00, m01 = m01, m02 = m02, m03 = m03,
                    m10 = m10, m11 = m11, m12 = m12, m13 = m13,
                    m20 = m20, m21 = m21, m22 = m22, m23 = m23,
                    m30 = 0,   m31 = 0,   m32 = 0,   m33 = 1,
                };
                InverseGlobalBindTransforms.Add(matrix);
            }
            for (int i = 0; i < skeleEleCount; i++) {
                var min = reader.ReadVector3();
                var max = reader.ReadVector3();
                BoneBounds.Add((min, max));
            }
            // This one we may need to link the IDs from this 
            for (int i = 0; i < skeleEleCount; i++) {
                BoneIdentifierHashes.Add(reader.ReadUInt32());
            }

            // TODO: There is technically more data here, but im going to ignore it

            int createdSubmeshes = 0;
            for (int i = 0; i < MeshEntries.Count; i++) {
                var meshEntry = MeshEntries[i];
                // TODO: handle this if better
                if (meshEntry.MaterialIndex < DmaFile.MaterialEntries.Count) {
                    var materialEntry = DmaFile.MaterialEntries[meshEntry.MaterialIndex];

                    for (int j = 0; j < meshEntry.SubMeshCount; j++) {
                        var mesh = CreateMesh(meshEntry, materialEntry, Definitions[createdSubmeshes], createdSubmeshes);
                        if (mesh != null) {
                            submeshes.Add(new SubMesh() { mesh = mesh, meshEntryIdx = i });
                            createdSubmeshes++;
                        }
                    }
                }
            }

            return true;
        }

        public Mesh CreateMesh(MeshEntry meshEntry, MaterialEntry materialEntry, SubMeshDefinition subMeshDef, int meshIndex) {
            var mesh = new Mesh {
                name = $"{name}_submesh_{meshIndex:d2}",
            };

            var materialDefinition = MaterialInfo.Instance?.MaterialDefinitions.SingleOrDefault(x => x.NameHash == materialEntry.Hash);
            if (materialDefinition is null) {
                Debug.LogError("Failed to find material definition.");
                return null;
            }

            var drawStyle = materialDefinition.DrawStyles.First();
            if (drawStyle is null) {
                Debug.LogError("Material Definition doesn't contain a DrawStyle.");
                return null;
            }

            var inputLayout = MaterialInfo.Instance?.InputLayouts.SingleOrDefault(x => x.NameHash == drawStyle.InputLayoutHash);
            if (inputLayout is null) {
                Debug.LogError("Failed to find Input Layout.");
                return null;
            }

            // Vertices
            var vertices = new Vector3[subMeshDef.VertexCount];
            var positionEntry = inputLayout.Entries.SingleOrDefault(x => x.Usage == MaterialInfo.InputLayout.Entry.EntryUsage.Position);
            if (positionEntry is null) {
                Debug.LogError("Model doesn't have a position entry.");
                return null;
            }

            if (positionEntry.Type == MaterialInfo.InputLayout.Entry.EntryType.Float3) {
                for (var i = 0; i < subMeshDef.VertexCount; i++) {
                    var startIndex = positionEntry.Offset + (i + (int) subMeshDef.VertexRangeStart) * meshEntry.VertexSize;

                    var x = BitConverter.ToSingle(meshEntry.VertexBuffer, startIndex + 0);
                    var y = BitConverter.ToSingle(meshEntry.VertexBuffer, startIndex + 4);
                    var z = BitConverter.ToSingle(meshEntry.VertexBuffer, startIndex + 8);

                    vertices[i] = AxisConversion.Position(new Vector3(x, y, z));
                }
            } else {
                Debug.LogError($"Unimplemented Input Layout Type \"{positionEntry.Type}\" for Position.");
                return null;
            }
            mesh.SetVertices(vertices);

            // Normals
            var normals = new Vector3[subMeshDef.VertexCount];
            var normalEntry = inputLayout.Entries.SingleOrDefault(x => x.Usage == MaterialInfo.InputLayout.Entry.EntryUsage.Normal);
            if (normalEntry is not null) {
                if (normalEntry.Type == MaterialInfo.InputLayout.Entry.EntryType.Float3) {
                    for (var i = 0; i < subMeshDef.VertexCount; i++) {
                        var startIndex = normalEntry.Offset + (i + (int) subMeshDef.VertexRangeStart) * meshEntry.VertexSize;

                        var x = BitConverter.ToSingle(meshEntry.VertexBuffer, startIndex + 0);
                        var y = BitConverter.ToSingle(meshEntry.VertexBuffer, startIndex + 4);
                        var z = BitConverter.ToSingle(meshEntry.VertexBuffer, startIndex + 8);

                        normals[i] = AxisConversion.Position(new Vector3(x, y, z));
                    }
                } else {
                    Debug.LogError($"Unimplemented Input Layout Type \"{normalEntry.Type}\" for Normal.");
                    return null;
                }
            }
            mesh.SetNormals(normals);

            // UVs
            foreach (var texCoordEntry in inputLayout.Entries.Where(x => x.Usage == MaterialInfo.InputLayout.Entry.EntryUsage.TexCoord)) {
                var uvs = new Vector2[subMeshDef.VertexCount];

                if (texCoordEntry.Type == MaterialInfo.InputLayout.Entry.EntryType.Float2) {
                    for (var i = 0; i < subMeshDef.VertexCount; i++) {
                        var startIndex = texCoordEntry.Offset + (i + (int) subMeshDef.VertexRangeStart) * meshEntry.VertexSize;

                        var x = BitConverter.ToSingle(meshEntry.VertexBuffer, startIndex + 0);
                        var y = BitConverter.ToSingle(meshEntry.VertexBuffer, startIndex + 4);

                        uvs[i] = new Vector2(x, y);
                    }
                } else {
                    Debug.LogError($"Unimplemented Input Layout Type \"{texCoordEntry.Type}\" for TexCoord, UsageIndex {texCoordEntry.UsageIndex}.");
                    return null;
                }

                mesh.SetUVs(texCoordEntry.UsageIndex, uvs);
            }

            // mesh weights
            var weightLayout = inputLayout.Entries.SingleOrDefault(x => x.Usage == MaterialInfo.InputLayout.Entry.EntryUsage.BlendWeight);
            var weightIdxLayout = inputLayout.Entries.SingleOrDefault(x => x.Usage == MaterialInfo.InputLayout.Entry.EntryUsage.BlendIndices);
            if (weightLayout != null && weightIdxLayout != null) {
                // TODO: how should the sub mesh bone map stuff blarg affect this stuff? for now, im going to do the naieve thing and just leave it alone, but will need to revisit
                
            }

            // Indices
            var indices = new int[subMeshDef.IndexCount];
            for (var i = 0; i < subMeshDef.IndexCount; i++) {
                int indexDataPosition = (i + (int) subMeshDef.IndexRangeStart) * meshEntry.IndexSize;
                indices[i] = meshEntry.IndexSize switch {
                    2 => BitConverter.ToInt16(meshEntry.IndexBuffer, indexDataPosition),
                    4 => BitConverter.ToInt32(meshEntry.IndexBuffer, indexDataPosition),
                    _ => throw new NotImplementedException()
                };

                // Note: Submesh index values are for the whole array of vertices, NOT for the submesh. We need to subtract the vertex range start from the value to account for this
                indices[i] -= (int) subMeshDef.VertexRangeStart;
            }
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);

            return mesh;
        }
    }

    [Serializable]
    public class BonePalletMapping {
        public short BoneIndex;
        public short Value;
    }

    [Serializable]
    public class SubMeshDefinition {
        public uint DrawMask;
        public uint BoneMappingStart;
        public uint BoneMappingCount;
        // This is always 0
        public uint BonePalletOffset;
        // this always matches BoneMappingCount
        public uint Unknown;
        public uint VertexRangeStart;
        public uint VertexCount;
        public uint IndexRangeStart;
        public uint IndexCount;
    }

    [Serializable]
    public class SubMesh {
        public Mesh mesh;
        public int meshEntryIdx;
    }
}
