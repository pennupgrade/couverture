using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class StaticSaveStateManager : SaveStateManager {
    public static readonly string STATIC_SAVE_PATH = SaveStateManagerGameObject.CLASSIC_MODE_SAVE_FILE;
    public static StaticSaveStateManager LoadStaticSave() {
        try {
            return LoadSave<StaticSaveStateManager>(STATIC_SAVE_PATH);
        }
        catch (FileNotFoundException) { // NOTE: Could break if there is a saving error!
            return CreateSave<StaticSaveStateManager>(STATIC_SAVE_PATH);
        }
    }

    private const int NUM_ENEMIES_KILLED_ACHIEVEMENT_ONE = 100;


    public enum Achievement {
        WIN_CLASSIC_FULL_NO_SKIP, WIN_CLASSIC_PART_TWO, WIN_CLASSIC_PART_ONE, WIN_CLASSIC_FULL, WIN_CAMPAIGN_MODE,NUM_ENEMIES_KILLED_ONE, CAMPAIGN_MODE_WITHOUT_DYING, CAMPAIGN_MODE_WITHOUT_BUBBLE, CAMPAIGN_MODE_WITHOUT_ROCKET, CAMPAIGN_MODE_WITHOUT_TAKING_DAMAGE, CAMPAIGN_MODE_WITHOUT_KILLING_ENEMY, CAMPAIGN_MODE_WITHOUT_DAMAGING_ENEMY, CAMPAIGN_MODE_WITHOUT_SWITCHING_CHARACTER
    }

    // TODO: UPDATE DEATHS, DAMAGE, ETC WHEN IT HAPPENS

    private class AchievementChecker {
        private readonly Func<bool> unlockTest;
        private readonly HashSet<AchievementChecker>[] addedLists;
        private readonly Achievement correspondingAchievement;
        private readonly StaticSaveStateManager outer;

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

    [Serializable]
    public class LevelAchievementData {
        public bool enemiesKilled = false;
        public bool damageDealtToEnemies = false;
        public bool damageTaken = false;
        public bool hasDied = false;
        public List<CharacterOption> charactersUsed = new();
    }

    [Serializable]
    public class CampaignModeAchievementData {
        public bool runIsValid = true;
        public List<LevelAchievementData> levelAchievementInfo = new();
    }

    protected override string InitialLevelName {get;} = "NULL";

    [SerializeField] private int classicModeHighScore = 0;
    [SerializeField] private List<Achievement> unlockedAchievements = new();

    [SerializeField] private int numEnemiesKilled = 0;


    // Achievement Checker Lists
    private HashSet<AchievementChecker> finishLevelClassicModeList = new();
    private HashSet<AchievementChecker> finishLevelCampaignModeList = new();
    private HashSet<AchievementChecker> enemyKilledList = new();
    private HashSet<AchievementChecker> finishCampaignModeList = new();



    // variables to help testing for achievements
    [NonSerialized] public bool inClassicMode = false;
    private bool isInNoSkipModeClassic = false;


    // helper variables
    private int oldClassicLevelNum = 0;
    private string lastLevelFinished;


    // campaign mode variable
    private CampaignModeAchievementData campaignLevelData;
    
    // helper functions
    private Func<bool> ClassicModeTest(int levelNumber) { // creates a function that tests whether in classic mode and just finished a particular level
        // probably don't needto check inClassicMode because only called when in classic mode, but just for safety
        return () => inClassicMode && RoomManager.LevelJustFinished == levelNumber;
    }

    private Func<bool> CampaignModeTestsLogicalAndOverAllLevels(Func<LevelAchievementData, bool> func) {
        return () => {
            if (!campaignLevelData.runIsValid) {
                return false;
            }
            foreach (LevelAchievementData i in campaignLevelData.levelAchievementInfo) {
                if (!func(i)) {
                    return false;
                }
            }
            return true;
        };
    }

    private Func<bool> CampaignModeWithoutCharacterTest(CharacterOption character) {
        return CampaignModeTestsLogicalAndOverAllLevels(levelData => !levelData.charactersUsed.Contains(character));
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
                Func<bool> testWinCampaignMode = () => !inClassicMode && lastLevelFinished != null && SaveStateManagerGameObject.GetLevelNumberFromSceneName(lastLevelFinished) == CampaignSaveStateManager.MAX_LEVEL_NUMBER;
                return new(this, testWinCampaignMode, achievement, finishCampaignModeList);
            case Achievement.NUM_ENEMIES_KILLED_ONE:
                Func<bool> testNumEnemiesKilledOne = () => numEnemiesKilled >= NUM_ENEMIES_KILLED_ACHIEVEMENT_ONE;
                return new(this, testNumEnemiesKilledOne, achievement, enemyKilledList);
            case Achievement.CAMPAIGN_MODE_WITHOUT_DYING:
                Func<LevelAchievementData, bool> testCampaignModeNoDeathsHelper = levelData => !levelData.hasDied;
                return new(this, CampaignModeTestsLogicalAndOverAllLevels(testCampaignModeNoDeathsHelper), achievement, finishCampaignModeList);
            case Achievement.CAMPAIGN_MODE_WITHOUT_BUBBLE:
                return new(this, CampaignModeWithoutCharacterTest(CharacterOption.BUBBLE_CAT), achievement, finishCampaignModeList);
            case Achievement.CAMPAIGN_MODE_WITHOUT_ROCKET:
                return new(this, CampaignModeWithoutCharacterTest(CharacterOption.ROCKET_CAT), achievement, finishCampaignModeList);
            case Achievement.CAMPAIGN_MODE_WITHOUT_TAKING_DAMAGE:
                Func<LevelAchievementData, bool> testCampaignModeNoTakingDamageHelper = levelData => !levelData.damageTaken;
                return new(this, CampaignModeTestsLogicalAndOverAllLevels(testCampaignModeNoTakingDamageHelper), achievement, finishCampaignModeList);
            case Achievement.CAMPAIGN_MODE_WITHOUT_KILLING_ENEMY:
                Func<LevelAchievementData, bool> testCampaignModeKillingEnemyHelper = levelData => !levelData.enemiesKilled;
                return new(this, CampaignModeTestsLogicalAndOverAllLevels(testCampaignModeKillingEnemyHelper), achievement, finishCampaignModeList);
            case Achievement.CAMPAIGN_MODE_WITHOUT_DAMAGING_ENEMY:
                Func<LevelAchievementData, bool> testCampaignModeDamagingEnemyHelper = levelData => !levelData.damageDealtToEnemies;
                return new(this, CampaignModeTestsLogicalAndOverAllLevels(testCampaignModeDamagingEnemyHelper), achievement, finishCampaignModeList);
            case Achievement.CAMPAIGN_MODE_WITHOUT_SWITCHING_CHARACTER:
                Func<LevelAchievementData, bool> testCampaignModeNoSwitchingHelper = levelData => levelData.charactersUsed.Count == 0;
                return new(this, CampaignModeTestsLogicalAndOverAllLevels(testCampaignModeNoSwitchingHelper), achievement, finishCampaignModeList);
            default:
                throw new InvalidOperationException("Achievement not mapped");
        }
    }

    public override void BeginSession() {
        UnlockAllCharacters();
        // find all achievements that have not been unlocked
        HashSet<Achievement> notUnlockedAchievements = new((Achievement[])Enum.GetValues(typeof(Achievement)));
        notUnlockedAchievements.ExceptWith(unlockedAchievements);

        // create an AchievementChecker for each not unlocked achievement
        foreach (Achievement achievement in notUnlockedAchievements) {
            MapAchievementsToChecker(achievement);
        }

        base.BeginSession();
    }

    private void UnlockAllCharacters() { // force unlock all characters for classic mode
        var allChars = (CharacterOption[])Enum.GetValues(typeof(CharacterOption));
        if (!GetUnlockedCharacters().SetEquals(allChars)) {
            // if unlocked characters arent all characters, won't handle updates that remove characters well
            ForceUnlockCharacters(allChars);
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
        if (SaveStateManagerGameObject.GetLevelNumberFromSceneName(levelFinished) == CampaignSaveStateManager.MAX_LEVEL_NUMBER) {
            CheckAchievements(finishCampaignModeList);
            // set the campaign level data to null if finishing a campaign mode run
            campaignLevelData = null;
        }
    }

    // call when changing level
    public void FinishLevelAchievementCheck(string levelFinished) {
        if (inClassicMode) {
            FinishLevelClassicModeAchievementCheck(RoomManager.LevelJustFinished);
        } else {
            FinishLevelCampaignModeAchievementCheck(levelFinished);
        }
    }

    public void LoadLevelAchievementCheck(string loadingLevel) {
        if (!inClassicMode) {
            LoadLevelCampaignModeAchievementCheck(loadingLevel);
        }
    }

    private void LoadLevelCampaignModeAchievementCheck(string loadingLevel) {
        SetupCampaignRunAchievementsData(loadingLevel);
    }

    private void SetupCampaignRunAchievementsData(string loadingLevel) {
        CampaignSaveStateManager campaignSave = SaveStateManagerGameObject.Instance.StateManager as CampaignSaveStateManager ?? throw new InvalidOperationException("SaveStateManagerGameObject State Manager not an instance of CampaignSaveStateManager!");
        CampaignModeAchievementData campaignSaveAchievementData = campaignSave.levelAchievementData;
        int levelNum = SaveStateManagerGameObject.GetLevelNumberFromSceneName(loadingLevel);

        if (levelNum == 1 && SaveStateManagerGameObject.GetCurrentCheckpoint() is null) {
            campaignLevelData = new();
        }

        if (loadingLevel == campaignSave.GetLatestLevelName() && campaignLevelData != null && campaignLevelData.runIsValid) { // if the user's current campaign run reaches the latest level and thus should be saved
            campaignSave.levelAchievementData = campaignLevelData;
        } else if (campaignSaveAchievementData.runIsValid) {
            if (loadingLevel != campaignSave.GetLatestLevelName()) { // set firstRunIsValid to false if not valid
                campaignSaveAchievementData.runIsValid = false;
                campaignSaveAchievementData.levelAchievementInfo = new();
                campaignLevelData = null;
            } else {
                campaignLevelData = campaignSaveAchievementData;
            }
        } 
        if (campaignLevelData is null) { // if the current campaign mode run is not the first one on the current save file and there is no other currently loaded campaign mode level data
            campaignLevelData = new();
            if (levelNum > 1) {
                campaignLevelData.runIsValid = false;
            }
        }
        if (campaignLevelData.levelAchievementInfo.Count < levelNum) { // add enough additional elements so that current level has a data slot
            LevelAchievementData[] tempArray = new LevelAchievementData[levelNum - campaignLevelData.levelAchievementInfo.Count];
            for (int i = 0; i < tempArray.Length; i++) {
                tempArray[i] = new();
            }
            campaignLevelData.levelAchievementInfo.AddRange(tempArray); // there is probably a better way to do this
        }
    }

    public void ExitLevelAchievementCheck() {
        campaignLevelData = null;
    }

    public void EnemyKilledAchievementCheck() { // maybe include type of enemy as parameter?
        numEnemiesKilled++;
        UpdateLevelData(levelData => levelData.enemiesKilled = true);
        CheckAchievements(enemyKilledList);
    }

    public void EnemyDamagedByPlayerAchievementCheck(int dmg) {
        UpdateLevelData(levelData => levelData.damageDealtToEnemies = true);
    }

    public void PlayerDamagedAchievementCheck(int dmg) {
        UpdateLevelData(levelData => levelData.damageTaken = true);
    }

    public void PlayerDiedAchievementCheck() {
        UpdateLevelData(levelData => levelData.hasDied = true);
    }

    public void SwitchCharacterAchievementCheck(CharacterOption character) {
        UpdateLevelData(levelData => {
            if (!levelData.charactersUsed.Contains(character)) {
                levelData.charactersUsed.Add(character);
            }});
    }

    private void UpdateLevelData(Action<LevelAchievementData> func) {
        if (campaignLevelData is null) {
            return;
        }
        func(campaignLevelData.levelAchievementInfo[^1]);
    }
}