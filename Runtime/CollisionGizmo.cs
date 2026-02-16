using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeLightToolkit.Runtime.EditorDebug {
    public class CollisionGizmo : MonoBehaviour {
        public class BvhData {
            public Vector4 center;
            public Vector4 size;
            public Color color;
        }

        [NonSerialized]
        public readonly List<BvhData> bvhData = new();

        void OnDrawGizmos() {
            foreach (var data in bvhData) {
                Gizmos.color = data.color;
                Gizmos.DrawWireCube(data.center, data.size);
            }
        }
    }
}
