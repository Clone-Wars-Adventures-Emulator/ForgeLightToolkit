
namespace ForgeLightToolkit.Editor {
    public class OverrideUtil {
        private bool fastMode;
        private bool overrideTerrainMaterials;
        private bool overrideObjectMaterials;
        private bool overrideObjectPrefabs;
        private bool overrideWorldPrefab;
        private bool overrideAllExistingAssets;

        public OverrideUtil(bool fastMode, bool overrideTerrainMaterials, bool overrideObjectMaterials, bool overrideObjectPrefabs, bool overrideWorldPrefab, bool overrideAllExistingAssets) {
            this.fastMode = fastMode;
            this.overrideTerrainMaterials = overrideTerrainMaterials;
            this.overrideObjectMaterials = overrideObjectMaterials;
            this.overrideObjectPrefabs = overrideObjectPrefabs;
            this.overrideWorldPrefab = overrideWorldPrefab;
            this.overrideAllExistingAssets = overrideAllExistingAssets;
        }
        public bool isFastMode() {
            return fastMode;
        }

        public bool shouldAttemptWorldLoad() {
            return !fastMode
                && !overrideAllExistingAssets
                && !overrideWorldPrefab;
        }

        public bool shouldProcessWorld() {
            return fastMode || !(!overrideTerrainMaterials
                && !overrideObjectMaterials
                && !overrideObjectPrefabs
                && !overrideAllExistingAssets);
        }

        public bool shouldSaveWorld() {
            return !fastMode
                && overrideWorldPrefab;
        }

        public bool shouldLoadTerrainMaterials() {
            return !fastMode
                && !overrideTerrainMaterials;
        }
        public bool shouldSaveTerrainMaterials() {
            return !fastMode
                && overrideTerrainMaterials;
        }

        public bool shouldLoadObjectMaterials() {
            return !fastMode
                && !overrideObjectMaterials;
        }

        public bool shouldSaveObjectMaterials() {
            return !fastMode
                && overrideObjectMaterials;
        }

        public bool shouldProcessWorldObject() {
            return fastMode
                || overrideObjectPrefabs
                || overrideObjectMaterials;
        }

        public bool shouldSaveWorldObjects() {
            return !fastMode
                && overrideObjectPrefabs;
        }
        
        public bool shouldLoadWorldObjects() {
            return !fastMode
                && !overrideObjectPrefabs;
        }
    }
}