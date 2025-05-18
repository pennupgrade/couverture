using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ClassicModeSaveStateManager : SaveStateManager {
    public static readonly string CLASSIC_MODE_SAVE_PATH = SaveStateManagerGameObject.CLASSIC_MODE_SAVE_FILE;
    public static ClassicModeSaveStateManager LoadClassicSave() {
        try {
            return LoadClassicInventory();
        }
        catch (FileNotFoundException) {
            CreateSaveDirectory(); // this shouldn't ever be used, but it's here just for safety
            ClassicModeSaveStateManager outManager = new();
            outManager.CreateNewSave(CLASSIC_MODE_SAVE_PATH);
            return outManager;
        }
    }

    private static ClassicModeSaveStateManager LoadClassicInventory() {
        CreateSaveDirectory();
        ClassicModeSaveStateManager outManager;
        using (StreamReader reader = new(CLASSIC_MODE_SAVE_PATH)) {
            string jsonData = reader.ReadToEnd();
            outManager = JsonUtility.FromJson<ClassicModeSaveStateManager>(jsonData);
            outManager.SetSaveLocation(CLASSIC_MODE_SAVE_PATH);
        }
        return outManager;
    }
}