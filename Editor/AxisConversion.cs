using UnityEngine;

namespace ForgeLightToolkit.Editor {
    public static class AxisConversion {
        public static Vector4 Position(Vector4 pos) {
            // acount for zaxis inversion
            pos.z = -pos.z;
            return pos;
        }

        public static Vector3 Position(Vector3 pos) {
            // acount for zaxis inversion
            pos.z = -pos.z;
            return pos;
        }

        public static Quaternion Rotation(Vector4 eulerRot) {
            // Looks like they stored it as heading pitch roll in radians, but unity eulers are pitch, heading, roll in deg
            // to account for the zaxis inversion, invert the rotation on the x and y axis (heading and pitch)
            return Quaternion.Euler(-eulerRot.y * Mathf.Rad2Deg, -eulerRot.x * Mathf.Rad2Deg, eulerRot.z * Mathf.Rad2Deg);
        }
    }
}
