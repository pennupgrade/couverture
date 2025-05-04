public class CompilationConstants {
    // save file path
        // Steam save file paths 
        # if STEAM && UNITY_STANDALONE_WIN
            public const string SAVE_DATA_PATH = "";
        # elif STEAM && UNITY_STANDALONE_LINUX
            public const string SAVE_DATA_PATH = "";
        # elif STEAM && UNITY_STANDALONE_OSX
            public const string SAVE_DATA_PATH = "";
        // Non-Steam save file paths
        # elif UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX
            public const string SAVE_DATA_PATH = "SAVE_DATA/";
        # elif UNITY_STANDALONE_WIN
            // Windows has a different syntax for path separators
            public const string SAVE_DATA_PATH = "SAVE_DATA\\";
        # else
            // shouldn't occur
            public const string SAVE_DATA_PATH = null;
        # endif
}