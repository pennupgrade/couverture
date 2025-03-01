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
public class SaveStateManager
{
    private const string SAVE_LOCATION = "save_data.json";
    public static SaveStateManager LoadInventory() {
        SaveStateManager outManager;
        try {
            using (StreamReader reader = new(SAVE_LOCATION)) {
                string jsonData = reader.ReadToEnd();
                outManager = JsonUtility.FromJson<SaveStateManager>(jsonData);
            }
        } catch (IOException) {
            outManager = new SaveStateManager();
        }
        outManager.setup();
        return outManager;
    }

    public enum CharacterOption {
        NONE,
        ROCKET_CAT,
        BUBBLE_CAT
    }

    // JSON representation for unlocked characters is an array of unlocked character id's
    public List<CharacterOption> unlockedCharList = new();

    public string currCheckpointLevelName = null;
    public int currCheckpoint = -1;

    public int numLives;

    private HashSet<CharacterOption> unlockedChars;

    // must be run after deserialization to correctly setup stuff
    public void setup() {
        // clean up unused characters
        unlockedChars = new HashSet<CharacterOption>();
        foreach (CharacterOption character in unlockedCharList) {
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
        unlockedCharList.Clear();
        if (GameManager.Instance is null) {
            throw new InvalidOperationException("GameManager Instance is Null!");
        }
        numLives = GameManager.Instance.GetLives();
        foreach (CharacterOption x in unlockedChars) {
            unlockedCharList.Add(x);
        }

        // write JSON to file
        File.WriteAllText(SAVE_LOCATION, JsonUtility.ToJson(this, true));
    }
}
