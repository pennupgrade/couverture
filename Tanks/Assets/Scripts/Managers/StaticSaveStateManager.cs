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
        catch (FileNotFoundException) { // NOTE: Could break if there is a saving error!
            StaticSaveStateManager outManager = new();
            outManager.CreateNewSave(CLASSIC_MODE_SAVE_PATH);
            outManager.SaveToFile();
            outManager.Setup();
            return outManager;
        }
    }


    public enum Achievement {
        WIN_CLASSIC_FULL_NO_SKIP, WIN_CLASSIC_PART_TWO, WIN_CLASSIC_PART_ONE, WIN_CLASSIC_FULL, WIN_CAMPAIGN_MODE
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

    [SerializeField] private int classicModeHighScore = 0;
    [SerializeField] private List<Achievement> unlockedAchievements = new();


    // Achievement Checker Lists
    private HashSet<AchievementChecker> finishLevelClassicModeList = new();
    private HashSet<AchievementChecker> finishLevelCampaignModeList = new();



    // variables to help testing for achievements
    [NonSerialized] public bool inClassicMode = false;
    private bool isInNoSkipModeClassic = false;


    // helper variables
    private int oldClassicLevelNum = 0;
    private string lastLevelFinished;
    
    // helper functions
    private Func<bool> ClassicModeTest(int levelNumber) { // creates a function that tests whether in classic mode and just finished a particular level
        // probably don't needto check inClassicMode because only called when in classic mode, but just for safety
        return () => inClassicMode && RoomManager.LevelJustFinished == levelNumber;
    }

    // Creates a checker for each achievement
    private AchievementChecker MapAchievementsToChecker(Achievement achievement) {
        switch (achievement) {
            case Achievement.WIN_CLASSIC_FULL_NO_SKIP:
                Func<bool> testClassicModeNoSkip = () => isInNoSkipModeClassic && ClassicModeTest(RoomManager.MAX_LEVEL_NUM)();
                return new(this, testClassicModeNoSkip, achievement, finishLevelClassicModeList);
            case Achievement.WIN_CLASSIC_PART_ONE:
                return new(this, ClassicModeTest(RoomManager.PART_ONE_LEVEL_NUM), achievement, finishLevelClassicModeList);
            case Achievement.WIN_CLASSIC_PART_TWO:
                return new(this, ClassicModeTest(RoomManager.PART_TWO_LEVEL_NUM), achievement, finishLevelClassicModeList);
            case Achievement.WIN_CLASSIC_FULL:
                return new(this, ClassicModeTest(RoomManager.MAX_LEVEL_NUM), achievement, finishLevelClassicModeList);
            case Achievement.WIN_CAMPAIGN_MODE:
                Func<bool> testWinCampaignMode = () => !inClassicMode && SaveStateManagerGameObject.GetLevelNumberFromSceneName(lastLevelFinished) == GameManager.MAX_LEVEL_NUMBER;
                return new(this, testWinCampaignMode, achievement, finishLevelCampaignModeList);
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


    private void CheckAchievements(HashSet<AchievementChecker> checkerList) {
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

    // call when changing level in classic mode
    private void FinishLevelClassicModeAchievementCheck(int levelNum) {
        if (RoomManager.LevelJustFinished == 1) {
            isInNoSkipModeClassic = true;
        } else if (levelNum > oldClassicLevelNum + 1 || levelNum < oldClassicLevelNum) { // check to make sure current level is either the same (in case of unexpected behavior) or 1 above
            isInNoSkipModeClassic = false;
        }
        oldClassicLevelNum = levelNum;

        // check achievements
        CheckAchievements(finishLevelClassicModeList);
    }

    private void FinishLevelCampaignModeAchievementCheck(string levelFinished) {
        lastLevelFinished = levelFinished;
        CheckAchievements(finishLevelCampaignModeList);
    }

    // call when changing level
    public void FinishLevelAchievementCheck(string levelFinished) {
        if (inClassicMode) {
            FinishLevelClassicModeAchievementCheck(RoomManager.LevelJustFinished);
        } else {
            FinishLevelCampaignModeAchievementCheck(levelFinished);
        }
    }

    private int GetJustFinishedClassicLevel() { // this is always called after RoomManager increments the level number, hence why it subtracts 1 from the current level number.  Should only be called after a level is finished
        return RoomManager.LevelNum - 1;
    }

    public void LoadLevelAchievementCheck(string loadingLevel) { }
}