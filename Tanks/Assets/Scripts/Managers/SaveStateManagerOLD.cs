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
public class SaveStateManagerOLD
{
    private const string SAVE_LOCATION = "save_data.json";
    public static SaveStateManagerOLD LoadInventory() {
        SaveStateManagerOLD outManager;
        try {
            using (StreamReader reader = new(SAVE_LOCATION)) {
                string jsonData = reader.ReadToEnd();
                outManager = JsonUtility.FromJson<SaveStateManagerOLD>(jsonData);
            }
        } catch (IOException) {
            outManager = new SaveStateManagerOLD();
        }
        outManager.Setup();
        return outManager;
    }

    public enum CharacterOption {
        NONE,
        ROCKET_CAT,
        BUBBLE_CAT
    }

    // JSON representation for unlocked characters is an array of unlocked character id's
    public List<CharacterOption> currentUnlockedCharList = new();

    public string currCheckpointLevelName = null;
    public int currCheckpoint = -1;
    public CharacterOption currCharacter = CharacterOption.NONE;

    public int numLives = -1;

    public List<CharacterOption> oldUnlockedCharList = new();


    private HashSet<CharacterOption> unlockedChars;

    // must be run after deserialization to correctly setup stuff
    public void Setup() {
        // clean up unused characters
        unlockedChars = new HashSet<CharacterOption>();
        foreach (CharacterOption character in currentUnlockedCharList) {
            unlockedChars.Add(character);
        }
    }


    //          CHARACTER STUFF

    // unlock character, returns true if character wasn't unlocked before (successfully unlocked) and false otherwise
    public bool UnlockCharacter(CharacterOption character) {
        return unlockedChars.Add(character);
    }

    // Switch character
    public void SwitchCharacter(Tank t, CharacterOption changeTo) {
        if (unlockedChars.Contains(changeTo)) {
            t.character = CreateNewChar(changeTo);
            currCharacter = changeTo;
        }
    }

    private Character CreateNewChar(CharacterOption characterId) {
        switch (characterId) {
            case CharacterOption.ROCKET_CAT:
                return new RocketChar();
            case CharacterOption.BUBBLE_CAT:
                return new BubbleChar();
            default:
                throw new ArgumentException();
        };
    }


    //          SAVE GAME STATE

    // Save the inventory to a file so the state of a player can be loaded
    public void SaveGameState() {
        // update unlockedCharList
        currentUnlockedCharList.Clear();
        foreach (CharacterOption x in unlockedChars) {
            currentUnlockedCharList.Add(x);
        }

        // write JSON to file
        File.WriteAllText(SAVE_LOCATION, JsonUtility.ToJson(this, true));
    }

    public HashSet<CharacterOption> GetUnlockedCharacters() {
        // recreates a new hashset, so that is a bit of extra computation, but it means things are better encapsulated
        return new HashSet<CharacterOption>(unlockedChars);
    }

    public void FullUnlockCurrCharacters() {
        oldUnlockedCharList = new(currentUnlockedCharList);
    }

    public void ResetToOld() {
        currCheckpointLevelName = null;
        currCheckpoint = -1;
        numLives = -1;
        // should I save the current character?
        currCharacter = CharacterOption.NONE;
        unlockedChars = new(oldUnlockedCharList);
    }
}
