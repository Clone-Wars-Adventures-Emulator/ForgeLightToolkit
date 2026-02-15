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
            MaterialMappings = 0x04,
            TextureAliases = 0x05,
            TintAliases = 0x06,
            Effect = 0x07,
            Unknown0x08 = 0x08, // Not used in CWA
            AnimationDataArray = 0x09,
            AnimationSounds = 0x0A,
            AnimationParticlesArray = 0x0B,
            AnimationActoinPoints = 0x0C,
            CollisionData = 0x0D,
            OcclusionData = 0x0E,
            // TODO: names
            MiscData = 0x0F,
            MiscData2 = 0x10,
            Unknown0x11 = 0x11, // Not used in CWA
            EquippedSlot = 0x12,
            Unknown0x13 = 0x13, // Not used in CWA
            MountData = 0x14,
            CompositeAnimationEffect = 0x15,
            LookControl = 0x16,
        }

        public enum EnumModelDataFieldType : byte {
            Unknown = 0,
            ModelFile = 1,
            MaterialFile = 2,
            UpdateRadius = 3,
        }

        // TODO: investigate defaults for all of this
        public string skeletonFileName;
        public float skeletonScale = 1.0f;

        public string modelFileName;
        public string materialFileName;
        public float updateRadius;

        public string collisionFile;

        public uint occlusionBitMask;

        // TODO: this is an enum, what are the enum values?
        public byte actorUsage;
        public string attachmentBone;
        // The name of this is weird, does this mean we need to validate the object when it is used by a player and non player character? wouldnt that just be all characters???
        public bool validatePcNpc;
        public bool inheritAnimations;

        public bool coversFacialHair;

        public string equippedSlot;

        public MountData mountData;

        public readonly List<ParticleEmitterDefinition> particleEmitterDefinitions = new();
        public readonly List<MaterialMapping> materialMappings = new();
        public readonly List<TextureAlias> textureAliases = new();
        public readonly List<TintAlias> tintAliases = new();
        public readonly List<Effect> effects = new();
        public readonly List<AnimationData> animations = new();
        public readonly List<AnimationSounds> animationSounds = new();
        public readonly List<AnimationParticles> animationParticles = new();
        public readonly List<AnimationActionPoints> animationActionPoints = new();
        public readonly List<CompositeAnimationEffects> compositeEffects = new();
        public readonly List<LookControl> lookControls = new();

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
                    case EnumPrimaryDataType.MaterialMappings:
                        ParseMaterialMappingArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.TextureAliases:
                        ParseTextureAliasArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.TintAliases:
                        ParseTintAliasArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.Effect:
                        ParseEffectsArray(definitionData, filePath);
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
                    case EnumPrimaryDataType.OcclusionData:
                        ParseOcclusionData(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.MiscData:
                        ParseMiscData(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.MiscData2:
                        ParseMiscData2(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.EquippedSlot:
                        ParseEquippedSlot(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.MountData:
                        ParseMountData(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.CompositeAnimationEffect:
                        ParseCompositeAnimationEffectsArray(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.LookControl:
                        ParseLookControlArray(definitionData, filePath);
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
                    case 2:
                        skeletonScale = reader.ReadAdrFloat();
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
            BoneName = 3,
            Heading = 4,
            Pitch = 5,
            Scale = 6,
            OffsetX = 7,
            OffsetY = 8,
            OffsetZ = 9,
            EmitterFile = 0x0a,
            UnknownBoneString = 0x0b,
            WorldOrientation = 0x0d,
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
                    case EnumParticleDataFieldType.BoneName:
                        defn.boneName = reader.ReadNullTerminatedString();
                        break;
                    case EnumParticleDataFieldType.Heading:
                        defn.heading = reader.ReadAdrFloat();
                        break;
                    case EnumParticleDataFieldType.Pitch:
                        defn.pitch = reader.ReadAdrFloat();
                        break;
                    case EnumParticleDataFieldType.Scale:
                        defn.scale = reader.ReadAdrFloat();
                        break;
                    case EnumParticleDataFieldType.OffsetX:
                        defn.offsetX = reader.ReadAdrFloat();
                        break;
                    case EnumParticleDataFieldType.OffsetY:
                        defn.offsetY = reader.ReadAdrFloat();
                        break;
                    case EnumParticleDataFieldType.OffsetZ:
                        defn.offsetZ = reader.ReadAdrFloat();
                        break;
                    case EnumParticleDataFieldType.UnknownBoneString:
                        defn.unknownBoneString = reader.ReadNullTerminatedString();
                        break;
                    case EnumParticleDataFieldType.EmitterFile:
                        defn.effectFileName = reader.ReadNullTerminatedString();
                        break;
                    case EnumParticleDataFieldType.WorldOrientation:
                        defn.worldOrientation = reader.ReadBool();
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

        #region ADR Type 0x04, Material Mappings
        public enum EnumMaterialMappingField : byte {
            Name = 1,
            SemanticHash = 2,
        }
        private MaterialMapping ParseMaterialMapping(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var mapping = new MaterialMapping();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumMaterialMappingField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumMaterialMappingField.Name:
                        mapping.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumMaterialMappingField.SemanticHash:
                        mapping.hash = reader.ReadUInt32();
                        break;
                    case (EnumMaterialMappingField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in MaterialMapping in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for MaterialMapping: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return mapping;
        }

        private void ParseMaterialMappingArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var mapping = ParseMaterialMapping(reader.ReadBytes(definitionSize), adrFilePath);
                        materialMappings.Add(mapping);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in MaterialMapping Array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for MaterialMapping array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x05, Texture Alias
        public enum EnumTextureAliasField : byte {
            MaterialIndex = 2,
            SemanticHash = 3,
            AliasName = 4,
            TextureName = 5,
            OcclusionMask = 6,
            AliasIsDefault = 7,
        }
        private TextureAlias ParseTextureAlias(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var alias = new TextureAlias();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumTextureAliasField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumTextureAliasField.MaterialIndex:
                        alias.materialIndex = reader.ReadByte();
                        break;
                    case EnumTextureAliasField.SemanticHash:
                        alias.hash = reader.ReadUInt32();
                        break;
                    case EnumTextureAliasField.AliasName:
                        alias.aliasName = reader.ReadNullTerminatedString();
                        break;
                    case EnumTextureAliasField.TextureName:
                        alias.texture = reader.ReadNullTerminatedString();
                        break;
                    case EnumTextureAliasField.OcclusionMask:
                        // TODO: blow up if this doesnt meet expectations
                        if (definitionSize > 4) {
                            Debug.LogError($"Occlusion Mask in TextureAlias in {adrFilePath} has len > 4 ({definitionSize}), the mask will not be parsed");
                            reader.Skip(definitionSize);
                            continue;
                        }
                        var bytes = reader.ReadBytes(definitionSize);
                        uint collector = 0;
                        if (!reader.IsLittleEndian) {
                            Array.Reverse(bytes);
                        }
                        
                        for (int i = definitionSize - 1; i >= 0; i--) {
                            collector |= ((uint) (bytes[i] << (8 * i)));
                        }
                        // alias.occlusionMask = ((uint) bytes[2] << 16) | ((uint) bytes[1] << 8) | bytes[0];
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

        #region ADR Type 0x06, Texture Alias
        public enum EnumTintAliasField : byte {
            MaterialIndex = 2,
            SemanticHash = 3,
            AliasName = 4,
            ColorChannel1 = 5,
            ColorChannel2 = 6,
            ColorChannel3 = 7,
            AliasIsDefault = 8,
        }
        private TintAlias ParseTintAlias(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var alias = new TintAlias();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumTintAliasField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumTintAliasField.MaterialIndex:
                        alias.materialIndex = reader.ReadByte();
                        break;
                    case EnumTintAliasField.SemanticHash:
                        alias.hash = reader.ReadUInt32();
                        break;
                    case EnumTintAliasField.AliasName:
                        alias.aliasName = reader.ReadNullTerminatedString();
                        break;
                    case EnumTintAliasField.ColorChannel1:
                        alias.colorChannel1 = reader.ReadAdrFloat();
                        break;
                    case EnumTintAliasField.ColorChannel2:
                        alias.colorChannel2 = reader.ReadAdrFloat();
                        break;
                    case EnumTintAliasField.ColorChannel3:
                        alias.colorChannel3 = reader.ReadAdrFloat();
                        break;
                    case EnumTintAliasField.AliasIsDefault:
                        alias.aliasIsDefault = reader.ReadBool();
                        break;
                    case (EnumTintAliasField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in TintAlias in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for TintAlias: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return alias;
        }

        private void ParseTintAliasArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var alias = ParseTintAlias(reader.ReadBytes(definitionSize), adrFilePath);
                        tintAliases.Add(alias);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in TintAlias Array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for TintAlias array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x07, Effect
        public enum EnumEffectField : byte {
            Name = 3,
            ToolName = 4,
            Id = 5,
        }
        private Effect ParseEffect(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var effect = new Effect();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumEffectField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumEffectField.Name:
                        effect.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumEffectField.ToolName:
                        effect.toolName = reader.ReadNullTerminatedString();
                        break;
                    case EnumEffectField.Id:
                        effect.id = reader.ReadUInt16();
                        break;
                    case (EnumEffectField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Effect in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for Effect: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return effect;
        }

        private void ParseEffectsArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var effect = ParseEffect(reader.ReadBytes(definitionSize), adrFilePath);
                        effects.Add(effect);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Effects Array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for Effects array: {adrFilePath}");
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
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in AnimationParticles array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
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

        #region ADR Type 0x0E, Occlusion Data
        private void ParseOcclusionData(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 2:
                        // TODO: blow up if this doesnt meet expectations
                        if (definitionSize > 4) {
                            Debug.LogError($"Occlusion Mask in OcclusionData in {adrFilePath} has len > 4 ({definitionSize}), the mask will not be parsed");
                            reader.Skip(definitionSize);
                            continue;
                        }
                        var bytes = reader.ReadBytes(definitionSize);
                        uint collector = 0;
                        if (!reader.IsLittleEndian) {
                            Array.Reverse(bytes);
                        }

                        for (int i = definitionSize - 1; i >= 0; i--) {
                            collector |= ((uint) (bytes[i] << (8 * i)));
                        }
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Occlusion Data in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for Occlusion data: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x0F, Misc Data
        private void ParseMiscData(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        actorUsage = reader.ReadByte();
                        break;
                    case 2:
                        attachmentBone = reader.ReadNullTerminatedString();
                        break;
                    case 3:
                        validatePcNpc = reader.ReadBool();
                        break;
                    case 4:
                        inheritAnimations = reader.ReadBool();
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Misc Data in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for Misc data: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x10, Misc Data 2
        private void ParseMiscData2(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        coversFacialHair = reader.ReadBool();
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Misc Data 2 in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for Misc data 2: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x12, Equipped Slot
        private void ParseEquippedSlot(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 5:
                        equippedSlot = reader.ReadNullTerminatedString();
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in EquippedSlot in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for EquippedSlot: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x14, Mount Data
        private MountSeatEntrance ParseMountSeatEntrance(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var entrance = new MountSeatEntrance();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        entrance.boneName = reader.ReadNullTerminatedString();
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in MountSeatEntrance in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for MountSeatEntrance: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return entrance;
        }

        private MountSeat ParseMountSeat(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var seat = new MountSeat();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // name
                    case 3:
                        seat.boneName = reader.ReadNullTerminatedString();
                        break;
                    // entries
                    case 1:
                        seat.entrances.Add(ParseMountSeatEntrance(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in MountSeat in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for MountSeat: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return seat;
        }

        private void ParseMountData(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            mountData = new MountData();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // seat
                    case 1:
                        var seat = ParseMountSeat(reader.ReadBytes(definitionSize), adrFilePath);
                        mountData.seats.Add(seat);
                        break;
                    case 9:
                        mountData.runToIdleAnim = reader.ReadNullTerminatedString();
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in MountData in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for MountData array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x15, Composite Animation Effect
        public enum EnumCompositeAnimationEffectField : byte {
            TriggerEvents = 1,
            Type = 2,
            Name = 3,
            ToolName = 4,
            Id = 5,
        }
        private CompositeAnimationEffect ParseCompositeAnimationEffect(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var effect = new CompositeAnimationEffect();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumCompositeAnimationEffectField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumCompositeAnimationEffectField.Name:
                        effect.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumCompositeAnimationEffectField.ToolName:
                        effect.toolName = reader.ReadNullTerminatedString();
                        break;
                    case EnumCompositeAnimationEffectField.Type:
                        effect.effectType = reader.ReadByte();
                        break;
                    case EnumCompositeAnimationEffectField.Id:
                        effect.id = reader.ReadUInt16();
                        break;
                    case EnumCompositeAnimationEffectField.TriggerEvents:
                        effect.events.Add(ParseTriggerEvent(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case (EnumCompositeAnimationEffectField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in CompositeAnimationEffect in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for CompositeAnimationEffect: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return effect;
        }

        private CompositeAnimationEffects ParseCompositeAnimationEffects(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var effects = new CompositeAnimationEffects();

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // name
                    case 2:
                        effects.animationName = reader.ReadNullTerminatedString();
                        break;
                    // entries
                    case 1:
                        effects.effects.Add(ParseCompositeAnimationEffect(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in CompositeAnimationEffects in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for CompositeAnimationEffects: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return effects;
        }

        private void ParseCompositeAnimationEffectsArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var effect = ParseCompositeAnimationEffects(reader.ReadBytes(definitionSize), adrFilePath);
                        compositeEffects.Add(effect);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in CompositeAnimationEffectsArray in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for CompositeAnimationEffects array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region ADR Type 0x16, Look Controls
        public enum EnumJointField : byte {
            Bone = 1,
            UnknwonFloat1 = 2,
            UnknwonFloat2 = 3,
            UnknwonFloat3 = 4,
        }
        private Joint ParseJoint(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var joint = new Joint();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumJointField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumJointField.Bone:
                        joint.bone = reader.ReadNullTerminatedString();
                        break;
                    case EnumJointField.UnknwonFloat1:
                        joint.unknownFloat1 = reader.ReadAdrFloat();
                        break;
                    case EnumJointField.UnknwonFloat2:
                        joint.unknownFloat2 = reader.ReadAdrFloat();
                        break;
                    case EnumJointField.UnknwonFloat3:
                        joint.unknownFloat3 = reader.ReadAdrFloat();
                        break;
                    case (EnumJointField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in Joint in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for Joint: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return joint;
        }

        public enum EnumLookControlField : byte {
            Name = 1,
            EffectorBone = 4,
            Type = 2,
            Joints = 3,
        }
        private LookControl ParseLookControl(byte[] data, string adrFilePath) {
            var reader = new Reader(data);
            var control = new LookControl();

            while (!reader.ReachedEnd) {
                var definitionType = (EnumLookControlField) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case EnumLookControlField.Name:
                        control.name = reader.ReadNullTerminatedString();
                        break;
                    case EnumLookControlField.EffectorBone:
                        control.effectorBone = reader.ReadNullTerminatedString();
                        break;
                    case EnumLookControlField.Type:
                        control.type = reader.ReadByte();
                        break;
                    case EnumLookControlField.Joints:
                        control.joints.Add(ParseJoint(reader.ReadBytes(definitionSize), adrFilePath));
                        break;
                    case (EnumLookControlField) 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in LookControl in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for LookControl: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }

            return control;
        }

        private void ParseLookControlArray(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    // entry
                    case 1:
                        var lookControl = ParseLookControl(reader.ReadBytes(definitionSize), adrFilePath);
                        lookControls.Add(lookControl);
                        break;
                    case 0xFE:
#if DEBUG_ALL_FE_INSTANCES
                        Debug.LogError($"0xFE marker found in LookControl Array in {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                    default:
#if DEBUG_ADR_PARSING
                        Debug.LogError($"Unhandled type 0x{definitionType:X} for LookControl array: {adrFilePath}");
#endif
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
        #endregion

        #region Data Structures
        [Serializable]
        public class ParticleEmitterDefinition {
            public int id;
            public string name;
            public string boneName;
            public float heading;
            public float pitch;
            public float scale = 1.0f;
            public float offsetX;
            public float offsetY;
            public float offsetZ;
            public string effectFileName;
            public string unknownBoneString;
            public bool worldOrientation;
        }

        [Serializable]
        public class MaterialMapping {
            public string name;
            public uint hash;
        }

        [Serializable]
        public class TextureAlias {
            public byte materialIndex;
            // Believe this to be the hash of the material property (dma) to apply this to, needs investigation to confirm
            public uint hash;
            public string aliasName;
            public string texture;
            public uint occlusionMask;
            public bool aliasIsDefault;
        }

        [Serializable]
        public class TintAlias {
            public byte materialIndex;
            // Believe this to be the hash of the material property (dma) to apply this to, needs investigation to confirm
            public uint hash;
            public string aliasName;
            // TODO: figure out which one is which (would assume RGB, but EDITz's exported data say 2 is red and the rest are unknown. X has been pressed...)
            public float colorChannel1;
            public float colorChannel2;
            public float colorChannel3;
            public bool aliasIsDefault;
        }

        [Serializable]
        public class Effect {
            public string name;
            public string toolName;
            public ushort id;
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

        [Serializable]
        public class MountData {
            public string runToIdleAnim;
            public readonly List<MountSeat> seats = new();
        }

        [Serializable]
        public class MountSeat {
            public string boneName;
            // TODO: unsure if this is an array, treating it as one for now
            public readonly List<MountSeatEntrance> entrances = new();
        }

        [Serializable]
        public class MountSeatEntrance {
            public string boneName;
        }

        [Serializable]
        public class CompositeAnimationEffect {
            public byte effectType;
            public string name;
            public string toolName;
            public ushort id;
            public readonly List<TriggerEvent> events = new();
        }

        [Serializable]
        public class CompositeAnimationEffects {
            public string animationName;
            public readonly List<CompositeAnimationEffect> effects = new();
        }

        [Serializable]
        public class Joint {
            public string bone;
            // one of these is pitch, the other is yaw, and another is a limit (is there a heading??)
            public float unknownFloat1;
            public float unknownFloat2;
            public float unknownFloat3;
        }

        [Serializable]
        public class LookControl {
            public string name;
            public byte type;
            public readonly List<Joint> joints = new();
            public string effectorBone;
        }
        #endregion
    }
}
