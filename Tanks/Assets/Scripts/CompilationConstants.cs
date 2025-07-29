// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using System.IO;
using UnityEngine;

public class CompilationConstants {
    // game data path
    // Steam game data path 
    #if !DISABLESTEAMWORKS
        public static readonly string GAME_DATA_PATH = Path.GetFullPath("./UPGRADE/Catanks/", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
    #elif UNITY_EDITOR // game data path when using unity editor
        public static readonly string GAME_DATA_PATH = Environment.CurrentDirectory;
    #else // itch.io, github release game data path
        public static readonly string GAME_DATA_PATH = Path.GetDirectoryName(Application.dataPath);
    #endif

    // Get absolute path (recommended by Microsoft: https://learn.microsoft.com/en-us/dotnet/standard/io/file-path-formats#path-normalization)
    public static readonly string SAVE_DATA_PATH = Path.GetFullPath("./SAVE_DATA/", GAME_DATA_PATH);
}