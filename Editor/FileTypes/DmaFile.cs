// Uncomment if you want to fill in missing FLTK Built In shaders
// #define COMPLTAIN_ABOUT_MISSING_BUILT_INS

using ForgeLightToolkit.Editor.FileTypes.Dma;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes {
    public class DmaFile : ScriptableObject {
        // The set of ForgeLight materials for which FLTK does not have a built in shader
        private static readonly HashSet<string> KnownMissingShaders = new() {
            "BubbleRigid",
            "BubbleSkin",
            "Bullet",
            "BulletSimple",
            "DualTextureSkin",
            "DualTextureSpecRigid",
            "ForceDistortRigid",
            "GhostRigidDoubleSided",
            "GhostSkinDoubleSided",
            "GlassRigidDepth",
            "GlassSkin",
            "GlowSkin",
            "HologramSkin",
            "HologramRigidOverlay",
            "LightsaberCore",
            "LightsaberCoreSkin",
            "LightsaberGlow",
            "LightsaberGlowSimple",
            "LightsaberGlowSimpleSkin",
            "LightsaberGlowSkin",
            "Ocean",
            "SilhouetteRigid",
            "SimpleRigidAlphaBlend",
            "SimpleRigidSkin",
            "SimpleSkinAlphaScreen",
            "SkyMesh",
            "SkyMeshBlend",
            "SwayRigidAlphaText",
            "TintMaskGlowSkin",
            "TintMaskRigid",
            "TintMaskSpeRigidAlphaTest",
            "WaterRigidSkin",
        };

        public List<string> Textures = new();

        public int MaterialCount;
        public List<MaterialEntry> MaterialEntries = new();

        public List<Material> materials = new();

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

            if (magic != "DMAT") {
                return false;
            }

            var version = reader.ReadUInt32();

            if (version != 1) {
                return false;
            }

            var textureDataSize = reader.ReadInt32();

            while (textureDataSize > 0) {
                var texture = reader.ReadNullTerminatedString();

                Textures.Add(texture);

                textureDataSize -= texture.Length + 1;
            }

            MaterialCount = reader.ReadInt32();

            for (var i = 0; i < MaterialCount; i++) {
                var materialEntry = new MaterialEntry();

                // TODO: dont know what this hash maps to
                var hash = reader.ReadUInt32();

                var materialData = reader.ReadBytes(reader.ReadInt32());

                materialEntry.Deserialize(new Reader(materialData), name);

                MaterialEntries.Add(materialEntry);
            }

            return true;
        }

        public List<Material> CreateMaterialsFrom(string embededIn = "") {
            materials.Clear();

            var textureHashes = new Dictionary<uint, string>();
            foreach (var texture in Textures) {
                var hash = JenkinsHelper.JenkinsOneAtATimeHash(texture.ToUpperInvariant());
                if (!textureHashes.ContainsKey(hash)) {
                    textureHashes.Add(hash, texture);
                }
            }

            for (int i = 0; i < MaterialEntries.Count; i++) {
                var entry = MaterialEntries[i];
                var materialName = IShaderNameSelector.GetUnityShaderForMaterial(entry.Name);

                var shader = Shader.Find(materialName);
                if (shader == null) {
                    if (materialName.StartsWith("FLTK/Built") && KnownMissingShaders.Contains(entry.Name)) {
#if COMPLTAIN_ABOUT_MISSING_BUILT_INS
                        Debug.LogError($"FLTK does not have a built in shader for the ForgeLight material {entry.Name}");
#endif
                    } else {
                        // is a user defined shader or is a built in that we have not marked as known
                        Debug.LogError($"Did not find shader for {materialName}");
                    }

                    // TODO: for now, dont worry about fall backs or trying to add one, just skip the material
                    materials.Add(null);
                    continue;
                }

                var material = new Material(shader) {
                    name = string.IsNullOrEmpty(embededIn) ? $"{name}_{i}" : $"{embededIn}_EmbedDma_{name}_{i}"
                };

                foreach (var parameterEntry in entry.ParameterEntries) {
                    var parameterName = $"_{(EnumParameterName) parameterEntry.Hash}";

                    if (!material.HasProperty(parameterName)) {
                        Debug.LogWarning($"{entry.Name}\t{parameterName}\t{parameterEntry.Class}\t{parameterEntry.Type}\t{parameterEntry.Int}\t{parameterEntry.Float}\t{parameterEntry.Vector4}\t{parameterEntry.Matrix4x4}\t{parameterEntry.Object}");
                    }

                    if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_SCALAR) {
                        if (parameterEntry.Type == D3DXPARAMETER_TYPE.D3DXPT_FLOAT) {
                            material.SetFloat(parameterName, parameterEntry.Float);
                        } else {
                            material.SetInteger(parameterName, parameterEntry.Int);
                        }
                    } else if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_VECTOR) {
                        material.SetVector(parameterName, parameterEntry.Vector4);
                    } else if (parameterEntry.Class is D3DXPARAMETER_CLASS.D3DXPC_MATRIX_ROWS or D3DXPARAMETER_CLASS.D3DXPC_MATRIX_COLUMNS) {
                        material.SetMatrix(parameterName, parameterEntry.Matrix4x4);
                    } else if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_OBJECT) {
                        var textureHash = parameterEntry.Object;

                        if (!textureHashes.TryGetValue(textureHash, out var textureName)) {
                            Debug.LogError($"Did not find texture data {textureHash} for parameter {(EnumParameterName) parameterEntry.Hash} in {entry.Name} {material.name}");
                            continue;
                        }
                        textureName = Path.ChangeExtension(textureName, "png");

                        var textures = AssetDatabase.FindAssets("t:texture")
                            .Select(AssetDatabase.GUIDToAssetPath)
                            .Where(p => Path.GetFileNameWithoutExtension(p).Equals(Path.GetFileNameWithoutExtension(textureName), System.StringComparison.InvariantCultureIgnoreCase))
                            .Select(AssetDatabase.LoadAssetAtPath<Texture2D>)
                            .ToList();

                        if (textures.Count == 0) {
                            Debug.LogError($"Did not any texture assets for {textureName}.");
                            continue;
                        }

                        material.SetTexture(parameterName, textures[0]);
                        material.SetTextureScale(parameterName, Vector2.right + Vector2.down);

                        material.name = $"{material.name}_{Path.GetFileNameWithoutExtension(textureName)}";
                    }
                }

                materials.Add(material);
            }

            return materials;
        }
    }
}
