using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class SaveStateManager {

    private const string NULL_LEVEL_NAME = "NOT_A_LEVEL";

    public static SaveStateManager TryLoadSaveState(string saveFilePath) {
        try {
            return LoadInventory(saveFilePath);
        } catch (FileNotFoundException) {
            return null;
        }
    }

    public static SaveStateManager LoadInventory(string saveLocation) {
        SaveStateManager outManager;
        using (StreamReader reader = new(saveLocation)) {
            string jsonData = reader.ReadToEnd();
            outManager = JsonUtility.FromJson<SaveStateManager>(jsonData);
            outManager.SetSaveLocation(saveLocation);
        }
        return outManager;
    }

    public static SaveStateManager CreateSave(string saveLocation) {
        SaveStateManager outManager = new();
        outManager.CreateNewSave(saveLocation);
        return outManager;
    }


    [Serializable]
    private class LevelSaveData {
        public string LevelName;
        public TankStats Stats = new();
        public CharacterOption CurrCharacter = CharacterOption.DEFAULT_CAT;
    }

    public enum CharacterOption {
        DEFAULT_CAT,
        ROCKET_CAT,
        BUBBLE_CAT
    }

    // serialized properties
    [SerializeField] private List<CharacterOption> unlockedCharList = new() { CharacterOption.DEFAULT_CAT};

    [SerializeField] private LevelSaveData latestLevel;

    [SerializeField] private DateTimeSerializable startTime;
    
    [SerializeField] private DateTimeSerializable lastPlayedTime;

    [SerializeField] private TimeSpanSerializable timePlayed;
    [SerializeField] private int classicModeHighScore = 0;




    private string saveLocation;
    private LevelSaveData currentLevel; 
    private CharacterOption currCharacter;
    private HashSet<CharacterOption> unlockedChars;
    private DateTime startOfSession;

    public SaveStateManager() { }

    public void SetSaveLocation(string saveLocation) {
        this.saveLocation = saveLocation;
    }

    // sets up the current SaveStateManager as a new save, DOES NOT SET startOfSession OR SAVE TO FILE!
    public void CreateNewSave(string saveLocation) {
        // set save location
        this.saveLocation = saveLocation;
        // initialize values
        latestLevel = new();
        latestLevel.LevelName = NULL_LEVEL_NAME;
        startTime = DateTimeSerializable.Now();
        lastPlayedTime = DateTimeSerializable.Now();
        timePlayed = new(TimeSpan.Zero);
    }

    // call when session is started (save file is selected!)
    // instantiates startOfSession and saves file
    public void BeginSession() {
        startOfSession = DateTime.Now;

        // should it be saved here?
        WriteToSaveFile();
    }

    // character management
    public HashSet<CharacterOption> GetUnlockedCharacters() {
        // recreates a new hashset, so that is a bit of extra computation, but it means things are better encapsulated
        // null check so that it works out of levels
        if (unlockedChars is null) {
            return new HashSet<CharacterOption>(unlockedCharList);
        }
        return new HashSet<CharacterOption>(unlockedChars);
    }

    public void SwitchCharacter(Tank t, CharacterOption changeTo) {
        // shouldn't ever be an issue since DEFAULT CAT should always be in the list, but its good to be safe
        if (unlockedChars.Contains(changeTo) || changeTo == CharacterOption.DEFAULT_CAT) {
            t.character = CreateNewChar(changeTo);
            currCharacter = changeTo;
        } else {
            throw new InvalidOperationException("Player has not unlocked that character!");
        }
    }

    public bool UnlockCharacter(CharacterOption character) {
        return unlockedChars.Add(character);
    }

    // force a character to be unlocked without having to complete a level, save to file
    public void ForceUnlockCharacters(IEnumerable<CharacterOption> c) {
        HashSet<CharacterOption> charSet = new(unlockedCharList);
        charSet.UnionWith(c);
        unlockedCharList = new(charSet);
        SaveToFile();
    }

    private Character CreateNewChar(CharacterOption characterId) {
        switch (characterId) {
            case CharacterOption.ROCKET_CAT:
                return new RocketChar();
            case CharacterOption.BUBBLE_CAT:
                return new BubbleChar();
            case CharacterOption.DEFAULT_CAT:
                return new DefaultChar();
            default:
                throw new ArgumentException();
        };
    }

    public void LoadLevel(string levelName, Tank t) {
        if (latestLevel is null || latestLevel.LevelName == NULL_LEVEL_NAME) {
            latestLevel = new();
            latestLevel.LevelName = levelName;
        }
        // this could be optimized
        unlockedChars = new(unlockedCharList);
        if (levelName == latestLevel.LevelName) {
            // if loading latest level, replace currentlevel with latestlevel
            currentLevel = latestLevel;
        } else if (currentLevel == null || currentLevel.LevelName != levelName) {
            // If currentLevel data is not applicable, wipe it and create new data
            currentLevel = new();
        }
        // load currentlevel
        currCharacter = currentLevel.CurrCharacter;
        // load current character
        SwitchCharacter(t, currentLevel.CurrCharacter);
        // Having the health stat be 0 will be an indicator to not transfer stats (essentially a null value)
        if (currentLevel.Stats.health != 0) {
            currentLevel.Stats.TransferStats(t);
        }
    }

    public void FinishLevel(string nextLevelName, TankStats t, bool toSave) {
        // update unlockedCharList
        unlockedCharList.Clear();
        foreach (CharacterOption x in unlockedChars) {
            unlockedCharList.Add(x);
        }
        if (nextLevelName != null) {
            currentLevel.LevelName = nextLevelName;
            currentLevel.Stats = t;
            currentLevel.CurrCharacter = currCharacter;
            if (nextLevelName == latestLevel.LevelName) {
                latestLevel = currentLevel;
            }
        } else {
            latestLevel = new();
            latestLevel.LevelName = "ALL LEVELS UNLOCKED";
        }
        if (toSave) {
            WriteToSaveFile();
        }
    }

    public void OnPlayerDeath() {
        if (currentLevel is null) {
            throw new InvalidOperationException("Trying to restart level, but not currently in a level!");
        }
        if (currentLevel.Stats.health != 0) {
            currentLevel.Stats.health = 0;
            if (currentLevel == latestLevel) {
                WriteToSaveFile();
            }
        }
    }

    public void OnExitLevel() {
        currentLevel = null;
    }

    // Save play time on game exit
    public void ExitSaveFile() {
        WriteToSaveFile();
    }

    private void WriteToSaveFile() {
        // calculate last played time and total play time
        DateTime now = DateTime.Now;
        // TODO: could have issues if crossing between time zones
        lastPlayedTime = new(now);
        timePlayed = new(GetTimePlayed().Add(now.Subtract(startOfSession)));
        startOfSession = now;

        // write JSON to file
        File.WriteAllText(saveLocation, JsonUtility.ToJson(this, true));
    }
    

    // Getters
    public DateTime GetStartTime() {
        return startTime.ToDateTime();
    }

    public DateTime GetLastPlayedTime() {
        return lastPlayedTime.ToDateTime();
    }

    public TimeSpan GetTimePlayed() {
        return timePlayed.ToTimeSpan();
    }

    public string GetLatestLevelName() {
        if (latestLevel is null) {
            // TODO: WHAT TO DO IN THIS CASE?
            return null;
        }
        return latestLevel.LevelName;
    }

    public static void DeleteSaveFile(string file) {
        File.Delete(file);
    }

    public void UpdateClassicModeHighScore(int newScore) {
        // checks if new score is larger than current max score
        if (newScore > classicModeHighScore) {
            classicModeHighScore = newScore;
        }
    }

    public void SaveToFile() {
        WriteToSaveFile();
    }

    public int GetClassicModeHighScore() {
        return classicModeHighScore;
    }
}