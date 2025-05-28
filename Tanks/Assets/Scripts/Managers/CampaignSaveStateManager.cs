using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class CampaignSaveStateManager : SaveStateManager {

    public const int MAX_LEVEL_NUMBER = 6; // maximum level number

    public static CampaignSaveStateManager TryLoadSaveState(string saveFilePath) {
        try {
            return LoadInventory(saveFilePath);
        }
        catch (FileNotFoundException) {
            return null;
        }
    }

    public static CampaignSaveStateManager LoadInventory(string saveLocation) {
        return LoadSave<CampaignSaveStateManager>(saveLocation);
    }

    public static CampaignSaveStateManager CreateCampaignSave(string saveLocation) {
        return CreateSave<CampaignSaveStateManager>(saveLocation);
    }

    
    [Serializable]
    public class LevelActionData {
        public int numEnemiesKilled = 0;
        public bool damageDealtToEnemies = false;
        public bool damageTaken = false;
        public bool hasDied = false;
        public List<CharacterOption> charactersUsed = new();
    }

    [SerializeField] private DateTimeSerializable startTime;

    [SerializeField] private DateTimeSerializable lastPlayedTime;

    [SerializeField] private TimeSpanSerializable timePlayed;

    [SerializeField] private bool firstRunIsValid = true;
    [SerializeField] private List<LevelActionData> levelAchievementData = new();

    private DateTime startOfSession;

    public override void CreateNewSave(string saveLocation)
    {
        base.CreateNewSave(saveLocation);

        // initialize values
        startTime = DateTimeSerializable.Now();
        startOfSession = DateTime.Now;
        lastPlayedTime = DateTimeSerializable.Now();
        timePlayed = new TimeSpanSerializable(TimeSpan.Zero);
    }

    public override void BeginSession()
    {
        startOfSession = DateTime.Now;
        base.BeginSession();
    }

    protected override void WriteToSaveFile()
    {
        // calculate last played time and total play time
        DateTime now = DateTime.Now;

        // TODO: could have issues if crossing between time zones
        lastPlayedTime = new DateTimeSerializable(now);
        timePlayed = new TimeSpanSerializable(GetTimePlayed().Add(now.Subtract(startOfSession)));
        startOfSession = now;

        base.WriteToSaveFile();
    }

    public override void LoadLevel(string levelName, Tank t)
    {
        if (firstRunIsValid) { // only do valid run checks if the run is currently valid
            string latestLevelName = GetLatestLevelName();
            int latestLevelNum = SaveStateManagerGameObject.GetLevelNumberFromSceneName(latestLevelName);
            if (latestLevelName == ALL_LEVELS_UNLOCKED) {
                latestLevelNum = MAX_LEVEL_NUMBER + 1;
            }
            int loadingLevelNum = SaveStateManagerGameObject.GetLevelNumberFromSceneName(levelName);
            if (loadingLevelNum < latestLevelNum) { // if goes to previous level, then invalidate run
                firstRunIsValid = false;
                levelAchievementData = new();
                WriteToSaveFile();
            }
        }
        base.LoadLevel(levelName, t);
    }

    // Getters
    public DateTime GetStartTime() => startTime.ToDateTime();

    public DateTime GetLastPlayedTime() => lastPlayedTime.ToDateTime();

    public TimeSpan GetTimePlayed() => timePlayed.ToTimeSpan();
}