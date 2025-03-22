using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.Json;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using System.IO;
using System.Net.NetworkInformation;

[Serializable]
public class SaveStateManager {
    public static SaveStateManager LoadInventory(string saveLocation) {
        SaveStateManager outManager;
        try {
            using (StreamReader reader = new(saveLocation)) {
                string jsonData = reader.ReadToEnd();
                outManager = JsonUtility.FromJson<SaveStateManager>(jsonData);
            }
        } catch (IOException) {
            outManager = new SaveStateManager(saveLocation);
        }
        return outManager;
    }



    [Serializable]
    public class LevelSaveData {
        public string LevelName;
        public TankStats Stats;
        public CharacterOption CurrCharacter = CharacterOption.DEFAULT_CAT;
    }

    public enum CharacterOption {
    DEFAULT_CAT,
    ROCKET_CAT,
    BUBBLE_CAT
    }

    private string saveLocation;

    public List<CharacterOption> unlockedCharList = new();

    public LevelSaveData latestLevel;
    private LevelSaveData currentLevel; 
    private CharacterOption currCharacter;

    private HashSet<CharacterOption> unlockedChars;

    public SaveStateManager(string saveLocation) {
        this.saveLocation = saveLocation;
    }


    // character management
    public HashSet<CharacterOption> GetUnlockedCharacters() {
        // recreates a new hashset, so that is a bit of extra computation, but it means things are better encapsulated
        return new HashSet<CharacterOption>(unlockedChars);
    }

    public void SwitchCharacter(Tank t, CharacterOption changeTo) {
        if (unlockedChars.Contains(changeTo)) {
            t.character = CreateNewChar(changeTo);
            currCharacter = changeTo;
        }
    }

    public bool UnlockCharacter(CharacterOption character) {
        return unlockedChars.Add(character);
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
        if (latestLevel is null) {
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
        // TODO: should change so that it can switch to NONE???
        SwitchCharacter(t, currentLevel.CurrCharacter);
        if (currentLevel.Stats != null) {
            currentLevel.Stats.TransferStats(t);
        }
    }

    public void FinishLevel(string nextLevelName, TankStats t) {
        // update unlockedCharList
        unlockedCharList.Clear();
        foreach (CharacterOption x in unlockedChars) {
            unlockedCharList.Add(x);
        }
        if (nextLevelName != null) {
            currentLevel.LevelName = nextLevelName;
            currentLevel.Stats = t;
            currentLevel.CurrCharacter = currCharacter;
        } else {
            latestLevel = new();
            latestLevel.LevelName = "ALL LEVELS UNLOCKED";
        }
        WriteToSaveFile();
    }

    public void OnPlayerDeath() {
        if (currentLevel.Stats != null) {
            currentLevel.Stats = null;
            if (currentLevel == latestLevel) {
                WriteToSaveFile();
            }
        }
    }

    public void OnExitLevel() {
        currentLevel = null;
    }

    private void WriteToSaveFile() {
        // write JSON to file
        File.WriteAllText(saveLocation, JsonUtility.ToJson(this, true));
    }
}