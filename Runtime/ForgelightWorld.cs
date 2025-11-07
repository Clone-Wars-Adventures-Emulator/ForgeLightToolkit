using UnityEngine;

namespace ForgeLightToolkit.Runtime {
    // This class exists to persist the information about a Forgelight world during runetime, but mostly editor time.
    // Removing this data pre-build is a pain, so just persist what could be used at runtime in a single object.
    public class ForgelightWorld : MonoBehaviour {
        public string worldName;
        public Transform invisibleWallsRoot;
    }
}
