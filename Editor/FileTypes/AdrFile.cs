// print out all instances of unknown ADR fields
#define DEBUG_ADR_PARSING
// print out all instances of 0xFE tags in
// #define DEBUG_ALL_FE_INSTANCES
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes {
    public class AdrFile : ScriptableObject {
        public enum EnumPrimaryDataType : byte {
            Unknown = 0x00,
            SkeletonData = 0x01,
            ModelData = 0x02,
            ParticleEmitterArray = 0x03,
            TextureAliases = 0x05,
            AnimationDataArray = 0x09,
            AnimationSounds = 0x0A,
            AnimationParticlesArray = 0x0B,
            AnimationActoinPoints = 0x0C,
            CollisionData = 0x0D,
        }

        public enum EnumModelDataFieldType : byte {
            Unknown = 0,
            ModelFile = 1,
            MaterialFile = 2,
            UpdateRadius = 3,
        }

        public string skeletonFileName;

        public string modelFileName;
        public string materialFileName;
        public float updateRadius;

        public string collisionFile;

        public readonly List<ParticleEmitterDefinition> particleEmitterDefinitions = new();
        public readonly List<TextureAlias> textureAliases = new();
        public readonly List<AnimationData> animations = new();
        public readonly List<AnimationSounds> animationSounds = new();
        public readonly List<AnimationParticles> animationParticles = new();
        public readonly List<AnimationActionPoints> animationActionPoints = new();

        public bool Load(string filePath) {
            name = Path.GetFileNameWithoutExtension(filePath);

            var reader = new Reader(File.OpenRead(filePath));

            while (!reader.ReachedEnd) {
                var definitionType = (EnumPrimaryDataType) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();
                var definitionData = reader.ReadBytes(definitionSize);

                switch (definitionType) {
                    case EnumPrimaryDataType.SkeletonData:
                        ParseSkeletonData(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.ModelData:
                        ParseModelDefinition(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.ParticleEmitterArray:
                        ParseParticleEmitterDataArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.TextureAliases:
                        ParseTextureAliasArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.AnimationDataArray:
                        ParseAnimationDataArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.AnimationSounds:
                        ParseAnimationSoundsArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.AnimationParticlesArray:
                        ParseAnimationParticlesArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.AnimationActoinPoints:
                        ParseAnimationActionPointsArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.CollisionData:
                        ParseCollisionDefinition(definitionData, filePath);
                        break;
                    case (EnumPrimaryDataType) 0xFE:
                        // TODO: this marker shows up in a lot of places and doesnt make sense where and why and what it serves
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found at top level in {filePath}");
#endif
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled top level adr type 0x{definitionType:X} for {filePath}");
#endif
                        break;
                }
            }

            return true;
        }

        #region ADR Type 0x01, Skeleton Data
        private void ParseSkeletonData(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        skeletonFileName = reader.ReadNullTerminatedString();
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Skeleton Data in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for skeleton data: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x02, Model Definition
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
                        updateRadius = reader.ReadAdrFloat();
                        break;
                    case (EnumModelDataFieldType) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Model Data in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for Model data: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x03, ParticleEmitter
        public enum EnumParticleDataFieldType : byte {
            Unknown = 0,
            EffectId = 1,
            EmitterName = 2,
            XmlFile = 3,
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
                    case (EnumParticleDataFieldType) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Particle Emitter in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for ParticleEmitter: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return defn;
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
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Particle Emitter array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for ParticleEmitter data array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x05, Texture Alias
        public enum EnumTextureAliasField : byte {
            SemanticHash = 3,
            AliasName = 4,
            TextureName = 5,
            AliasIsDefault = 7,
        }
        private TextureAlias ParseTextureAlias(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var alias = new TextureAlias();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumTextureAliasField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumTextureAliasField.SemanticHash:
                        alias.hash = reader.ReadUInt32();
                        break;
                    case EnumTextureAliasField.AliasName:
                        alias.aliasName = reader.ReadNullTerminatedString();
                        break;
                    case EnumTextureAliasField.TextureName:
                        alias.texture = reader.ReadNullTerminatedString();
                        break;
                    case EnumTextureAliasField.AliasIsDefault:
                        alias.aliasIsDefault = reader.ReadBool();
                        break;
                    case (EnumTextureAliasField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in TextureAlias in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for TextureAlias: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return alias;
        }

        private void ParseTextureAliasArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var alias = ParseTextureAlias(reader.ReadBytes(definitionSize), adrFilePath);
                        textureAliases.Add(alias);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in TextureAlias Array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for TextureAlias array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x09, Animation Data
        public enum EnumAnimationDataField : byte {
            Name = 1,
            FileName = 2,
            Duration = 4,
            LoadType = 5,
        }
        private AnimationData ParseAnimationData(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var anim = new AnimationData();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumAnimationDataField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumAnimationDataField.Name:
                        anim.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumAnimationDataField.FileName:
                        anim.fileName = reader.ReadNullTerminatedString();
                        break;
                    case EnumAnimationDataField.Duration:
                        anim.duration = reader.ReadAdrFloat();
                        break;
                    case EnumAnimationDataField.LoadType:
                        anim.loadType = reader.ReadByte();
                        break;
                    case  (EnumAnimationDataField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationData in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationData: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return anim;
        }

        private void ParseAnimationDataArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var anim = ParseAnimationData(reader.ReadBytes(definitionSize), adrFilePath);
                        animations.Add(anim);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationData array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationData array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        // This is shared across a couple different ADR types, so it doesnt get put into a region
        public TriggerEvent ParseTriggerEvent(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var trigger = new TriggerEvent();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        trigger.start = definitionSize == 1 ? reader.ReadByte() : reader.ReadAdrFloat();
                        break;
                    case 2:
                        trigger.end = definitionSize == 1 ? reader.ReadByte() : reader.ReadAdrFloat();
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in TriggerEvent in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for TriggerEvent: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return trigger;
        }

        #region ADR Type 0x0A, Animation Sounds Data
        public enum EnumAnimationSoundField : byte {
            TriggerEvents = 1,
            Type = 2,
            Name = 3,
            ToolName = 4,
            Id = 5,
            PlayOnce = 6,
        }
        private AnimationSoundEntry ParseAnimationSound(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var sound = new AnimationSoundEntry();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumAnimationSoundField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumAnimationSoundField.Name:
                        sound.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumAnimationSoundField.ToolName:
                        sound.toolName = reader.ReadNullTerminatedString();
                        break;
                    case EnumAnimationSoundField.Type:
                        sound.effectType = reader.ReadByte();
                        break;
                    case EnumAnimationSoundField.Id:
                        sound.id = reader.ReadUInt16();
                        break;
                    case EnumAnimationSoundField.PlayOnce:
                        sound.playOnce = reader.ReadBool();
                        break;
                    case EnumAnimationSoundField.TriggerEvents:
                        sound.events.Add(ParseTriggerEvent(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case (EnumAnimationSoundField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationSoundEntry in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationSoundEntry: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return sound;
        }

        private AnimationSounds ParseAnimationSounds(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var sounds = new AnimationSounds();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // name
                    case 2:
                        sounds.animationName = reader.ReadNullTerminatedString();
                        break;
                    // entries
                    case 1:
                        sounds.sounds.Add(ParseAnimationSound(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationSounds in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationSounds: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return sounds;
        }

        private void ParseAnimationSoundsArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var sound = ParseAnimationSounds(reader.ReadBytes(definitionSize), adrFilePath);
                        animationSounds.Add(sound);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationSoundsArray in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationSounds array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x0B, Animation Particle Data
        // structurally, this is very similar to AnimationSounds
        public enum EnumAnimationParticleField : byte {
            TriggerEvents = 1,
            Name = 3,
            ToolName = 4,
            Id = 5,
        }
        private AnimationParticleEntry ParseAnimationParticle(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var particle = new AnimationParticleEntry();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumAnimationParticleField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumAnimationParticleField.Name:
                        particle.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumAnimationParticleField.ToolName:
                        particle.toolName = reader.ReadNullTerminatedString();
                        break;
                    case EnumAnimationParticleField.Id:
                        particle.id = reader.ReadUInt16();
                        break;
                    case EnumAnimationParticleField.TriggerEvents:
                        particle.events.Add(ParseTriggerEvent(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case (EnumAnimationParticleField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationParticleEntry in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationParticleEntry: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return particle;
        }

        private AnimationParticles ParseAnimationParticles(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var particles = new AnimationParticles();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // name
                    case 2:
                        particles.particleName = reader.ReadNullTerminatedString();
                        break;
                    // entries
                    case 1:
                        particles.sounds.Add(ParseAnimationParticle(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationParticles in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationParticles: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return particles;
        }

        private void ParseAnimationParticlesArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var particle = ParseAnimationParticles(reader.ReadBytes(definitionSize), adrFilePath);
                        animationParticles.Add(particle);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationParticles array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x0C, Animation Action Point Data
        public enum EnumAnimationActionPointField : byte {
            Name = 1,
            Time = 2,
        }
        private AnimationActionPoint ParseAnimationActionPoint(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var point = new AnimationActionPoint();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumAnimationActionPointField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumAnimationActionPointField.Name:
                        point.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumAnimationActionPointField.Time:
                        point.time = reader.ReadAdrFloat();
                        break;
                    case (EnumAnimationActionPointField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationActionPoint in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationActionPoint: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return point;
        }

        private List<AnimationActionPoint> ParseAnimationActionPointArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var list = new List<AnimationActionPoint>();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entries
                    case 1:
                        list.Add(ParseAnimationActionPoint(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationActionPoint array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationActionPoint array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return list;
        }

        private AnimationActionPoints ParseAnimationActionPoints(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var particles = new AnimationActionPoints();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // name
                    case 2:
                        particles.animationName = reader.ReadNullTerminatedString();
                        break;
                    // entries
                    case 1:
                        particles.actionPoints.AddRange(ParseAnimationActionPointArray(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnmationActionPoints in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationActionPoints: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return particles;
        }

        private void ParseAnimationActionPointsArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var actionPoint = ParseAnimationActionPoints(reader.ReadBytes(definitionSize), adrFilePath);
                        animationActionPoints.Add(actionPoint);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationActionPoints array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for AnimationActionPoints array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x0D, Collision Data
        private void ParseCollisionDefinition(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        collisionFile = reader.ReadNullTerminatedString();
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Collision data in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for Collision data: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region Data Structures
        // TODO: organize this code better
        [Serializable]
        public class ParticleEmitterDefinition {
            public int id;
            public string name;
            public string effectFileName;
        }

        [Serializable]
        public class TextureAlias {
            // Believe this to be the hash of the material property (dma) to apply this to, needs investigation to confirm
            public uint hash;
            public string aliasName;
            public string texture;
            public bool aliasIsDefault;
        }

        [Serializable]
        public class AnimationData {
            public string name;
            public string fileName;
            // what happens when this doesnt match the actual file's duration?
            public float duration;
            public byte loadType;
        }

        [Serializable]
        public class TriggerEvent {
            public float start;
            public float end;
        }

        [Serializable]
        public class AnimationSoundEntry {
            public byte effectType;
            public string name;
            public string toolName;
            public ushort id;
            public bool playOnce;
            public readonly List<TriggerEvent> events = new();
        }

        [Serializable]
        public class AnimationSounds {
            public string animationName;
            public readonly List<AnimationSoundEntry> sounds = new();
        }

        [Serializable]
        public class AnimationParticleEntry {
            public string name;
            public string toolName;
            public ushort id;
            public readonly List<TriggerEvent> events = new();
        }

        [Serializable]
        public class AnimationParticles {
            public string particleName;
            public readonly List<AnimationParticleEntry> sounds = new();
        }

        [Serializable]
        public class AnimationActionPoint {
            public string name;
            public float time;
        }

        [Serializable]
        public class AnimationActionPoints {
            public string animationName;
            public readonly List<AnimationActionPoint> actionPoints = new();
        }

        #endregion
    }
}
