using System.IO;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using ForgeLightToolkit.Editor.FileTypes;
using ForgeLightToolkit.Editor.FileTypes.Dma;
using ForgeLightToolkit.Editor.FileTypes.Gcnk;

namespace ForgeLightToolkit.Editor
{
    public class LoadWorldWindow : EditorWindow
    {
        private string worldName = "";
        private string assetsPath = "Assets/ExtractedPacks";
        private string prefabSavePath = "Assets/Prefabs/Objects";
        private string materialsSavePath = "Assets/Materials";
        private string terrainMaterialsSavePath = "Assets/TerrainMaterials";
        private string worldPrefabSavePath = "Assets/Prefabs/Worlds";

        private bool _fastMode = false;
        private bool _overrideTerrainMaterials;
        private bool _overrideObjectMaterials;
        private bool _overrideObjectPrefabs;
        private bool _overrideWorldPrefab;
        private bool _overrideAllExistingAssets;

        private HashSet<string> objectsAlreadyProcessed;
        private HashSet<string> objectMaterialsAlreadyProcessed;


        [MenuItem("ForgeLight/Load World")]
        public static void ShowWindow()
        {
            GetWindow<LoadWorldWindow>("Load World");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width / EditorGUIUtility.pixelsPerPoint, Screen.height / EditorGUIUtility.pixelsPerPoint));

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Assets Path", EditorStyles.boldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Example: Assets/ForgeLight/CloneWarsAdventures", EditorStyles.miniBoldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            assetsPath = EditorGUILayout.TextField(assetsPath);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Object Prefab Save Location", EditorStyles.boldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Example: Assets/Prefabs/Objects", EditorStyles.miniBoldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            prefabSavePath = EditorGUILayout.TextField(prefabSavePath);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("World Prefab Save Location", EditorStyles.boldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Example: Assets/Prefabs/Worlds", EditorStyles.miniBoldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            worldPrefabSavePath = EditorGUILayout.TextField(worldPrefabSavePath);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Terrain Materials Save Location", EditorStyles.boldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Example: Assets/TerrainMaterials", EditorStyles.miniBoldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            terrainMaterialsSavePath = EditorGUILayout.TextField(terrainMaterialsSavePath);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Object Materials Save Location", EditorStyles.boldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Example: Assets/Materials", EditorStyles.miniBoldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            materialsSavePath = EditorGUILayout.TextField(materialsSavePath);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("World Name", EditorStyles.boldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Example: JediTemple", EditorStyles.miniBoldLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            worldName = EditorGUILayout.TextField(worldName);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label(
                "Please read the tooltips on the following boxes " +
                "to ensure you know what you are doing before " +
                "you run with any of the options selected.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            _fastMode = GUILayout.Toggle(_fastMode, new GUIContent("Fast Mode", "Loads directly from all original pack assets and skips saving any materials or prefabs"));
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            _overrideTerrainMaterials = GUILayout.Toggle(_overrideTerrainMaterials, new GUIContent("Override Terrain Materials", "Allows for reprocessing of terrain materials while maintaining all existing object prefabs and materials"));
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            _overrideObjectMaterials = GUILayout.Toggle(_overrideObjectMaterials, new GUIContent("Override Object Materials", "Allows for reprocessing of object materials while maintaining all existing object prefabs themselves and all existing terrain materials"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            _overrideObjectPrefabs = GUILayout.Toggle(_overrideObjectPrefabs, new GUIContent("Override Object Prefabs", "Allows for the reprocessing of object prefabs while maintaining all existing materials"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            _overrideWorldPrefab = GUILayout.Toggle(_overrideWorldPrefab, new GUIContent("Override World Prefab", "Allows for reprocessing of the .gzne/.gcnk files and allows the world prefab to be modified. Useful if you have just added a .dme or .adr you were previously missing"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            _overrideAllExistingAssets = GUILayout.Toggle(_overrideAllExistingAssets, new GUIContent("Override All Existing Assets", "Completely reprocesses the entire world(s) you are loading and overwrites any existing prefabs or materials of any kind"));
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(35);

            if (GUILayout.Button("Load World(s)", GUILayout.ExpandWidth(false)) && !string.IsNullOrEmpty(assetsPath) && !string.IsNullOrEmpty(prefabSavePath) && !string.IsNullOrEmpty(materialsSavePath)) {
                var gzneFileAssetGuids = AssetDatabase.FindAssets($"glob:\"{assetsPath}/{worldName}.gzne\"");

                OverrideUtil overrideUtil = new OverrideUtil(_fastMode, _overrideTerrainMaterials, _overrideObjectMaterials, _overrideObjectPrefabs, _overrideWorldPrefab, _overrideAllExistingAssets);
                objectsAlreadyProcessed = new HashSet<string>();
                objectMaterialsAlreadyProcessed = new HashSet<string>();

                foreach (var gzneFileAssetGuid in gzneFileAssetGuids) {
                    var gzneFileAssetPath = AssetDatabase.GUIDToAssetPath(gzneFileAssetGuid);

                    var gzneFile = AssetDatabase.LoadAssetAtPath<GzneFile>(gzneFileAssetPath);

                    if (gzneFile is null)
                        continue;

                    LoadWorld(gzneFile.name, overrideUtil);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void LoadWorld(string worldName, OverrideUtil overrideUtil) {
            GzneFile gzneFile = AssetDatabase.LoadAssetAtPath<GzneFile>(Path.Combine(assetsPath, $"{worldName}.gzne"));

            if (gzneFile is null)
                return;

            GameObject loadedWorldObject = null;

            if (overrideUtil.shouldAttemptWorldLoad()) {
                loadedWorldObject = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(worldPrefabSavePath, $"World_{worldName}.prefab"));
            }
            if (loadedWorldObject is not null && !overrideUtil.shouldProcessWorld()) {
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

                    if (gcnkFile is null)
                        continue;

                    var chunkObject = new GameObject($"Chunk ({gcnkFile.Coords.x}, {gcnkFile.Coords.y})") {
                        transform =
                        {
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
                            Material loadedChunkMaterial = null;
                            if (overrideUtil.shouldLoadTerrainMaterials()) {
                                loadedChunkMaterial = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(terrainMaterialsSavePath, gcnkFile.name + "_" + tile.Index.ToString() + ".mat"));
                            }

                            var chunkMaterial = new Material(Shader.Find($"Custom/RuntimeTerrain_{tile.EcoDataList.Count}")) {
                                name = $"Tile {tile.Index}"
                            };

                            if (loadedChunkMaterial is not null && !overrideUtil.isFastMode()) {
                                chunkMaterial = loadedChunkMaterial;
                                chunkMaterials[tile.Index] = chunkMaterial;
                                continue;
                            }
                            if (gck2File is not null)
                                chunkMaterial.mainTexture = gck2File.Texture;

                            if (gcnkFile.DetailMask is not null)
                                chunkMaterial.SetTexture("_DetailMaskMap", gcnkFile.DetailMask);

                            for (var i = 0; i < tile.EcoDataList.Count; i++) {
                                var ecoDataIndex = tile.EcoDataList[i];
                                var ecoData = gzneFile.EcoData[ecoDataIndex];

                                chunkMaterial.SetFloat($"_DetailRepeat{i}", ecoData.Scale);

                                var ecoDataTextureFilePath = Path.Combine(assetsPath, Path.ChangeExtension(ecoData.Texture, "png"));

                                var ecoDataTexture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(ecoDataTextureFilePath);

                                chunkMaterial.SetTexture($"_DetailColorMap{i}", ecoDataTexture2d);
                            }
                            if (overrideUtil.shouldSaveTerrainMaterials()) {
                                AssetDatabase.CreateAsset(chunkMaterial, Path.Combine(terrainMaterialsSavePath, gcnkFile.name + "_" + tile.Index.ToString() + ".mat"));
                            }
                            chunkMaterials[tile.Index] = chunkMaterial;
                        }
                        chunkMeshRenderer.materials = chunkMaterials;
                    }

                    foreach (var tile in gcnkFile.Tiles) {
                        foreach (var runtimeObject in tile.RuntimeObjects) {
                            if (runtimeObject.Unknown > 0) {
                                if (!loadedRuntimeObjects.TryAdd(runtimeObject.Unknown, runtimeObject))
                                    continue;
                            } else {
                                if (!loadedRuntimeObjects.TryAdd(runtimeObject.ObjectId, runtimeObject))
                                    continue;
                            }

                            var fileExtension = Path.GetExtension(runtimeObject.FileName);

                            if (fileExtension == ".adr") {
                                LoadAdrFile(assetsPath, runtimeObject.FileName, chunkObject, runtimeObject.Position, runtimeObject.Scale, runtimeObject.Rotation, overrideUtil);
                            } else if (fileExtension == ".agr") {
                                var agrFilePath = Path.Combine(assetsPath, runtimeObject.FileName);

                                var agrFile = AssetDatabase.LoadAssetAtPath<AgrFile>(agrFilePath);

                                if (agrFile is null) {
                                    Debug.LogError($"Failed to load Agr. {agrFilePath}");
                                    continue;
                                }

                                foreach (var actor in agrFile.ActorSet.Actors) {
                                    LoadAdrFile(assetsPath, actor.Name, chunkObject, runtimeObject.Position, runtimeObject.Scale, runtimeObject.Rotation, overrideUtil);
                                }
                            }
                        }

                        foreach (var rawLight in tile.RawLights) {
                            var lightObject = new GameObject($"Light ({rawLight.Name})") {
                                transform =
                                {
                                    parent = chunkObject.transform,
                                    position = rawLight.Position
                                }
                            };

                            var lightComp = lightObject.AddComponent<Light>();

                            lightComp.range = rawLight.Range;
                            lightComp.color = rawLight.Color;
                            lightComp.intensity = rawLight.Intensity;
                            lightComp.lightmapBakeType = LightmapBakeType.Baked;
                        }
                    }
                }
            }
            worldObject.transform.localScale = new Vector3(1, 1, -1);

            if (loadedWorldObject is not null && !overrideUtil.shouldProcessWorld()) {
                DestroyImmediate(worldObject);
                PrefabUtility.InstantiatePrefab(loadedWorldObject, worldObject.transform);
                return;
            }

            if (overrideUtil.shouldSaveWorld() || (loadedWorldObject is null && !overrideUtil.isFastMode())) {
                PrefabUtility.SaveAsPrefabAssetAndConnect(worldObject, Path.Combine(worldPrefabSavePath, worldObject.name + ".prefab"), InteractionMode.AutomatedAction);
            }
        }

        private void LoadAdrFile(string assetsPath, string adrFileName, GameObject parentObject, Vector4 position, float scale, Vector4 rotation, OverrideUtil overrideUtil) {
            var adrFilePath = Path.Combine(assetsPath, adrFileName);

            var adrFile = AssetDatabase.LoadAssetAtPath<AdrFile>(adrFilePath);
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(prefabSavePath, Path.ChangeExtension(adrFileName, "prefab")));
            if (existingPrefab is not null && (!overrideUtil.shouldProcessWorldObject() || objectsAlreadyProcessed.Contains(adrFileName.Split(".")[0]))) {
                GameObject loadedObject = PrefabUtility.InstantiatePrefab(existingPrefab, parentObject.transform) as GameObject;
                loadedObject.transform.localPosition = position;
                loadedObject.transform.localScale = Vector3.one * scale;
                loadedObject.transform.localRotation = Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg);
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
                transform =
                {
                    parent = parentObject.transform,
                    localPosition = position,
                    localScale = Vector3.one * scale,
                    localRotation = Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg)
                }
            };

            foreach (var meshEntry in dmeFile.Meshes) {
                var meshObject = new GameObject() {
                    transform =
                    {
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

                if (materialDefinition is null)
                    continue;

                var materialShader = Shader.Find($"Custom/{materialDefinition.Name}");

                if (materialShader is null) {
                    Debug.LogWarning($"Missing Shader \"{materialDefinition.Name}\" for Object \"{adrFileName}\".");
                    continue;
                }

                Material objectMaterial = new Material(materialShader);

                var matFileName = "";
                Material loadedMat = null;

                foreach (var parameterEntry in materialEntry.ParameterEntries) {
                    if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_OBJECT && (overrideUtil.shouldLoadObjectMaterials() || objectMaterialsAlreadyProcessed.Contains(Path.ChangeExtension(materialDefinition.Name + "_" + dmeFile.DmaFile.Textures.FirstOrDefault(x => JenkinsHelper.JenkinsOneAtATimeHash(x.ToUpper()) == parameterEntry.Object), "mat")))) {
                        var textureName = dmeFile.DmaFile.Textures.FirstOrDefault(x => JenkinsHelper.JenkinsOneAtATimeHash(x.ToUpper()) == parameterEntry.Object);
                        if (textureName is null) textureName = "SOMETHING_HAS_GONE_WRONG.mat";
                        matFileName = Path.ChangeExtension(materialDefinition.Name + "_" + textureName, "mat");
                        loadedMat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(materialsSavePath, matFileName));
                    }
                }

                if (loadedMat != null) {
                    objectMeshRenderer.material = loadedMat;
                    meshObject.name = meshEntry.Mesh.name;
                    continue;
                }

                foreach (var parameterEntry in materialEntry.ParameterEntries) {
                    var parameterName = $"_{(ParameterName)parameterEntry.Hash}";

                    if (!objectMaterial.HasProperty(parameterName))
                        Debug.LogWarning($"{materialDefinition.Name}\t{parameterName}\t{parameterEntry.Class}\t{parameterEntry.Type}\t{parameterEntry.Int}\t{parameterEntry.Float}\t{parameterEntry.Vector4}\t{parameterEntry.Matrix4x4}\t{parameterEntry.Object}");

                    if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_SCALAR) {
                        if (parameterEntry.Type == D3DXPARAMETER_TYPE.D3DXPT_FLOAT)
                            objectMaterial.SetFloat(parameterName, parameterEntry.Float);
                        else
                            objectMaterial.SetInteger(parameterName, parameterEntry.Int);
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

                        matFileName = Path.ChangeExtension(materialDefinition.Name + "_" + textureName, "mat");
                        objectMaterial.name = textureName.Split(".")[0];
                    }
                }
                if (overrideUtil.shouldSaveObjectMaterials()) {
                    if (matFileName == "") matFileName = "FUCK.mat";
                    AssetDatabase.CreateAsset(objectMaterial, Path.Combine(materialsSavePath, matFileName));
                    objectMaterialsAlreadyProcessed.Add(matFileName);
                    meshObject.name = meshEntry.Mesh.name;
                    matFileName = "";
                }

                objectMeshRenderer.material = objectMaterial;
            }

            if (overrideUtil.shouldLoadWorldObjects() && existingPrefab is not null) {
                DestroyImmediate(runtimeObject);
                GameObject go = Instantiate(existingPrefab);
                go.transform.localPosition = position;
                go.transform.localScale = Vector3.one * scale;
                go.transform.localRotation = Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg);
                return;
            }

            if (overrideUtil.shouldSaveWorldObjects()) {
                PrefabUtility.SaveAsPrefabAssetAndConnect(runtimeObject, Path.Combine(prefabSavePath, runtimeObject.name + ".prefab"), InteractionMode.AutomatedAction);
                objectsAlreadyProcessed.Add(runtimeObject.name);
            }
        }
    }
}