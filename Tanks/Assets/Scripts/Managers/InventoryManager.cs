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
public class InventoryManager
{
    private const string SAVE_LOCATION = "save_data.json";
    public static InventoryManager loadInventory() {
        StreamReader reader = new StreamReader(SAVE_LOCATION);
        string jsonData = reader.ReadToEnd();
        reader.Close();
        InventoryManager outManager = JsonUtility.FromJson<InventoryManager>(jsonData);
        outManager.setup();
        return outManager;
    }

    // JSON representation for unlocked characters is an array of unlocked character id's
    private static HashSet<string> allChars = new HashSet<string>(new string[]{"default_cat", "catfish"});

    public List<string> unlockedCharList = new List<string>();


    private HashSet<string> unlockedChars;

    // must be run after deserialization to correctly setup stuff
    void setup() {
        // clean up unused characters
        unlockedChars = new HashSet<string>();
        foreach (string character in unlockedCharList) {
            if (allChars.Contains(character)) {
                unlockedChars.Add(character);
            }
        }
    }

    // unlock character
    void unlockCharacter(string character) {
        if (!allChars.Contains(character)) {
            throw new ArgumentException();
        }
        unlockedChars.Add(character);
    }

    // Switch character
    void switchCharacter(Tank t, string changeTo) {
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
    void saveInventory() {
        // update unlockedCharList
        List<string> newCharList = new List<string>(unlockedChars.Count);
        foreach (string x in unlockedChars) {
            newCharList.Add(x);
        }
        unlockedCharList = newCharList;

        // TODO: SERIALIZE DATA
    }
}
