namespace ForgeLightToolkit.Editor {
    public static class QueueOffsets {
        /// <summary>
        /// Use this for things that require PNGS to be loaded or other assets that are imported by unity or do not have an import order
        /// </summary>
        public const int STAGE_1_POST_IMAGES = 2;
        public const int STAGE_2_ADR = 6;
        public const int STAGE_3_POST_ADR = 15;
    }
}
