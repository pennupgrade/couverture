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
        StreamReader reader = new StreamReader(SAVE_LOCATION);
        string jsonData = reader.ReadToEnd();
        reader.Close();
        SaveStateManager outManager = JsonUtility.FromJson<SaveStateManager>(jsonData);
        outManager.setup();
        return outManager;
    }

    // JSON representation for unlocked characters is an array of unlocked character id's
    private static HashSet<string> allChars = new HashSet<string>(new string[]{"default_cat", "catfish"});

    public List<string> unlockedCharList = new List<string>();


    private HashSet<string> unlockedChars;

    // must be run after deserialization to correctly setup stuff
    public void setup() {
        // clean up unused characters
        unlockedChars = new HashSet<string>();
        foreach (string character in unlockedCharList) {
            if (allChars.Contains(character)) {
                unlockedChars.Add(character);
            }
        }
    }

    // unlock character
    public void unlockCharacter(string character) {
        if (!allChars.Contains(character)) {
            throw new ArgumentException();
        }
        unlockedChars.Add(character);
    }

    // Switch character
    public void switchCharacter(Tank t, string changeTo) {
        if (unlockedChars.Contains(changeTo)) {
            t.character = createNewChar(changeTo);
        }
    }

    private Character createNewChar(string characterId) {
        switch (characterId) {
            case "default_cat":
                return new RocketChar();
            default:
                throw new ArgumentException();
        };
    }


    // Save the inventory to a file so the state of a player can be loaded
    public void saveGameState() {
        // update unlockedCharList
        List<string> newCharList = new List<string>(unlockedChars.Count);
        foreach (string x in unlockedChars) {
            newCharList.Add(x);
        }
        unlockedCharList = newCharList;


        // write JSON to file
        File.WriteAllText(SAVE_LOCATION, JsonUtility.ToJson(this, true));
    }
}
