using UnityEngine;

namespace ForgeLightToolkit.Runtime {
    // This class exists to persist the information about a Forgelight Light during runetime, but mostly editor time.
    // Removing this data pre-build is a pain, so just persist what could be used at runtime in a single object.
    public class ForgelightLight : MonoBehaviour {
        public string uniqueLightId;

        public static string CreateUniqueLightId(int tileX, int tileY, int lightIndex, string colorName) {
            return $"flLight:{tileX}x{tileY}-{lightIndex}_{colorName}";
        }
    }
}
