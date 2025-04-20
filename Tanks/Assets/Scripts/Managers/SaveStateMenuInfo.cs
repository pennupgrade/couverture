using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class SaveStateMenuInfo
{
    const int NUMBER_OF_SAVES = 3;

    private SaveStateManager[] saves = new SaveStateManager[NUMBER_OF_SAVES];

    public SaveStateMenuInfo() {
        for (int i = 0; i < NUMBER_OF_SAVES; i++) {
            saves[i] = SaveStateManager.TryLoadSaveState(SaveStateManagerGameObject.GetSaveLocation(i));
        }
    }

    public DateTime GetStartTime(int saveNumber) {
        //return new DateTime(1, 1, 1);
        return saves[saveNumber].GetStartTime();
    }

    public DateTime GetLastPlayedTime(int saveNumber) {
        //return new DateTime(1, 1, 1);
        return saves[saveNumber].GetLastPlayedTime();
    }

    public TimeSpan GetTimePlayed(int saveNumber) {
        //return TimeSpan.FromDays(1);
        return saves[saveNumber].GetTimePlayed();
    }

    public string GetLatestLevelName(int saveNumber) {
        //return "hi";
        return saves[saveNumber].GetLatestLevelName();
    }

    public bool DoesSaveExist(int saveNumber) {
        //return true;
        return saves[saveNumber] != null;
    }

    public HashSet<SaveStateManager.CharacterOption> GetUnlockedCharacters(int saveNumber) {
        return saves[saveNumber].GetUnlockedCharacters();
    }
}
