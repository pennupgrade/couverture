using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class StaticSaveStateManager : SaveStateManager {
    public static readonly string CLASSIC_MODE_SAVE_PATH = SaveStateManagerGameObject.CLASSIC_MODE_SAVE_FILE;
    public static StaticSaveStateManager LoadClassicSave() {
        try {
            return LoadClassicInventory();
        }
        catch (FileNotFoundException) {
            CreateSaveDirectory(); // this shouldn't ever be used, but it's here just for safety
            StaticSaveStateManager outManager = new();
            outManager.CreateNewSave(CLASSIC_MODE_SAVE_PATH);
            return outManager;
        }
    }

    private static StaticSaveStateManager LoadClassicInventory() {
        CreateSaveDirectory();
        StaticSaveStateManager outManager;
        using (StreamReader reader = new(CLASSIC_MODE_SAVE_PATH)) {
            string jsonData = reader.ReadToEnd();
            outManager = JsonUtility.FromJson<StaticSaveStateManager>(jsonData);
            outManager.SetSaveLocation(CLASSIC_MODE_SAVE_PATH);
        }
        return outManager;
    }
}