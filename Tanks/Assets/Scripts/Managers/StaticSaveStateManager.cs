using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class StaticSaveStateManager : SaveStateManager {
    public static readonly string CLASSIC_MODE_SAVE_PATH = SaveStateManagerGameObject.CLASSIC_MODE_SAVE_FILE;
    public static StaticSaveStateManager LoadStaticSave() {
        CreateSaveDirectory(); // this shouldn't ever be used, but it's here just for safety
        try {
            StaticSaveStateManager outManager;
            using (StreamReader reader = new(CLASSIC_MODE_SAVE_PATH)) {
                string jsonData = reader.ReadToEnd();
                outManager = JsonUtility.FromJson<StaticSaveStateManager>(jsonData);
                outManager.SetSaveLocation(CLASSIC_MODE_SAVE_PATH);
            }
            return outManager;
        }
        catch (FileNotFoundException) {
            StaticSaveStateManager outManager = new();
            outManager.CreateNewSave(CLASSIC_MODE_SAVE_PATH);
            return outManager;
        }
    }




    [SerializeField] private int classicModeHighScore;
    [SerializeField] private List<Achievement> unlockedAchievements;
    
    public enum Achievement {
        NO_SKIP_CLASSIC, NO_SKIP_CLASSIC_PART_TWO, NO_SKIP_CLASSIC_PART_ONE
    }


    public void UpdateClassicModeHighScore(int newScore) {
        // checks if new score is larger than current max score
        if (newScore > classicModeHighScore) {
            classicModeHighScore = newScore;
        }
    }

    public int GetClassicModeHighScore() => classicModeHighScore;

    public HashSet<Achievement> GetAchievements() {
        return new(unlockedAchievements);
    }

}