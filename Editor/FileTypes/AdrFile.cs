#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes {
    public class AdrFile : ScriptableObject {
        // TODO: use the Actor Tool Export, logic, facts, and knowledge (here in my garage) to parse as much data as possible from the ADRs in FLTK
        public enum EnumPrimaryDataType : byte {
            Unknown = 0,
            SkeletonData = 1,
            ModelData = 2,
            PossiblyParticleEmitterDataArray = 3,
            CollisionData = 0x0D,
        }

        public enum EnumModelDataFieldType : byte {
            Unknown = 0,
            ModelFile = 1,
            MaterialFile = 2,
            UpdateRadius = 3,
        }

        public enum EnumParticleDataFieldType : byte {
            Unknown = 0,
            EffectId = 1,
            EmitterName = 2,
            XmlFile = 3,
        }

        public string skeletonFileName;

        public string modelFileName;
        public string materialFileName;
        public float updateRadius;

        public string collisionFile;

        public List<ParticleEmitterDefinition> particleEmitterDefinitions = new();

        public bool Load(string filePath) {
            name = Path.GetFileNameWithoutExtension(filePath);

            var reader = new Reader(File.OpenRead(filePath));

            while (!reader.ReachedEnd) {
                var definitionType = (EnumPrimaryDataType) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();
                var definitionData = reader.ReadBytes(definitionSize);

                // java syntax here would be broken dude, like think how clean it would look if you could do:
                // switch {
                //   EnumPrimaryDataType.SkeletonData -> ParseSkeletonData(definitionData, filePath);
                // }
                switch (definitionType) {
                    case EnumPrimaryDataType.SkeletonData:
                        ParseSkeletonData(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.ModelData:
                        ParseModelDefinition(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.CollisionData:
                        ParseCollisionDefinition(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.PossiblyParticleEmitterDataArray:
                        ParseParticleEmitterDataArray(definitionData, filePath);
                        break;
                }
            }

            return true;
        }

        private void ParseSkeletonData(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        skeletonFileName = reader.ReadNullTerminatedString();
                        break;
                    default:
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }

        private void ParseModelDefinition(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = (EnumModelDataFieldType) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumModelDataFieldType.ModelFile:
                        modelFileName = reader.ReadNullTerminatedString();
                        break;
                    case EnumModelDataFieldType.MaterialFile:
                        materialFileName = reader.ReadNullTerminatedString();
                        break;
                    case EnumModelDataFieldType.UpdateRadius:
                        byte[] bigEndianUpdateRadBytes = reader.ReadBytes(4);
                        updateRadius = BitConverter.ToSingle(BitConverter.IsLittleEndian ? bigEndianUpdateRadBytes.Reverse().ToArray() : bigEndianUpdateRadBytes);
                        break;
                    default:
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }

        private void ParseParticleEmitterDataArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 2:
                        var defn = ParseParticleEmitterData(reader.ReadBytes(definitionSize), adrFilePath);
                        particleEmitterDefinitions.Add(defn);
                        break;
                    default:
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }

        private ParticleEmitterDefinition ParseParticleEmitterData(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var defn = new ParticleEmitterDefinition();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumParticleDataFieldType) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumParticleDataFieldType.EffectId:
                        // TODO: this type
                        defn.id = reader.ReadInt32();
                        break;
                    case EnumParticleDataFieldType.EmitterName:
                        defn.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumParticleDataFieldType.XmlFile:
                        defn.effectFileName = reader.ReadNullTerminatedString();
                        break;
                    default:
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return defn;
        }

        private void ParseCollisionDefinition(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        collisionFile = reader.ReadNullTerminatedString();
                        break;

                    default:
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }

        // TODO: organize this code better
        [Serializable]
        public class ParticleEmitterDefinition {
            public int id;
            public string name;
            public string effectFileName;
        }
    }
}
