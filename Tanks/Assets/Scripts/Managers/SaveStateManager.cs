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
    public static SaveStateManager loadInventory() {
        SaveStateManager outManager;
        try {
            using (StreamReader reader = new StreamReader(SAVE_LOCATION)) {
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
        DEFAULT_CAT
    }

    // JSON representation for unlocked characters is an array of unlocked character id's
    public List<CharacterOption> unlockedCharList = new List<CharacterOption>();


    private HashSet<CharacterOption> unlockedChars;

    // must be run after deserialization to correctly setup stuff
    public void setup() {
        // clean up unused characters
        unlockedChars = new HashSet<CharacterOption>();
        foreach (CharacterOption character in unlockedCharList) {
            unlockedChars.Add(character);
        }
    }

    // unlock character
    public void unlockCharacter(CharacterOption character) {
        unlockedChars.Add(character);
    }

    // Switch character
    public void switchCharacter(Tank t, CharacterOption changeTo) {
        if (unlockedChars.Contains(changeTo)) {
            t.character = createNewChar(changeTo);
        }
    }

    private Character createNewChar(CharacterOption characterId) {
        switch (characterId) {
            case CharacterOption.DEFAULT_CAT:
                return new RocketChar();
            default:
                throw new ArgumentException();
        };
    }


    // Save the inventory to a file so the state of a player can be loaded
    public void saveGameState() {
        // update unlockedCharList
        unlockedCharList.Clear();
        foreach (CharacterOption x in unlockedChars) {
            unlockedCharList.Add(x);
        }

        // write JSON to file
        File.WriteAllText(SAVE_LOCATION, JsonUtility.ToJson(this, true));
    }
}
