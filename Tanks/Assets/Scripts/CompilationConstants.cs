using System;
using System.IO;

public class CompilationConstants {
    // save file path
    // Steam save file paths 
    # if STEAM
        public static readonly string SAVE_DATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/UPGRADE/Catanks/SAVE_DATA/";
    # else
        // Get absolute path (recommended by Microsoft: https://learn.microsoft.com/en-us/dotnet/standard/io/file-path-formats#path-normalization)
       public static readonly string SAVE_DATA_PATH = Path.GetFullPath("SAVE_DATA/");
    # endif
}