using System.Collections.Generic;

using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes
{
    public class CollisionGizmo : MonoBehaviour
    {
        public List<Vector4> BvhCenters = new();
        public List<Vector4> BvhSizes = new();
        public List<int> Depth;

        void OnDrawGizmos()
        {
            Color[] colors = {Color.purple, Color.blue, Color.green, Color.yellow, Color.orange, Color.red};
            for (var i = 0; i < BvhCenters.Count; i++)
            {
                var colorIndex = Depth[i] % colors.Length;
                Gizmos.color = colors[colorIndex];
                Gizmos.DrawWireCube(BvhCenters[i], BvhSizes[i]);
            }
        }
    }
}