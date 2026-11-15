using System;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Map {
    [Serializable]
    public struct Node {
        public int Id;

        // Why is this a vector3? Isnt the map a 2d construct?
        public Vector3 Position;

        public Edge[] Edges;
    }

    [Serializable]
    public struct Edge {
        public int Id;

        public float Distance;
    }
}
