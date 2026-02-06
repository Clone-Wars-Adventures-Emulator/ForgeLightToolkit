using System.Collections.Generic;

using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes
{
    public class CollisionGizmo : MonoBehaviour
    {
        public List<Vector4> BvhCenters = new();

        public List<Vector4> BvhSizes = new();

        void OnDrawGizmos()
        {
            for (var i = 0; i < BvhCenters.Count; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(BvhCenters[i], BvhSizes[i]);
            }
        }
    }
}