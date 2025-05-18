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


    public enum Achievement {
        NO_SKIP_CLASSIC, NO_SKIP_CLASSIC_PART_TWO, NO_SKIP_CLASSIC_PART_ONE
    }

    private class AchievementChecker {
        private Func<bool> unlockTest;
        private HashSet<AchievementChecker>[] addedLists;
        private Achievement correspondingAchievement;
        private StaticSaveStateManager outer;

        public AchievementChecker(StaticSaveStateManager outerClass, Func<bool> test, HashSet<AchievementChecker>[] toAddLists, Achievement achievement) {
            unlockTest = test;
            addedLists = toAddLists;
            correspondingAchievement = achievement;
            outer = outerClass;
            foreach (HashSet<AchievementChecker> i in toAddLists) {
                i.Add(this);
            }
        }

        public void attemptUnlock() {
            if (unlockTest()) {
                foreach (HashSet<AchievementChecker> i in addedLists) { // when calling this make sure not in a foreach loop of the lists (create a copy)
                    i.Remove(this);
                }
                outer.unlockedAchievements.Add(correspondingAchievement);
            }
        }
    }

    [SerializeField] private int classicModeHighScore;
    [SerializeField] private List<Achievement> unlockedAchievements;


    public bool inClassicMode = false;
    
    // TODO: Map achievements to AchievementCheckers, create lists, connect lists to SaveStateManagerGameObject
    
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

    public override void ExitSaveFile()
    {
        inClassicMode = false;
        base.ExitSaveFile();
    }

}