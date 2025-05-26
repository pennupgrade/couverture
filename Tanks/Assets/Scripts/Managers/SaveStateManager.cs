using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SaveStateManager
{
    private const string NULL_LEVEL_NAME = "NOT_A_LEVEL";

    public static string GetFullSavePath(string save_name) => Path.GetFullPath(save_name, CompilationConstants.SAVE_DATA_PATH);

    protected static void CreateSaveDirectory() {
        Directory.CreateDirectory(CompilationConstants.SAVE_DATA_PATH);
    }

    public static T LoadSave<T>(string saveLocation) where T : SaveStateManager {
        CreateSaveDirectory();
        T outManager;
        using (StreamReader reader = new(saveLocation)) {
            string jsonData = reader.ReadToEnd();
            outManager = JsonUtility.FromJson<T>(jsonData);
            outManager.SetSaveLocation(saveLocation);
            // start the session
            outManager.BeginSession();
        }
        return outManager;
    }

    public static T CreateSave<T>(string saveLocation) where T : SaveStateManager, new() {
        CreateSaveDirectory();
        T outManager = new();
        outManager.CreateNewSave(saveLocation);
        outManager.SaveToFile();
        outManager.BeginSession();
        return outManager;
    }

    [Serializable]
    private class LevelSaveData
    {
        public string LevelName = null;
        public TankStats Stats = new();
        public CharacterOption CurrCharacter = CharacterOption.DEFAULT_CAT;

        // checkpoint save data
        public int CheckpointIndex = -1;
        public List<CharacterOption> AdditionalUnlockedChars = new();

        public void Save(TankStats t, CharacterOption c, int checkpointIndex, List<CharacterOption> unlockedChars) {
            Stats = t;
            CurrCharacter = c;
            CheckpointIndex = checkpointIndex;
            AdditionalUnlockedChars = unlockedChars;
        }
    }

    public enum CharacterOption
    {
        DEFAULT_CAT,
        ROCKET_CAT,
        BUBBLE_CAT
    }

    // serialized properties
    [SerializeField] private List<CharacterOption> unlockedCharList = new() { CharacterOption.DEFAULT_CAT };

    [SerializeField] private LevelSaveData latestLevel;

    

    private string saveLocation;
    private LevelSaveData currentLevel;
    public CharacterOption CurrCharacter {get; private set;}
    private HashSet<CharacterOption> unlockedChars;

    public void SetSaveLocation(string saveLocation) {
        this.saveLocation = saveLocation;
    }

    // sets up the current SaveStateManager as a new save, DOES NOT SET startOfSession OR SAVE TO FILE!
    public virtual void CreateNewSave(string saveLocation) {
        // set save location
        SetSaveLocation(saveLocation);
        // initialize values
        latestLevel = new LevelSaveData();
        latestLevel.LevelName = NULL_LEVEL_NAME;
    }

    // call when session is started (save file is selected!)
    // make sure it is not needed to be run before newly created save manager is created 
    public virtual void BeginSession() { }

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
            CurrCharacter = changeTo;
        }
        else {
            throw new InvalidOperationException("Player has not unlocked that character!");
        }
    }

    public bool UnlockCharacter(CharacterOption character) => unlockedChars.Add(character);

    // force a character to be unlocked without having to complete a level, save to file
    public void ForceUnlockCharacters(IEnumerable<CharacterOption> c) {
        HashSet<CharacterOption> charSet = new(unlockedCharList);
        charSet.UnionWith(c);
        unlockedCharList = new List<CharacterOption>(charSet);
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
            latestLevel = new LevelSaveData();
            latestLevel.LevelName = levelName;
        }

        // this could be optimized
        unlockedChars = new HashSet<CharacterOption>(unlockedCharList);
        if (levelName == latestLevel.LevelName) {
            // if loading latest level, replace currentlevel with latestlevel
            currentLevel = latestLevel;
            unlockedChars.AddRange(currentLevel.AdditionalUnlockedChars);
        }
        else if (currentLevel == null || currentLevel.LevelName != levelName) {
            // If currentLevel data is not applicable, wipe it and create new data
            currentLevel = new LevelSaveData();
            currentLevel.LevelName = levelName;
        }
        
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
            currentLevel.Save(t, CurrCharacter, -1, new List<CharacterOption>());
            currentLevel.LevelName = nextLevelName;
            if (nextLevelName == latestLevel.LevelName) {
                latestLevel = currentLevel;
            }
        }
        else {
            latestLevel = new LevelSaveData();
            latestLevel.Save(t, CurrCharacter, -1, new List<CharacterOption>());
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
    public virtual void ExitSaveFile() {
        WriteToSaveFile();
    }

    protected virtual void WriteToSaveFile() {
        // write JSON to file
        File.WriteAllText(saveLocation, JsonUtility.ToJson(this, true));
    }

    // Getters
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

    

    public void SaveToFile() {
        WriteToSaveFile();
    }



    public bool UnlockCheckpoint(int i, TankStats t) {
        bool successfulCheckpoint = currentLevel.CheckpointIndex < i;
        if (successfulCheckpoint) {
            currentLevel.Save(t, CurrCharacter, i, new List<CharacterOption>(unlockedChars));
            if (currentLevel.LevelName == latestLevel.LevelName) {
                // prevent unnecessary writes
                WriteToSaveFile();
            }
        }

        return successfulCheckpoint;
    }

    public int? GetCurrentCheckpoint() {
        if (currentLevel.CheckpointIndex == -1) {
            return null;
        }

        return currentLevel.CheckpointIndex;
    }

    public string GetCurrentLevelName() {
        if (currentLevel is null) {
            return null;
        }
        return currentLevel.LevelName;
    }
}