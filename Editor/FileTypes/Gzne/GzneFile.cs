using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Gzne {
    public class GzneFile : ScriptableObject {
        public bool HideTerrain;

        public int ChunkSize;
        public int TileSize;

        public int TilePerChunkAxis => ChunkSize / TileSize;

        // I think this was misunderstood by FLTK, need to find out what this actually is and how its used
        public float WorldSize;

        // Always 8, except for RailsTest, when its a nonsensical value.....
        public int Unknown5;

        public int MinChunkX;
        public int MinChunkZ;
        public int MaxChunkX;
        public int MaxChunkZ;

        // this is likely some CRC or some other counter or hash for validation
        public int Unknown10;

        public List<EcoData> EcoData = new();

        // Not used by CWA
        public List<FloraDefinition> FloraDefinitions = new();

        public List<Mesh> wallMeshes = new();

        public bool Load(string filePath) {
            name = Path.GetFileNameWithoutExtension(filePath);

            var reader = new Reader(File.OpenRead(filePath));

            var magic = new string(reader.ReadChars(4));

            if (magic != "GZNE") {
                return false;
            }

            var version = reader.ReadInt32();

            if (version > 3) {
                throw new NotSupportedException($"Cannot process file with version {version}");
            }

            if (version >= 3) {
                HideTerrain = (reader.ReadInt32() & 1) == 1;
            }

            ChunkSize = reader.ReadInt32();
            TileSize = reader.ReadInt32();

            WorldSize = reader.ReadSingle();
            Unknown5 = reader.ReadInt32();
            MinChunkX = reader.ReadInt32();
            MinChunkZ = reader.ReadInt32();
            MaxChunkX = reader.ReadInt32();
            MaxChunkZ = reader.ReadInt32();

            // This is supposedly unused
            Unknown10 = reader.ReadInt32();

            var ecoDataCount = reader.ReadInt32();

            for (var i = 0; i < ecoDataCount; i++) {
                var ecoData = new EcoData();

                ecoData.Deserialize(reader);

                EcoData.Add(ecoData);
            }

            var floraDefinitionCount = reader.ReadInt32();

            for (var i = 0; i < floraDefinitionCount; i++) {
                var floraDefinition = new FloraDefinition();

                floraDefinition.Deserialize(reader);

                FloraDefinitions.Add(floraDefinition);
            }

            if (version >= 2) {
                var invisibleWallCount = reader.ReadInt32();

                if (invisibleWallCount > 10000) {
                    throw new IndexOutOfRangeException($"Invalid number of Invisible Walls: {invisibleWallCount}");
                }

                for (var i = 0; i < invisibleWallCount; i++) {
                    var invisibleWallVertexCount = reader.ReadInt32();

                    var invisibleWallVertices = new List<Vector3>();
                    for (var j = 0; j < invisibleWallVertexCount; j++) {
                        invisibleWallVertices.Add(AxisConversion.Position(reader.ReadVector3()));
                    }

                    // unfortunately, ForgeLight saved these walls a TriangleStrip meshes (see https://en.wikipedia.org/wiki/Triangle_strip)
                    // and unity our beloved doesnt have a way to generate a mesh form this format out of the box, so we have to deconstruct that outselves
                    // given the hypothetical triangle strip
                    // 0---2---4
                    // |  /|  /|
                    // | / | / |
                    // |/  |/  |
                    // 1---3---5
                    // the indexes in unity's format would be (0, 1, 2), (2, 1, 3), (2, 3, 4), (4, 3, 5).
                    // This can be generalized to a strip with [3,n] verticies, where the faces are constructed by looping through the verticies starting at
                    // index 2 (the third vertex) and alternating between the faces (idx - 2, idx - 1, idx) and (idx - 1, idx - 2, idx)
                    var idxs = new List<int>();
                    for (int idx = 2; idx < invisibleWallVertices.Count; idx++) {
                        if (idx % 2 == 0) {
                            idxs.Add(idx - 2);
                            idxs.Add(idx - 1);
                            idxs.Add(idx);
                        } else {
                            idxs.Add(idx - 1);
                            idxs.Add(idx - 2);
                            idxs.Add(idx);
                        }
                    }
                    var reversedIndcies = new List<int>(idxs);
                    reversedIndcies.Reverse();

                    var mesh = new Mesh {
                        name = $"{name}_InvisibleWall_{i}"
                    };
                    mesh.SetVertices(invisibleWallVertices);
                    mesh.SetIndices(reversedIndcies, MeshTopology.Triangles, 0);
                    wallMeshes.Add(mesh);
                }
            }

            return true;
        }
    }
}
