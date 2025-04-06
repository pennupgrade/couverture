using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class SaveStateMenuInfo
{
    private SaveStateManager[] saves = new SaveStateManager[3];

    private static SaveStateManager TryLoadSaveState(int saveNumber) {
        try {
            return SaveStateManagerGameObject.LoadSaveToManager(saveNumber);
        } catch (FileNotFoundException) {
            return null;
        }
    }

    public SaveStateMenuInfo() {
        for (int i = 0; i < 3; i++) {
            saves[i] = TryLoadSaveState(i);
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
