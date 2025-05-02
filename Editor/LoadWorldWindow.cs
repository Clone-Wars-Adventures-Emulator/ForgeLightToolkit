using ForgeLightToolkit.Editor.FileTypes;
using ForgeLightToolkit.Editor.FileTypes.Dma;
using ForgeLightToolkit.Editor.FileTypes.Gcnk;
using ForgeLightToolkit.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ForgeLightToolkit.Editor {
    public class LoadWorldWindow : EditorWindow {
        private const int VerticalSpace = 15;
        private const int HorizontalSpace = 15;
        private const int HorizontalTabbedSpace = 25;

        private string worldName = "";
        private string adrName = "";
        private string assetsPath = "Assets/ExtractedPacks";
        private string prefabSavePath = "Assets/Prefabs/Objects";
        private string materialsSavePath = "Assets/Materials";
        private string terrainMaterialsSavePath = "Assets/TerrainMaterials";
        private string worldPrefabSavePath = "Assets/Prefabs/Worlds";

        private bool fastMode;
        private bool overrideTerrainMaterials;
        private bool overrideWorldPrefabsAndMats;

        private readonly HashSet<string> objectsAlreadyProcessed = new();

        [MenuItem("ForgeLight/Load World")]
        public static void ShowWindow() {
            GetWindow<LoadWorldWindow>("Load World");
        }

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width / EditorGUIUtility.pixelsPerPoint, Screen.height / EditorGUIUtility.pixelsPerPoint));

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Assets Path", EditorStyles.boldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Example: Assets/ForgeLight/CloneWarsAdventures", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            assetsPath = EditorGUILayout.TextField(assetsPath);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Object Prefab Save Location", EditorStyles.boldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Example: Assets/Prefabs/Objects", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            prefabSavePath = EditorGUILayout.TextField(prefabSavePath);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("World Prefab Save Location", EditorStyles.boldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Example: Assets/Prefabs/Worlds", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            worldPrefabSavePath = EditorGUILayout.TextField(worldPrefabSavePath);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Terrain Materials Save Location", EditorStyles.boldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Example: Assets/TerrainMaterials", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            terrainMaterialsSavePath = EditorGUILayout.TextField(terrainMaterialsSavePath);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Object Materials Save Location", EditorStyles.boldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Example: Assets/Materials", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            materialsSavePath = EditorGUILayout.TextField(materialsSavePath);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("World Name", EditorStyles.boldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label("Example: JediTemple", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            worldName = EditorGUILayout.TextField(worldName);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            GUILayout.Label(
                "Please read the tooltips on the following boxes " +
                "to ensure you know what you are doing before " +
                "you run with any of the options selected.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            fastMode = GUILayout.Toggle(fastMode, new GUIContent("Fast Mode", "Loads directly from all original pack assets and skips saving any materials or prefabs"));
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            overrideTerrainMaterials = GUILayout.Toggle(overrideTerrainMaterials, new GUIContent("Override Terrain Materials", "Allows for reprocessing of terrain materials while maintaining all existing object prefabs and materials"));
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            overrideWorldPrefabsAndMats = GUILayout.Toggle(overrideWorldPrefabsAndMats, new GUIContent("Override World Object Prefabs And Materials", "Reprocesses all objects and prefabs in the world"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            if (GUILayout.Button("Load World(s)", GUILayout.ExpandWidth(false)) && buttonClickValid()) {
                var gzneFileAssetGuids = AssetDatabase.FindAssets($"glob:\"{assetsPath}/{worldName}.gzne\"");

                ensureDirectoriesExist();

                objectsAlreadyProcessed.Clear();

                foreach (var gzneFileAssetGuid in gzneFileAssetGuids) {
                    var gzneFileAssetPath = AssetDatabase.GUIDToAssetPath(gzneFileAssetGuid);

                    var gzneFile = AssetDatabase.LoadAssetAtPath<GzneFile>(gzneFileAssetPath);

                    if (gzneFile is null) {
                        continue;
                    }

                    LoadWorld(gzneFile.name);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            adrName = EditorGUILayout.TextField(adrName);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.Space(VerticalSpace);

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            if (GUILayout.Button("Load Adr(s)", GUILayout.ExpandWidth(false)) && buttonClickValid()) {
                var adrFileAssetGuids = AssetDatabase.FindAssets($"glob:\"{assetsPath}/{adrName}.adr\"");

                ensureDirectoriesExist();

                objectsAlreadyProcessed.Clear();

                foreach (var adrFileAssetGuid in adrFileAssetGuids) {
                    var adrFileAssetPath = AssetDatabase.GUIDToAssetPath(adrFileAssetGuid);

                    var adrFile = AssetDatabase.LoadAssetAtPath<AdrFile>(adrFileAssetPath);

                    if (adrFile is null) {
                        continue;
                    }

                    LoadAdrFile(adrFile.name + ".adr", null, new Vector4(0, 0, 0, 0), 1.0f, new Vector4(0, 0, 0, 0));
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private bool buttonClickValid() {
            bool condition = !string.IsNullOrEmpty(assetsPath) && !string.IsNullOrEmpty(prefabSavePath) && !string.IsNullOrEmpty(materialsSavePath);

            if (!condition) {
                List<string> badInputs = new();
                if (string.IsNullOrEmpty(assetsPath)) {
                    badInputs.Add("Assets Path");
                }
                if (string.IsNullOrEmpty(prefabSavePath)) {
                    badInputs.Add("Prefab Save Path");
                }
                if (!string.IsNullOrEmpty(materialsSavePath)) {
                    badInputs.Add($"Materials Save Path");
                }

                Debug.LogError($"LWW: Missing Required Inputs: {string.Join(", ", badInputs)}");
            }

            return condition;
        }

        private void ensureDirectoriesExist() {
            if (!Directory.Exists(assetsPath)) {
                Directory.CreateDirectory(assetsPath);
            }
            if (!Directory.Exists(prefabSavePath)) {
                Directory.CreateDirectory(prefabSavePath);
            }
            if (!Directory.Exists(materialsSavePath)) {
                Directory.CreateDirectory(materialsSavePath);
            }
            if (!Directory.Exists(terrainMaterialsSavePath)) {
                Directory.CreateDirectory(terrainMaterialsSavePath);
            }
            if (!Directory.Exists(worldPrefabSavePath)) {
                Directory.CreateDirectory(worldPrefabSavePath);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void LoadWorld(string worldName) {
            GzneFile gzneFile = AssetDatabase.LoadAssetAtPath<GzneFile>(Path.Combine(assetsPath, $"{worldName}.gzne"));

            if (gzneFile is null) {
                return;
            }

            GameObject loadedWorldObject = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(worldPrefabSavePath, $"World_{worldName}.prefab"));
            if (loadedWorldObject is not null && !overrideWorldPrefabsAndMats && !fastMode) {
                PrefabUtility.InstantiatePrefab(loadedWorldObject);
                return;
            }
            GameObject worldObject = new GameObject($"World_{worldName}");

            Dictionary<int, RuntimeObject> loadedRuntimeObjects = new Dictionary<int, RuntimeObject>();

            for (var x = gzneFile.StartX; x < gzneFile.WorldSize; x += gzneFile.TilePerChunkAxis) {
                for (var y = gzneFile.StartY; y < gzneFile.WorldSize; y += gzneFile.TilePerChunkAxis) {
                    var chunkFileName = $"{worldName}_{x}_{y}";

                    var gcnkFilePath = Path.Combine(assetsPath, $"{chunkFileName}.gcnk");

                    var gcnkFile = AssetDatabase.LoadAssetAtPath<GcnkFile>(gcnkFilePath);

                    if (gcnkFile is null) {
                        continue;
                    }

                    var chunkObject = new GameObject($"Chunk ({gcnkFile.Coords.x}, {gcnkFile.Coords.y})") {
                        transform = {
                            parent = worldObject.transform
                        }
                    };

                    if (!gzneFile.HideTerrain) {
                        var chunkMeshFilter = chunkObject.AddComponent<MeshFilter>();

                        chunkMeshFilter.sharedMesh = gcnkFile.Mesh;

                        var chunkMeshRenderer = chunkObject.AddComponent<MeshRenderer>();

                        var chunkMaterials = new Material[gcnkFile.Mesh.subMeshCount];

                        var gck2FilePath = Path.Combine(assetsPath, $"{chunkFileName}.gck2");

                        var gck2File = AssetDatabase.LoadAssetAtPath<Gck2File>(gck2FilePath);

                        foreach (var tile in gcnkFile.Tiles) {
                            Material loadedChunkMaterial = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(terrainMaterialsSavePath, gcnkFile.name + "_" + tile.Index.ToString() + ".mat"));
                            var chunkMaterial = new Material(Shader.Find($"Custom/RuntimeTerrain_{tile.EcoDataList.Count}")) {
                                name = $"Tile {tile.Index}"
                            };

                            if (loadedChunkMaterial is not null && !overrideTerrainMaterials && !fastMode) {
                                chunkMaterial = loadedChunkMaterial;
                                chunkMaterials[tile.Index] = chunkMaterial;
                                continue;
                            }
                            if (gck2File is not null) {
                                chunkMaterial.mainTexture = gck2File.Texture;
                            }

                            if (gcnkFile.DetailMask is not null) {
                                chunkMaterial.SetTexture("_DetailMaskMap", gcnkFile.DetailMask);
                            }

                            for (var i = 0; i < tile.EcoDataList.Count; i++) {
                                var ecoDataIndex = tile.EcoDataList[i];
                                var ecoData = gzneFile.EcoData[ecoDataIndex];

                                chunkMaterial.SetFloat($"_DetailRepeat{i}", ecoData.Scale);

                                var ecoDataTextureFilePath = Path.Combine(assetsPath, Path.ChangeExtension(ecoData.Texture, "png"));

                                var ecoDataTexture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(ecoDataTextureFilePath);

                                chunkMaterial.SetTexture($"_DetailColorMap{i}", ecoDataTexture2d);
                            }
                            if (!fastMode) {
                                AssetDatabase.CreateAsset(chunkMaterial, Path.Combine(terrainMaterialsSavePath, gcnkFile.name + "_" + tile.Index.ToString() + ".mat"));
                            }
                            chunkMaterials[tile.Index] = chunkMaterial;
                        }
                        chunkMeshRenderer.materials = chunkMaterials;
                    }

                    foreach (var tile in gcnkFile.Tiles) {
                        foreach (var runtimeObject in tile.RuntimeObjects) {
                            if (runtimeObject.Unknown > 0) {
                                if (!loadedRuntimeObjects.TryAdd(runtimeObject.Unknown, runtimeObject)) {
                                    continue;
                                }
                            } else {
                                if (!loadedRuntimeObjects.TryAdd(runtimeObject.ObjectId, runtimeObject)) {
                                    continue;
                                }
                            }

                            var fileExtension = Path.GetExtension(runtimeObject.FileName);

                            if (fileExtension == ".adr") {
                                LoadAdrFile(runtimeObject.FileName, chunkObject, runtimeObject.Position, runtimeObject.Scale, runtimeObject.Rotation, runtimeObject.ObjectId);
                            } else if (fileExtension == ".agr") {
                                var agrFilePath = Path.Combine(assetsPath, runtimeObject.FileName);

                                var agrFile = AssetDatabase.LoadAssetAtPath<AgrFile>(agrFilePath);

                                if (agrFile is null) {
                                    Debug.LogError($"Failed to load Agr. {agrFilePath}");
                                    continue;
                                }

                                foreach (var actor in agrFile.ActorSet.Actors) {
                                    LoadAdrFile(actor.Name, chunkObject, runtimeObject.Position, runtimeObject.Scale, runtimeObject.Rotation, runtimeObject.ObjectId);
                                }
                            }
                        }

                        foreach (var rawLight in tile.RawLights) {
                            var lightObject = new GameObject($"Light ({rawLight.Name})") {
                                transform = {
                                    parent = chunkObject.transform,
                                    position = rawLight.Position
                                }
                            };

                            Light lightComp = lightObject.AddComponent<Light>();

                            lightComp.range = rawLight.Range;
                            lightComp.color = rawLight.Color;
                            lightComp.intensity = rawLight.Intensity;
                            lightComp.lightmapBakeType = LightmapBakeType.Baked;
                        }
                    }
                }
            }
            worldObject.transform.localScale = new Vector3(1, 1, -1);
            if (!fastMode) {
                PrefabUtility.SaveAsPrefabAssetAndConnect(worldObject, Path.Combine(worldPrefabSavePath, worldObject.name + ".prefab"), InteractionMode.AutomatedAction);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void LoadAdrFile(string adrFileName, GameObject parentObject, Vector4 position, float scale, Vector4 rotation, int runtimeId = 0) {
            var adrFilePath = Path.Combine(assetsPath, adrFileName);
            var adrFile = AssetDatabase.LoadAssetAtPath<AdrFile>(adrFilePath);
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(prefabSavePath, Path.ChangeExtension(adrFileName, "prefab")));
            if (existingPrefab is not null && objectsAlreadyProcessed.Contains(adrFileName.Split(".")[0]) && !fastMode) {
                GameObject loadedObject = PrefabUtility.InstantiatePrefab(existingPrefab, parentObject.transform) as GameObject;
                loadedObject.transform.localPosition = position;
                loadedObject.transform.localScale = Vector3.one * scale;
                loadedObject.transform.localRotation = Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg);

                if (!loadedObject.TryGetComponent<ForgelightObject>(out var forgelightObject)) {
                    Debug.LogWarning($"Prefab for {adrFileName} was missing the ForgelightObject component. Adding a new instance");
                    forgelightObject = loadedObject.AddComponent<ForgelightObject>();
                    forgelightObject.AdrFileName = adrFileName;
                }

                // Runtime instance IDs of objects shouldnt be saved in the prefab, which means each instantiation needs it's id
                forgelightObject.RuntimeObjectId = runtimeId;

                return;
            }

            if (adrFile is null) {
                Debug.LogError($"Failed to load Adr. {adrFilePath}");
                return;
            }

            if (adrFile.ModelFileName is null) {
                Debug.LogError($"Adr has no model file name. {adrFilePath}");
                return;
            }

            var dmeFilePath = Path.Combine(assetsPath, adrFile.ModelFileName);
            var dmeFile = AssetDatabase.LoadAssetAtPath<DmeFile>(dmeFilePath);
            if (dmeFile is null) {
                Debug.LogError($"Failed to load Dme. {dmeFilePath}");
                return;
            }

            var runtimeObject = new GameObject(adrFileName.Split(".")[0]) {
                transform = {
                    parent = parentObject == null ? null : parentObject.transform,
                    localPosition = position,
                    localScale = Vector3.one * scale,
                    localRotation = Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg)
                }
            };

            // add the runtime data for this object
            var flo = runtimeObject.AddComponent<ForgelightObject>();
            flo.AdrFileName = adrFileName;

            foreach (var meshEntry in dmeFile.Meshes) {
                var meshObject = new GameObject() {
                    transform = {
                        parent = runtimeObject.transform,
                        localPosition = Vector3.zero,
                        localScale = Vector3.one,
                        localRotation = Quaternion.identity
                    }
                };

                var objectMeshFilter = meshObject.AddComponent<MeshFilter>();
                objectMeshFilter.sharedMesh = meshEntry.Mesh;
                var objectMeshRenderer = meshObject.AddComponent<MeshRenderer>();
                var materialEntry = dmeFile.DmaFile.MaterialEntries[meshEntry.MaterialIndex];
                var materialDefinition = MaterialInfo.Instance.MaterialDefinitions.FirstOrDefault(x => x.NameHash == materialEntry.Hash);
                if (materialDefinition is null) {
                    continue;
                }

                if (materialDefinition.Name.Contains("NoShadow")) {
                    objectMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                }

                var materialShader = Shader.Find($"Custom/{materialDefinition.Name}");

                if (materialShader is null) {
                    Debug.LogWarning($"Missing Shader \"{materialDefinition.Name}\" for Object \"{adrFileName}\".");
                    continue;
                }

                Material objectMaterial = new Material(materialShader);

                var matFileName = "";
                Material loadedMat = null;

                foreach (var parameterEntry in materialEntry.ParameterEntries) {
                    if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_OBJECT && !fastMode) {
                        var textureName = dmeFile.DmaFile.Textures.FirstOrDefault(x => JenkinsHelper.JenkinsOneAtATimeHash(x.ToUpper()) == parameterEntry.Object);
                        textureName ??= "SOMETHING_HAS_GONE_WRONG.mat";
                        matFileName = Path.ChangeExtension(materialDefinition.Name + "_" + textureName.Split(".")[0] + adrFileName, "mat");
                        loadedMat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(materialsSavePath, matFileName));
                    }
                }

                if (loadedMat != null) {
                    objectMeshRenderer.material = loadedMat;
                    meshObject.name = meshEntry.Mesh.name;
                    continue;
                }

                foreach (var parameterEntry in materialEntry.ParameterEntries) {
                    var parameterName = $"_{(ParameterName) parameterEntry.Hash}";

                    if (!objectMaterial.HasProperty(parameterName)) {
                        Debug.LogWarning($"{materialDefinition.Name}\t{parameterName}\t{parameterEntry.Class}\t{parameterEntry.Type}\t{parameterEntry.Int}\t{parameterEntry.Float}\t{parameterEntry.Vector4}\t{parameterEntry.Matrix4x4}\t{parameterEntry.Object}");
                    }

                    if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_SCALAR) {
                        if (parameterEntry.Type == D3DXPARAMETER_TYPE.D3DXPT_FLOAT) {
                            objectMaterial.SetFloat(parameterName, parameterEntry.Float);
                        } else {
                            objectMaterial.SetInteger(parameterName, parameterEntry.Int);
                        }
                    } else if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_VECTOR) {
                        objectMaterial.SetVector(parameterName, parameterEntry.Vector4);
                    } else if (parameterEntry.Class is D3DXPARAMETER_CLASS.D3DXPC_MATRIX_ROWS or D3DXPARAMETER_CLASS.D3DXPC_MATRIX_COLUMNS) {
                        objectMaterial.SetMatrix(parameterName, parameterEntry.Matrix4x4);
                    } else if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_OBJECT) {
                        var textureHash = parameterEntry.Object;

                        var textureName = dmeFile.DmaFile.Textures.FirstOrDefault(x => JenkinsHelper.JenkinsOneAtATimeHash(x.ToUpper()) == textureHash);

                        if (textureName is null) {
                            Debug.LogError($"Failed to find texture. {textureHash}");
                            continue;
                        }

                        var textureFilePath = Path.Combine(assetsPath, Path.ChangeExtension(textureName, "png"));

                        var texture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(textureFilePath);

                        if (texture2d is null) {
                            Debug.LogError($"Failed to find texture. {textureFilePath}");
                            continue;
                        }

                        objectMaterial.SetTexture(parameterName, texture2d);
                        objectMaterial.SetTextureScale(parameterName, Vector2.right + Vector2.down);

                        matFileName = Path.ChangeExtension(materialDefinition.Name + "_" + textureName.Split(".")[0] + adrFileName, "mat");
                        objectMaterial.name = textureName.Split(".")[0];
                    }
                }
                if (matFileName == "") {
                    matFileName = $"See_LoadWorldWindow_Line_{LineNumber()}.mat";
                }
                if (!fastMode) {
                    AssetDatabase.CreateAsset(objectMaterial, Path.Combine(materialsSavePath, matFileName));
                }
                meshObject.name = meshEntry.Mesh.name;
                matFileName = "";

                objectMeshRenderer.material = objectMaterial;
            }

            objectsAlreadyProcessed.Add(runtimeObject.name);
            if (!fastMode) {
                runtimeObject.transform.localScale = Vector3.one;
                PrefabUtility.SaveAsPrefabAssetAndConnect(runtimeObject, Path.Combine(prefabSavePath, runtimeObject.name + ".prefab"), InteractionMode.AutomatedAction);
                runtimeObject.transform.localScale = Vector3.one * scale;
            }

            // only do this after the prefab has been saved, Runtime instance IDs of objects shouldnt be saved in that
            flo.RuntimeObjectId = runtimeId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LineNumber([CallerLineNumber] int lineNumber = 0) {
            return lineNumber;
        }
    }
}
