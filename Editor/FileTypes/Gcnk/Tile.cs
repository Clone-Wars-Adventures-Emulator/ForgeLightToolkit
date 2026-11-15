using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Gcnk {
    [Serializable]
    public class Tile {
        private int _version;

        public Vector2Int Coords;

        public uint RandomSeed;
        public uint EnvironmentIndex;
        public float TerrainHeightDefault1;
        public uint EnabledFlag;

        public int BoundsEnabled;
        public Vector4 OptionalVolume;

        public float TerrainHeightDefault2;

        public List<int> EcoDataList = new();

        public List<RuntimeObject> RuntimeObjects = new();

        public List<RawLight> RawLights = new();

        public List<RawArea> RawAreas = new();

        public List<RawGroup> RawGroups = new();

        public int Index;

        public Tile(int version) {
            _version = version;
        }

        public void Deserialize(Reader reader) {
            Coords.x = reader.ReadInt32();
            Coords.y = reader.ReadInt32();

            RandomSeed = reader.ReadUInt32();
            EnvironmentIndex = reader.ReadUInt32();
            TerrainHeightDefault1 = reader.ReadSingle();
            EnabledFlag = reader.ReadUInt32();

            BoundsEnabled = reader.ReadInt32();
            if (BoundsEnabled > 0) {
                OptionalVolume = reader.ReadVector4();
            }

            TerrainHeightDefault2 = reader.ReadSingle();

            var ecoDataCount = reader.ReadInt32();

            for (var i = 0; i < ecoDataCount; i++) {
                EcoDataList.Add(reader.ReadInt32());

                // What?
                if (!reader.IsLittleEndian) {
                    var unknown = reader.ReadInt32();

                    if (unknown > 0) {
                        reader.Skip(12 * unknown);
                    }
                }
            }

            var runtimeObjectCount = reader.ReadInt32();
            for (var i = 0; i < runtimeObjectCount; i++) {
                var runtimeObject = new RuntimeObject(_version);
                runtimeObject.Deserialize(reader);

                RuntimeObjects.Add(runtimeObject);
            }

            var rawLightCount = reader.ReadInt32();
            for (var i = 0; i < rawLightCount; i++) {
                var rawLight = new RawLight();
                rawLight.Deserialize(reader);

                RawLights.Add(rawLight);
            }

            var rawAreaCount = reader.ReadInt32();
            for (var i = 0; i < rawAreaCount; i++) {
                var rawArea = new RawArea();
                rawArea.Deserialize(reader);

                RawAreas.Add(rawArea);
            }

            var rawGroupCount = reader.ReadInt32();
            for (var i = 0; i < rawGroupCount; i++) {
                var rawGroup = new RawGroup();
                rawGroup.Deserialize(reader);

                RawGroups.Add(rawGroup);
            }

            Index = reader.ReadInt32();

            reader.Skip(4);
        }
    }
}
