using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class StaticSaveStateManager : SaveStateManager {
    public static readonly string CLASSIC_MODE_SAVE_PATH = SaveStateManagerGameObject.CLASSIC_MODE_SAVE_FILE;
    public static StaticSaveStateManager LoadStaticSave() {
        CreateSaveDirectory();
        try {
            StaticSaveStateManager outManager;
            using (StreamReader reader = new(CLASSIC_MODE_SAVE_PATH)) {
                string jsonData = reader.ReadToEnd();
                outManager = JsonUtility.FromJson<StaticSaveStateManager>(jsonData);
                outManager.SetSaveLocation(CLASSIC_MODE_SAVE_PATH);
                outManager.Setup();
            }
            return outManager;
        }
        catch (FileNotFoundException) {
            StaticSaveStateManager outManager = new();
            outManager.CreateNewSave(CLASSIC_MODE_SAVE_PATH);
            outManager.SaveToFile();
            outManager.Setup();
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

        public AchievementChecker(StaticSaveStateManager outerClass, Func<bool> test, Achievement achievement, params HashSet<AchievementChecker>[] toAddLists) {
            unlockTest = test;
            addedLists = toAddLists;
            correspondingAchievement = achievement;
            outer = outerClass;
            foreach (HashSet<AchievementChecker> i in toAddLists) {
                i.Add(this);
            }
        }

        public bool WillUnlock() {
            return unlockTest();
        }

        public void UnlockAchievement() {
            foreach (HashSet<AchievementChecker> i in addedLists) { // when calling this make sure not in a foreach loop of the lists (create a copy)
                i.Remove(this);
            }
            MonoBehaviour.print("Unlocked Achievement: " + correspondingAchievement);
            outer.unlockedAchievements.Add(correspondingAchievement);
            // TODO: Unlock in Steam Achievements!
        }
    }

    [SerializeField] private int classicModeHighScore;
    [SerializeField] private List<Achievement> unlockedAchievements;


    // Achievement Checker Lists
    private HashSet<AchievementChecker> nextLevelClassicMode = new();


    // variables to help testing for achievements
    public bool inClassicMode = false;
    private bool isInNoSkipModeClassic = false;


    // helper variables
    private int oldClassicLevelNum = 0;
    
    // helper functions
    private Func<bool> NoSkipClassicModeTest(int levelNumber) {
        return () => isInNoSkipModeClassic && inClassicMode && RoomManager.LevelNum == levelNumber;
    }

    // TODO: Map achievements to AchievementCheckers, create lists, connect lists to SaveStateManagerGameObject
    private AchievementChecker MapAchievementsToChecker(Achievement achievement) {
        switch (achievement) {
            case Achievement.NO_SKIP_CLASSIC:
                return new(this, NoSkipClassicModeTest(RoomManager.MAX_LEVEL_NUM), achievement, nextLevelClassicMode);
            case Achievement.NO_SKIP_CLASSIC_PART_ONE:
                return new(this, NoSkipClassicModeTest(RoomManager.PART_ONE_LEVEL_NUM), achievement, nextLevelClassicMode);
            case Achievement.NO_SKIP_CLASSIC_PART_TWO:
                return new(this, NoSkipClassicModeTest(RoomManager.PART_TWO_LEVEL_NUM), achievement, nextLevelClassicMode);
            default:
                throw new InvalidOperationException("Achievement not mapped");
        }
    }

    public void Setup() {
        // find all achievements that have not been unlocked
        HashSet<Achievement> notUnlockedAchievements = new HashSet<Achievement>((Achievement[])Enum.GetValues(typeof(Achievement)));
        notUnlockedAchievements.ExceptWith(unlockedAchievements);

        // create an AchievementChecker for each not unlocked achievement
        foreach (Achievement achievement in notUnlockedAchievements) {
            MapAchievementsToChecker(achievement);
        }
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

    public override void ExitSaveFile()
    {
        inClassicMode = false;
        base.ExitSaveFile();
    }


    private void checkAchievements(HashSet<AchievementChecker> checkerList) {
        if (checkerList.Count == 0) {
            return;
        }
        List<AchievementChecker> toUnlock = new();
        foreach (AchievementChecker checker in checkerList) {
            if (checker.WillUnlock()) {
                toUnlock.Add(checker);
            }
        }
        if (toUnlock.Count > 0) {
            foreach (AchievementChecker checker in toUnlock) {
                checker.UnlockAchievement();
            }
            SaveToFile();
        }
    }

    // methods to test for achievements

    // call when loading level in classic mode
    public void LevelChangeClassicMode() {
        int levelNum = RoomManager.LevelNum;
        if (levelNum == 0) {
            isInNoSkipModeClassic = true;
        } else if (levelNum > oldClassicLevelNum + 1 || levelNum < oldClassicLevelNum) { // check to make sure current level is either the same (in case of unexpected behavior) or 1 above
            isInNoSkipModeClassic = false;
        }
        oldClassicLevelNum = levelNum;
        checkAchievements(nextLevelClassicMode);
    }
}