using ForgeLightToolkit.Editor.FileTypes;
using ForgeLightToolkit.Editor.FileTypes.Dma;
using ForgeLightToolkit.Editor.FileTypes.Gcnk;
using ForgeLightToolkit.Runtime;
using ForgeLightToolkit.Runtime.EditorDebug;
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

        [SerializeField]
        private string worldName = "";
        [NonSerialized]
        private string adrName = "";

        private const string DefaultAssetsPath = "Assets/ExtractedPacks";
        [SerializeField]
        private string assetsPath = DefaultAssetsPath;

        private const string DefaultPrefabSavePath = "Assets/Worlds/_Prefabs";
        [SerializeField]
        private string prefabSavePath = DefaultPrefabSavePath;

        private const string DefaultMaterialsSavePath = "Assets/Worlds/_Materials";
        [SerializeField]
        private string materialsSavePath = DefaultMaterialsSavePath;

        private const string DefaultTerrainMaterialsSavePath = "Assets/Worlds/_Materials";
        [SerializeField]
        private string terrainMaterialsSavePath = DefaultTerrainMaterialsSavePath;

        private const string DefaultWorldPrefabSavePath = "Assets/Worlds/_Prefabs";
        [SerializeField]
        private string worldPrefabSavePath = DefaultWorldPrefabSavePath;

        [SerializeField]
        private bool fastMode;
        [SerializeField]
        private bool visualizeBvh;
        [SerializeField]
        private bool overrideTerrainMaterials;
        [SerializeField]
        private bool overrideWorldPrefabsAndMats;

        [NonSerialized]
        private readonly HashSet<string> objectsAlreadyProcessed = new();

        private SerializedObject so;
        private SerializedObject SObject => so ??= new(this);

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
            GUILayout.Label($"Example: {DefaultAssetsPath}", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            EditorGUILayout.PropertyField(SObject.FindProperty("assetsPath"), new GUIContent());
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
            GUILayout.Label($"Example: {DefaultPrefabSavePath}", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            EditorGUILayout.PropertyField(SObject.FindProperty("prefabSavePath"), new GUIContent());
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
            GUILayout.Label($"Example: {DefaultWorldPrefabSavePath}", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            EditorGUILayout.PropertyField(SObject.FindProperty("worldPrefabSavePath"), new GUIContent());
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
            GUILayout.Label($"Example: {DefaultTerrainMaterialsSavePath}", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            EditorGUILayout.PropertyField(SObject.FindProperty("terrainMaterialsSavePath"), new GUIContent());
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
            GUILayout.Label($"Example: {DefaultMaterialsSavePath}", EditorStyles.miniBoldLabel);
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalSpace);
            EditorGUILayout.PropertyField(SObject.FindProperty("materialsSavePath"), new GUIContent());
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
            EditorGUILayout.PropertyField(SObject.FindProperty("worldName"), new GUIContent());
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

            var fastProp = SObject.FindProperty("fastMode");
            bool wasFast = fastProp.boolValue;
            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            fastProp.boolValue = GUILayout.Toggle(fastMode, new GUIContent("Fast Mode", "Loads directly from all original pack assets and skips saving any materials or prefabs"));
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            var vizProp = SObject.FindProperty("visualizeBvh");
            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            vizProp.boolValue = GUILayout.Toggle(visualizeBvh, new GUIContent("Visualize BVH", "Visualize the Bounding Volume Hierarchy of the world to be loaded. Only works in fast mode."));
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            // if fast mod was enabled but is now off, turn off BVH Visualizing
            if (wasFast && !fastProp.boolValue) {
                vizProp.boolValue = false;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            SObject.FindProperty("overrideTerrainMaterials").boolValue = GUILayout.Toggle(overrideTerrainMaterials, new GUIContent("Override Terrain Materials", "Allows for reprocessing of terrain materials while maintaining all existing object prefabs and materials"));
            GUILayout.Space(HorizontalSpace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            SObject.FindProperty("overrideWorldPrefabsAndMats").boolValue = GUILayout.Toggle(overrideWorldPrefabsAndMats, new GUIContent("Override World Object Prefabs And Materials", "Reprocesses all objects and prefabs in the world"));
            GUILayout.EndHorizontal();

            if (SObject.hasModifiedProperties) {
                SObject.ApplyModifiedPropertiesWithoutUndo();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(HorizontalTabbedSpace);
            if (GUILayout.Button("Load World(s)", GUILayout.ExpandWidth(false)) && buttonClickValid()) {
                var gzneFileAssetGuids = AssetDatabase.FindAssets($"glob:\"{assetsPath}/{worldName}.gzne\"");

                ensureDirectoriesExist();

                objectsAlreadyProcessed.Clear();

                try {
                    AssetDatabase.StartAssetEditing();
                    foreach (var gzneFileAssetGuid in gzneFileAssetGuids) {
                        var gzneFileAssetPath = AssetDatabase.GUIDToAssetPath(gzneFileAssetGuid);

                        var gzneFile = AssetDatabase.LoadAssetAtPath<GzneFile>(gzneFileAssetPath);

                        if (gzneFile == null) {
                            continue;
                        }

                        LoadWorld(gzneFile, worldName);
                    }
                } catch (Exception e) {
                    Debug.LogError($"Caught Exception when trying to import world(s) {worldName}");
                    Debug.LogException(e);
                } finally {
                    AssetDatabase.StopAssetEditing();
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

                try {
                    AssetDatabase.StartAssetEditing();
                    foreach (var adrFileAssetGuid in adrFileAssetGuids) {
                        var adrFileAssetPath = AssetDatabase.GUIDToAssetPath(adrFileAssetGuid);

                        var adrFile = AssetDatabase.LoadAssetAtPath<AdrFile>(adrFileAssetPath);

                        if (adrFile == null) {
                            continue;
                        }

                        LoadAdrFile(adrFile.name + ".adr", null, new Vector4(0, 0, 0, 0), 1.0f, new Vector4(0, 0, 0, 0));
                    }
                } catch (Exception e) {
                    Debug.LogError($"Caught Exception when trying to import adr file(s) {adrName}");
                    Debug.LogException(e);
                } finally {
                    AssetDatabase.StopAssetEditing();
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
        private void LoadWorld(GzneFile gzneFile, string loadedWorldName) {
            // TODO: is it worth looking into handling upserts?

            GameObject loadedWorldObject = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(worldPrefabSavePath, $"World_{loadedWorldName}.prefab"));
            if (loadedWorldObject != null && !overrideWorldPrefabsAndMats && !fastMode) {
                PrefabUtility.InstantiatePrefab(loadedWorldObject);
                return;
            }
            GameObject worldObject = new($"World_{loadedWorldName}");
            var flWorld = worldObject.AddComponent<ForgelightWorld>();
            flWorld.worldName = loadedWorldName;

            Dictionary<int, RuntimeObject> loadedRuntimeObjects = new();

            for (var x = gzneFile.StartX; x < gzneFile.WorldSize; x += gzneFile.TilePerChunkAxis) {
                for (var y = gzneFile.StartY; y < gzneFile.WorldSize; y += gzneFile.TilePerChunkAxis) {
                    var chunkFileName = $"{loadedWorldName}_{x}_{y}";

                    var gcnkFilePath = Path.Combine(assetsPath, $"{chunkFileName}.gcnk");

                    var gcnkFile = AssetDatabase.LoadAssetAtPath<GcnkFile>(gcnkFilePath);

                    if (gcnkFile == null) {
                        continue;
                    }

                    var chunkObject = new GameObject($"Chunk ({gcnkFile.Coords.x}, {gcnkFile.Coords.y})") {
                        transform = {
                            parent = worldObject.transform
                        }
                    };

                    if (fastMode && visualizeBvh) {
                        // This should only be loaded when in editor and not saved as part of any scene or prefab
                        var collisionGizmo = chunkObject.AddComponent<CollisionGizmo>();
                        float maxDepth = gcnkFile.Depth.Max();
                        for (var i = 0; i < gcnkFile.Depth.Count; i++) {
                            var alpha = 1.0f;
                            if (maxDepth > 0) {
                                alpha = gcnkFile.Depth[i] / maxDepth;
                            }
                            var color = new Color(0f, 0f, 1f, alpha);
                            collisionGizmo.bvhData.Add(new CollisionGizmo.BvhData() {
                                center = gcnkFile.BvhCenters[i],
                                size = gcnkFile.BvhSizes[i],
                                color = color,
                            });
                        }
                    }

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

                            if (loadedChunkMaterial != null && !overrideTerrainMaterials && !fastMode) {
                                chunkMaterial = loadedChunkMaterial;
                                chunkMaterials[tile.Index] = chunkMaterial;
                                continue;
                            }
                            if (gck2File != null) {
                                chunkMaterial.mainTexture = gck2File.Texture;
                            }

                            if (gcnkFile.DetailMask != null) {
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

                                if (agrFile == null) {
                                    Debug.LogError($"Failed to load Agr. {agrFilePath}");
                                    continue;
                                }

                                foreach (var actor in agrFile.ActorSet.Actors) {
                                    LoadAdrFile(actor.Name, chunkObject, runtimeObject.Position, runtimeObject.Scale, runtimeObject.Rotation, runtimeObject.ObjectId);
                                }
                            }
                        }

                        for (int i = 0; i < tile.RawLights.Count; i++) {
                            var rawLight = tile.RawLights[i];
                            var lightObject = new GameObject($"Light ({rawLight.Name})") {
                                transform = {
                                    parent = chunkObject.transform,
                                    position = rawLight.Position
                                }
                            };

                            Light lightComp = lightObject.AddComponent<Light>();

                            SetLightProperties(lightComp, rawLight);

                            // persist this data so that we can reset the light to default via a button in the inspector if we want to
                            var flLight = lightObject.AddComponent<ForgelightLight>();
                            flLight.uniqueLightId = ForgelightLight.CreateUniqueLightId(tile.Coords.x, tile.Coords.y, i, rawLight.ColorName);
                        }
                    }
                }
            }

            addWallsToGameObject(flWorld, gzneFile);

            worldObject.transform.localScale = new Vector3(1, 1, -1);
            if (!fastMode) {
                PrefabUtility.SaveAsPrefabAssetAndConnect(worldObject, Path.Combine(worldPrefabSavePath, worldObject.name + ".prefab"), InteractionMode.AutomatedAction);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void LoadAdrFile(string adrFileName, GameObject parentObject, Vector4 position, float scale, Vector4 rotation, int runtimeId = 0) {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(prefabSavePath, Path.ChangeExtension(adrFileName, "prefab")));
            if (existingPrefab != null && objectsAlreadyProcessed.Contains(adrFileName.Split(".")[0]) && !fastMode) {
                GameObject loadedObject = PrefabUtility.InstantiatePrefab(existingPrefab, parentObject.transform) as GameObject;
                loadedObject.transform.localPosition = position;
                loadedObject.transform.localScale = Vector3.one * scale;
                loadedObject.transform.localRotation = Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg);

                if (!loadedObject.TryGetComponent<ForgelightObject>(out var forgelightObject)) {
                    Debug.LogWarning($"Prefab for {adrFileName} was missing the ForgelightObject component. Adding a new instance");
                    forgelightObject = loadedObject.AddComponent<ForgelightObject>();
                    forgelightObject.adrFileName = adrFileName;
                }

                // Runtime instance IDs of objects shouldnt be saved in the prefab, which means each instantiation needs it's id
                forgelightObject.runtimeObjectId = runtimeId;

                return;
            }

            var adrFilePath = Path.Combine(assetsPath, adrFileName);
            var adrFile = AssetDatabase.LoadAssetAtPath<AdrFile>(adrFilePath);

            if (adrFile == null) {
                Debug.LogError($"Failed to load Adr. {adrFilePath}");
                return;
            }

            if (adrFile.modelFileName is null) {
                Debug.LogError($"Adr has no model file name. {adrFilePath}");
                return;
            }

            var dmeFilePath = Path.Combine(assetsPath, adrFile.modelFileName);
            var dmeFile = AssetDatabase.LoadAssetAtPath<DmeFile>(dmeFilePath);
            if (dmeFile == null) {
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
            flo.adrFileName = adrFileName;

            if (adrFile.collisionFile != null && adrFile.collisionFile.Length > 0) {
                AddColliderToObject(runtimeObject, adrFile.collisionFile);
            }

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

                if (materialShader == null) {
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

                        if (texture2d == null) {
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

            // Apply the extensions to the adr instance
            IAdrExtender.ApplyExtenstionsToPrefab(runtimeObject, adrFile);

            objectsAlreadyProcessed.Add(runtimeObject.name);
            if (!fastMode) {
                // save the prefab's position as 0,0,0, transformation should only occur when doing placements
                runtimeObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                runtimeObject.transform.localScale = Vector3.one;
                PrefabUtility.SaveAsPrefabAssetAndConnect(runtimeObject, Path.Combine(prefabSavePath, runtimeObject.name + ".prefab"), InteractionMode.AutomatedAction);
                runtimeObject.transform.localScale = Vector3.one * scale;
                runtimeObject.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg));
            }

            // only do this after the prefab has been saved, Runtime instance IDs of objects shouldnt be saved in that
            flo.runtimeObjectId = runtimeId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LineNumber([CallerLineNumber] int lineNumber = 0) {
            return lineNumber;
        }

        public static void AddColliderToObject(GameObject go, string collisionFile) {
            var collisionFileNoExt = Path.GetFileNameWithoutExtension(collisionFile);
            var cdtPaths = AssetDatabase.FindAssets($"t:cdtFile {collisionFileNoExt}")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => Path.GetFileNameWithoutExtension(path) == collisionFileNoExt)
                .ToList();

            if (cdtPaths == null || cdtPaths.Count == 0) {
                Debug.LogError($"No collision file exists for {collisionFile}, not adding a collider to {go.name}");
                return;
            }

            if (cdtPaths.Count > 1) {
                Debug.LogError($"More than one ({cdtPaths.Count}) collision file exists for {collisionFile}, not adding a collider to {go.name}");
                return;
            }

            var cdtFile = AssetDatabase.LoadAssetAtPath<CdtFile>(cdtPaths[0]);

            var meshCollider = go.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = cdtFile.colliderMesh;
        }

        [MenuItem("ForgeLight/Ensure Preafabs Have Correct ADR File Name")]
        public static void EnsurePreafabsHaveCorrectName() {
            var all = Directory.EnumerateFiles("Assets/Worlds", "*.prefab", SearchOption.AllDirectories);

            try {
                AssetDatabase.StartAssetEditing();
                foreach (var path in all) {
                    var prefabName = Path.GetFileNameWithoutExtension(path);

                    using var prefabContext = new PrefabUtility.EditPrefabContentsScope(path);
                    var go = prefabContext.prefabContentsRoot;

                    if (go.TryGetComponent<ForgelightObject>(out var flo)) {
                        var adrPaths = AssetDatabase.FindAssets($"t:adrFile {Path.GetFileNameWithoutExtension(flo.adrFileName)}")
                            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                            .Where(path => Path.GetFileNameWithoutExtension(path) == prefabName)
                            .ToList();

                        if (adrPaths.Count == 1) {
                            flo.adrFileName = $"{prefabName}.adr";
                        } else {
                            Debug.LogWarning($"{path} found {string.Join(",\n", adrPaths)} as matches");
                        }
                    }
                }
            } catch (Exception e) {
                Debug.LogError("Failed to ensure correct adr file names");
                Debug.LogException(e);
            } finally {
                AssetDatabase.StopAssetEditing();
            }
        }

        [MenuItem("ForgeLight/Add Colliders to ADR Prefabs")]
        public static void AddCollidersToObjectPrefabs() {
            var all = Directory.EnumerateFiles("Assets/Worlds", "*.prefab", SearchOption.AllDirectories);

            try {
                AssetDatabase.StartAssetEditing();
                foreach (var path in all) {
                    try {
                        var prefabName = Path.GetFileNameWithoutExtension(path);

                        using var prefabContext = new PrefabUtility.EditPrefabContentsScope(path);
                        var go = prefabContext.prefabContentsRoot;

                        if (go.TryGetComponent<ForgelightObject>(out var flo) && !go.TryGetComponent<MeshCollider>(out var _)) {
                            var adrPaths = AssetDatabase.FindAssets($"t:adrFile {Path.GetFileNameWithoutExtension(flo.adrFileName)}")
                                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                                .Where(path => Path.GetFileNameWithoutExtension(path) == prefabName)
                                .ToList();

                            if (adrPaths.Count == 1) {
                                var adrFile = AssetDatabase.LoadAssetAtPath<AdrFile>(adrPaths[0]);

                                var cdtFileName = adrFile.collisionFile;
                                if (cdtFileName == null || cdtFileName.Length == 0) {
                                    // if no collider name, dont do anything
                                    continue;
                                }

                                var cdtPaths = AssetDatabase.FindAssets($"t:cdtFile {Path.GetFileNameWithoutExtension(cdtFileName)}")
                                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                                    .Where(path => Path.GetFileNameWithoutExtension(path) == Path.GetFileNameWithoutExtension(cdtFileName))
                                    .ToList();

                                if (cdtPaths.Count == 1) {
                                    var cdtFile = AssetDatabase.LoadAssetAtPath<CdtFile>(cdtPaths[0]);
                                    var mc = go.AddComponent<MeshCollider>();
                                    mc.sharedMesh = cdtFile.colliderMesh;
                                } else {
                                    Debug.LogWarning($"{path} found {string.Join(",\n", cdtPaths)} as cdt matches");
                                }
                            } else {
                                Debug.LogWarning($"{path} found {string.Join(",\n", adrPaths)} as adr matches");
                            }
                        }
                    } catch (Exception e) {
                        Debug.LogError($"Individual Collider {path} failed");
                        Debug.LogException(e);
                    }
                }
            } catch (Exception e) {
                Debug.LogError("Failed to apply colliders");
                Debug.LogException(e);
            } finally {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static void addWallsToGameObject(ForgelightWorld toModify, GzneFile gzneFile) {
            if (gzneFile.wallMeshes.Count > 0) {
                if (toModify.invisibleWallsRoot == null) {
                    var parentGo = new GameObject("InvisibleWalls");
                    parentGo.transform.SetParent(toModify.transform, false);
                    toModify.invisibleWallsRoot = parentGo.transform;
                }

                foreach (var wallMesh in gzneFile.wallMeshes) {
                    var wallGo = new GameObject(wallMesh.name);
                    wallGo.transform.SetParent(toModify.invisibleWallsRoot, false);
                    var meshCollide = wallGo.AddComponent<MeshCollider>();
                    meshCollide.sharedMesh = wallMesh;
                }
            }

        }

        [MenuItem("ForgeLight/Add Invisible Walls to Current World")]
        public static void AddInvisibleWallsToExistingWorld() {
            // TODO: do i make this only use ForgelightWorld or do i keep the old way in?

            var gameObjects = FindObjectsOfType<GameObject>();
            List<GameObject> found = new();
            foreach (var gameObject in gameObjects) {
                if (gameObject.name.StartsWith("World_")) {
                    found.Add(gameObject);
                }
            }

            if (found.Count == 0) {
                Debug.LogError("Could not determine the world object for the current scene. Expecting it's name to start with World_");
                return;
            }
            if (found.Count > 1) {
                Debug.LogError($"Found too many possibly world candidates {found.Count}. Are there multiple worlds loaded?");
                return;
            }

            var foundObject = found[0];
            var worldName = foundObject.name.Replace("World_", "");
            var gzneFile = FindSingularGzne(worldName);
            if (gzneFile == null) {
                return;
            }

            PrefabUtility.EditPrefabContentsScope? scope = null;
            var toModify = foundObject;
            if (PrefabUtility.GetPrefabAssetType(foundObject) is PrefabAssetType.Regular or PrefabAssetType.Variant) {
                scope = new PrefabUtility.EditPrefabContentsScope(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(foundObject));
                // this'll never be null, but this sure is an interesting pattern (needing to ?. a nullable struct) from C#, thanks microsoft
                toModify = scope?.prefabContentsRoot;
            }

            // only add the walls if there is not already a game object for it
            var possiblyExistingWallsObj = toModify.transform.Find("InvisibleWalls");
            if (possiblyExistingWallsObj) {
                Debug.LogWarning("Walls object already exists, if you want to regenerate the walls objects, delete the existing one.", possiblyExistingWallsObj.gameObject);
            } else {
                if (!toModify.TryGetComponent<ForgelightWorld>(out var world)) {
                    world = toModify.AddComponent<ForgelightWorld>();
                    world.worldName = worldName;
                }

                addWallsToGameObject(world, gzneFile);
            }

            // be a good citizen to the unity world
            if (scope != null) {
                scope?.Dispose();
            }
        }

        public static GzneFile FindSingularGzne(string worldName) {
            var gzneFileAssetGuids = AssetDatabase.FindAssets($"glob:\"{worldName}.gzne\"");

            if (gzneFileAssetGuids.Length == 0) {
                Debug.LogError($"Could not find a world file to match calculated world name {worldName}");
                return null;
            }
            if (gzneFileAssetGuids.Length > 1) {
                Debug.LogError($"Found {gzneFileAssetGuids.Length} world files to match calculated world name {worldName}, only expecting one...");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<GzneFile>(AssetDatabase.GUIDToAssetPath(gzneFileAssetGuids[0]));
        }

        public static GcnkFile[] FindTilesForWorld(GzneFile gzneFile) {
            string worldName = gzneFile.name;

            List<GcnkFile> chunks = new();
            for (var x = gzneFile.StartX; x < gzneFile.WorldSize; x += gzneFile.TilePerChunkAxis) {
                for (var y = gzneFile.StartY; y < gzneFile.WorldSize; y += gzneFile.TilePerChunkAxis) {
                    var chunkFileName = $"{worldName}_{x}_{y}";

                    var gcnkFileAssetPaths = AssetDatabase.FindAssets($"glob:\"{chunkFileName}.gcnk\"").Select(AssetDatabase.GUIDToAssetPath).ToArray();
                    if (gcnkFileAssetPaths.Length == 0) {
                        continue;
                    }
                    if (gcnkFileAssetPaths.Length > 1) {
                        Debug.LogError($"Found {gcnkFileAssetPaths.Length} cunk files for {chunkFileName}, only expecting one...\nUsing the first one at {gcnkFileAssetPaths[0]}");
                    }

                    var chunk = AssetDatabase.LoadAssetAtPath<GcnkFile>(gcnkFileAssetPaths[0]);
                    chunks.Add(chunk);
                }
            }

            return chunks.ToArray();
        }

        public static void SetLightProperties(Light unityLight, RawLight rawLight) {
            unityLight.range = rawLight.Range;
            unityLight.color = rawLight.Color;
            unityLight.intensity = rawLight.Intensity;
            unityLight.lightmapBakeType = LightmapBakeType.Baked;
        }
    }
}
