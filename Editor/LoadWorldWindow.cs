using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using ForgeLightToolkit.Editor.FileTypes;
using ForgeLightToolkit.Editor.FileTypes.Dma;
using ForgeLightToolkit.Editor.FileTypes.Gcnk;

namespace ForgeLightToolkit.Editor
{
    public class LoadWorldWindow : EditorWindow
    {
        private string _worldName = "";
        private string _assetsPath = "";
        private string _prefabSavePath = "";
        private string _materialsSavePath = "";

        private bool _loadLights = true;
        private bool _loadObjects = true;
        private bool _skipAssetSaving = false;
        private bool _ignoreExistingAssets = false;

        [MenuItem("ForgeLight/Load World")]
        public static void ShowWindow()
        {
            GetWindow<LoadWorldWindow>("Load World");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width / EditorGUIUtility.pixelsPerPoint, Screen.height / EditorGUIUtility.pixelsPerPoint));

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Assets Path", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Example: Assets/ForgeLight/CloneWarsAdventures", EditorStyles.miniBoldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _assetsPath = EditorGUILayout.TextField(_assetsPath, GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Prefab Save Location", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Example: Assets/Prefabs/Worlds", EditorStyles.miniBoldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _prefabSavePath = EditorGUILayout.TextField(_prefabSavePath, GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Materials Save Location", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Example: Assets/Materials", EditorStyles.miniBoldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _materialsSavePath = EditorGUILayout.TextField(_materialsSavePath, GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("World Name", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Example: JediTemple", EditorStyles.miniBoldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _worldName = EditorGUILayout.TextField(_worldName, GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _loadLights = GUILayout.Toggle(_loadLights, "Lights");

            GUILayout.Space(20);

            _loadObjects = GUILayout.Toggle(_loadObjects, "Objects");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _skipAssetSaving = GUILayout.Toggle(_skipAssetSaving, "Skip Asset Saving");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _ignoreExistingAssets = GUILayout.Toggle(_ignoreExistingAssets, "Ignore Existing Assets");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Load World(s)", GUILayout.ExpandWidth(false)) && !string.IsNullOrEmpty(_assetsPath) && !string.IsNullOrEmpty(_prefabSavePath) && !string.IsNullOrEmpty(_materialsSavePath)) {
                var gzneFileAssetGuids = AssetDatabase.FindAssets($"glob:\"{_assetsPath}/{_worldName}.gzne\"");

                foreach (var gzneFileAssetGuid in gzneFileAssetGuids) {
                    var gzneFileAssetPath = AssetDatabase.GUIDToAssetPath(gzneFileAssetGuid);

                    var gzneFile = AssetDatabase.LoadAssetAtPath<GzneFile>(gzneFileAssetPath);

                    if (gzneFile is null)
                        continue;

                    LoadWorld(gzneFile.name, _assetsPath, _loadObjects, _loadLights, _prefabSavePath);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void LoadWorld(string worldName, string assetsPath, bool loadObjects, bool loadLights, string prefabSaveLocation) {
            var gzneFile = AssetDatabase.LoadAssetAtPath<GzneFile>($"{assetsPath}/{worldName}.gzne");

            if (gzneFile is null)
                return;

            var worldObject = new GameObject($"World_{worldName}");

            var loadedRuntimeObjects = new Dictionary<int, RuntimeObject>();

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
                            var loadedChunkMaterial = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(_materialsSavePath, gcnkFile.name + "_" + tile.Index.ToString() + ".mat"));
                            var chunkMaterial = new Material(Shader.Find($"Custom/RuntimeTerrain_{tile.EcoDataList.Count}")) {
                                name = $"Tile {tile.Index}"
                            };

                            if (loadedChunkMaterial is not null && !_ignoreExistingAssets) {
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
                            if (!_skipAssetSaving) {
                                AssetDatabase.CreateAsset(chunkMaterial, Path.Combine(_materialsSavePath, gcnkFile.name + "_" + tile.Index.ToString() + ".mat"));
                            }

                            chunkMaterials[tile.Index] = chunkMaterial;
                        }

                        chunkMeshRenderer.materials = chunkMaterials;
                    }

                    foreach (var tile in gcnkFile.Tiles) {
                        if (loadObjects) {
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
                                    LoadAdrFile(assetsPath, runtimeObject.FileName, chunkObject, runtimeObject.Position, runtimeObject.Scale, runtimeObject.Rotation);
                                } else if (fileExtension == ".agr") {
                                    var agrFilePath = Path.Combine(assetsPath, runtimeObject.FileName);

                                    var agrFile = AssetDatabase.LoadAssetAtPath<AgrFile>(agrFilePath);

                                    if (agrFile is null) {
                                        Debug.LogError($"Failed to load Agr. {agrFilePath}");
                                        continue;
                                    }

                                    foreach (var actor in agrFile.ActorSet.Actors) {
                                        LoadAdrFile(assetsPath, actor.Name, chunkObject, runtimeObject.Position, runtimeObject.Scale, runtimeObject.Rotation);
                                    }
                                }
                            }
                        }

                        if (loadLights) {
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
            }
            worldObject.transform.localScale = new Vector3(1, 1, -1);
            if (!_skipAssetSaving) {
                PrefabUtility.SaveAsPrefabAssetAndConnect(worldObject, Path.Combine(_prefabSavePath, "Worlds", worldObject.name + ".prefab"), InteractionMode.AutomatedAction);
            }
        }

        private void LoadAdrFile(string assetsPath, string adrFileName, GameObject parentObject, Vector4 position, float scale, Vector4 rotation)
        {
            var adrFilePath = Path.Combine(assetsPath, adrFileName);

            var adrFile = AssetDatabase.LoadAssetAtPath<AdrFile>(adrFilePath);

            if (adrFile is null)
            {
                Debug.LogError($"Failed to load Adr. {adrFilePath}");
                return;
            }

            if (adrFile.ModelFileName is null)
            {
                Debug.LogError($"Adr has no model file name. {adrFilePath}");
                return;
            }

            var dmeFilePath = Path.Combine(assetsPath, adrFile.ModelFileName);

            var dmeFile = AssetDatabase.LoadAssetAtPath<DmeFile>(dmeFilePath);

            if (dmeFile is null)
            {
                Debug.LogError($"Failed to load Dme. {dmeFilePath}");
                return;
            }
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(_prefabSavePath, "Objects", Path.ChangeExtension(adrFileName, "prefab")));

            if (existingPrefab is not null && !_ignoreExistingAssets) {
                GameObject loadedObject = PrefabUtility.InstantiatePrefab(existingPrefab, parentObject.transform) as GameObject;
                loadedObject.transform.localPosition = position;
                loadedObject.transform.localScale = Vector3.one * scale;
                loadedObject.transform.localRotation = Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg);
                return;
            }

            var runtimeObject = new GameObject(adrFileName.Split(".")[0])
            {
                transform =
                {
                    parent = parentObject.transform,
                    localPosition = position,
                    localScale = Vector3.one * scale,
                    localRotation = Quaternion.Euler(rotation.y * Mathf.Rad2Deg, rotation.x * Mathf.Rad2Deg, rotation.z * Mathf.Rad2Deg)
                }
            };

            foreach (var meshEntry in dmeFile.Meshes)
            {
                var meshObject = new GameObject()
                {
                    transform =
                    {
                        parent = runtimeObject.transform,
                        localPosition = Vector3.zero,
                        localScale = Vector3.one,
                        localRotation = Quaternion.identity
                    }
                };

                var objectMeshFilter = meshObject.AddComponent<MeshFilter>();

                var loadedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(Path.Combine(_materialsSavePath, meshObject.name + ".asset"));

                if (loadedMesh != null && !_ignoreExistingAssets) {
                    objectMeshFilter.sharedMesh = loadedMesh;
                }
                else {
                    objectMeshFilter.sharedMesh = meshEntry.Mesh;
                }

                var objectMeshRenderer = meshObject.AddComponent<MeshRenderer>();

                var materialEntry = dmeFile.DmaFile.MaterialEntries[meshEntry.MaterialIndex];

                var materialDefinition = MaterialInfo.Instance.MaterialDefinitions.FirstOrDefault(x => x.NameHash == materialEntry.Hash);

                if (materialDefinition is null)
                    continue;

                var materialShader = Shader.Find($"Custom/{materialDefinition.Name}");

                if (materialShader is null)
                {
                    Debug.LogWarning($"Missing Shader \"{materialDefinition.Name}\" for Object \"{adrFileName}\".");
                    continue;
                }

                Material objectMaterial = new Material(materialShader);

                var matFileName = "";
                Material loadedMat = null;

                foreach (var parameterEntry in materialEntry.ParameterEntries) {
                    if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_OBJECT) {
                        var textureName = dmeFile.DmaFile.Textures.FirstOrDefault(x => JenkinsHelper.JenkinsOneAtATimeHash(x.ToUpper()) == parameterEntry.Object);
                        if (textureName is null) textureName = "SOMETHING_HAS_GONE_WRONG.mat";
                        matFileName = Path.ChangeExtension(textureName, "mat");
                        loadedMat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(_materialsSavePath, matFileName));
                    }
                }

                if (loadedMat != null) {
                    objectMeshRenderer.material = loadedMat;
                    meshObject.name = loadedMat.name;
                    continue;
                }

                foreach (var parameterEntry in materialEntry.ParameterEntries)
                {
                    var parameterName = $"_{(ParameterName)parameterEntry.Hash}";

                    if (!objectMaterial.HasProperty(parameterName))
                        Debug.LogWarning($"{materialDefinition.Name}\t{parameterName}\t{parameterEntry.Class}\t{parameterEntry.Type}\t{parameterEntry.Int}\t{parameterEntry.Float}\t{parameterEntry.Vector4}\t{parameterEntry.Matrix4x4}\t{parameterEntry.Object}");

                    if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_SCALAR)
                    {
                        if (parameterEntry.Type == D3DXPARAMETER_TYPE.D3DXPT_FLOAT)
                            objectMaterial.SetFloat(parameterName, parameterEntry.Float);
                        else
                            objectMaterial.SetInteger(parameterName, parameterEntry.Int);
                    }
                    else if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_VECTOR)
                    {
                        objectMaterial.SetVector(parameterName, parameterEntry.Vector4);
                    }
                    else if (parameterEntry.Class is D3DXPARAMETER_CLASS.D3DXPC_MATRIX_ROWS or D3DXPARAMETER_CLASS.D3DXPC_MATRIX_COLUMNS)
                    {
                        objectMaterial.SetMatrix(parameterName, parameterEntry.Matrix4x4);
                    }
                    else if (parameterEntry.Class == D3DXPARAMETER_CLASS.D3DXPC_OBJECT)
                    {
                        var textureHash = parameterEntry.Object;

                        var textureName = dmeFile.DmaFile.Textures.FirstOrDefault(x => JenkinsHelper.JenkinsOneAtATimeHash(x.ToUpper()) == textureHash);

                        if (textureName is null)
                        {
                            Debug.LogError($"Failed to find texture. {textureHash}");
                            continue;
                        }

                        var textureFilePath = Path.Combine(assetsPath, Path.ChangeExtension(textureName, "png"));

                        var texture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(textureFilePath);

                        if (texture2d is null)
                        {
                            Debug.LogError($"Failed to find texture. {textureFilePath}");
                            continue;
                        }

                        objectMaterial.SetTexture(parameterName, texture2d);
                        objectMaterial.SetTextureScale(parameterName, Vector2.right + Vector2.down);

                        matFileName = Path.ChangeExtension(textureName, "mat");
                        objectMaterial.name = textureName.Split(".")[0];
                    }
                }
                if (!AssetDatabase.Contains(objectMaterial) && matFileName != "" && !_skipAssetSaving) {
                    AssetDatabase.CreateAsset(objectMaterial, Path.Combine(_materialsSavePath, matFileName));
                    meshObject.name = matFileName;
                    matFileName = "";
                }

                objectMeshRenderer.material = objectMaterial;
            }
            if (!_skipAssetSaving) {
                PrefabUtility.SaveAsPrefabAssetAndConnect(runtimeObject, Path.Combine(_prefabSavePath, "Objects", runtimeObject.name + ".prefab"), InteractionMode.AutomatedAction);
            }
        }
    }
}