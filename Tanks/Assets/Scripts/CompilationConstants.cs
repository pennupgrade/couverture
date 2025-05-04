public class CompilationConstants {
    // save file path
    public const string SAVE_DATA_PATH =
        # if STEAM && UNITY_STANDALONE_WIN
            "";
        # elif STEAM && UNITY_STANDALONE_LINUX
            "";
        # elif STEAM && UNITY_STANDALONE_OSX
            ""; // TODO: FINISH
        # else
            "SAVE_DATA/";
        # endif
}