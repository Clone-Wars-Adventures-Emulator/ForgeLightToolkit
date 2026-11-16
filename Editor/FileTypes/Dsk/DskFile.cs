using ForgeLightToolkit.Editor.XmlData;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Dsk {
    public class DskFile : ScriptableObject {
        public List<SkeletonNode> boneNodes = new();
        public List<Matrix4x4> boneLocalTransforms = new();

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
            if (magic != "DSKE") {
                return false;
            }

            var version = reader.ReadUInt32();
            if (version > 2) {
                Debug.LogError($"Unknown Skeleton version: {version}");
                return false;
            }

            char[] strings = reader.ReadChars(reader.ReadInt32());
            Dictionary<uint, string> offsetToStr = new();

            int len = 0;
            int lastStart = 0;
            for (uint offset = 0; offset < strings.Length; offset++) {
                if (strings[offset] == 0) {
                    var s = new string(new ReadOnlySpan<char>(strings, lastStart, len));
                    offsetToStr.Add((uint) lastStart, s);
                    lastStart = (int) offset + 1;
                    len = 0;
                } else {
                    len++;
                }
            }

            uint count = reader.ReadUInt32();
            for (int i = 0; i < count; i++) {
                uint nameOffset = reader.ReadUInt32();
                var node = new SkeletonNode() {
                    name = offsetToStr[nameOffset],
                    nameHash = JenkinsHelper.JenkinsOneAtATimeHash(offsetToStr[nameOffset]),
                    parentIndex = reader.ReadInt16(),
                    firstChildIndex = reader.ReadInt16(),
                };
                boneNodes.Add(node);
            }
            for (int i = 0; i < count; i++) {
                Vector3 r1 = reader.ReadVector3();
                Vector3 r2 = reader.ReadVector3();
                Vector3 r3 = reader.ReadVector3();
                Vector3 translate = reader.ReadVector3();

                var matrix = new Matrix4x4() {
                    m00 = r1.x, m01 = r1.y, m02 = r1.z, m03 = translate.x,
                    m10 = r2.x, m11 = r2.y, m12 = r2.z, m13 = translate.y,
                    m20 = r3.x, m21 = r3.y, m22 = r3.z, m23 = translate.z,
                    m30 = 0,    m31 = 0,    m32 = 0,    m33 = 1
                };
                boneLocalTransforms.Add(matrix);
            }

            return true;
        }

        public GameObject CreateSkeletonObjects() {
            Dictionary<int, MemTree> dict = new();
            for (int i = 0; i < boneNodes.Count; i++) {
                dict.Add(i, new MemTree() { idx = i, node = boneNodes[i] });
            }
            MemTree root = null;
            for (int i = 0; i < boneNodes.Count; i++) {
                var node = boneNodes[i];
                if (!dict.TryGetValue(i, out var t)) {
                    Debug.LogError($"Index {i} in {name} missing internal reference. There may be a bug.");
                    continue;
                }
                if (node.parentIndex == -1) {
                    if (root != null) {
                        Debug.LogError($"Possible duplicate root bones in {name}. saved: {root}, testing: {t}");
                    } else {
                        root = t;
                    }
                } else {
                    if (dict.TryGetValue(node.parentIndex, out var p)) {
                        t.parent = p;
                        p.children.Add(t);
                    } else {
                        Debug.LogError($"Index {i} in {name} references a non-existent parent");
                    }
                }
            }

            GameObject recurse(GameObject parent, MemTree node) {
                var go = new GameObject($"{node.node.name}");
                if (parent != null) {
                    go.transform.SetParent(parent.transform, false);
                }

                var localMat = boneLocalTransforms[node.idx];
                // negate the x position, seems to be an engine difference (but oddly, we dont need to negate anything else...???)
                var position = localMat.GetPosition();
                position.x = -position.x;
                // handle difference between ForgeLight (YXZ / HPR) and Unity (ZXY / RPH) by abusing the com.unity.mathematics package
                var rotEulerDeg = (Vector3) math.degrees(math.EulerYXZ(localMat.rotation));
                // we also need to negate the x here for some reason...
                rotEulerDeg.x = -rotEulerDeg.x;
                var rotation = Quaternion.Euler(rotEulerDeg);
                go.transform.SetLocalPositionAndRotation(position, rotation);
                go.transform.localScale = localMat.lossyScale;

                foreach (var child in node.children) {
                    recurse(go, child);
                }

                return go;
            }

            return recurse(null, root);
        }

        private class MemTree {
            public int idx;
            public SkeletonNode node;
            public MemTree parent;
            public readonly List<MemTree> children = new();

            public override string ToString() {
                return $"{idx} {parent} {string.Join(',', children)}";
            }
        }
    }

    [Serializable]
    public class SkeletonNode {
        public string name;
        public uint nameHash;
        public short parentIndex;
        public short firstChildIndex;
    }
}
