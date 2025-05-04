using System;
using System.IO;

public class CompilationConstants {
    // game data path
    // Steam game data path 
    # if STEAM
        public static readonly string GAME_DATA_PATH = Path.GetFullPath("./UPGRADE/Catanks/", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
    # else
        public static readonly string GAME_DATA_PATH = Environment.CurrentDirectory;
    # endif

    // Get absolute path (recommended by Microsoft: https://learn.microsoft.com/en-us/dotnet/standard/io/file-path-formats#path-normalization)
    public static readonly string SAVE_DATA_PATH = Path.GetFullPath("./SAVE_DATA/", GAME_DATA_PATH);
}