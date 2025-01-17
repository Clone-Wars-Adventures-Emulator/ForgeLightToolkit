using UnityEngine;

namespace ForgeLightToolkit.Runtime {
    // This class exists to persist the ADR and ID of a Forgelight Object during runetime, but mostly editor time. Removing these pre-build is a pain.
    public class ForgelightObject : MonoBehaviour {
        public string AdrFileName { get; set; }
        public int RuntimeObjectId { get; set; }
    }
}
