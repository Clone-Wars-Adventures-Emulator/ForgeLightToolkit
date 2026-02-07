using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes
{
    public class CollisionGizmo : MonoBehaviour
    {
        public List<Vector4> BvhCenters = new();
        public List<Vector4> BvhSizes = new();
        public List<Color> Colors = new();

        void OnDrawGizmos()
        {
            for (var i = 0; i < BvhCenters.Count; i++)
            {
                Gizmos.color = Colors[i];
                Gizmos.DrawWireCube(BvhCenters[i], BvhSizes[i]);
            }
        }
    }
}