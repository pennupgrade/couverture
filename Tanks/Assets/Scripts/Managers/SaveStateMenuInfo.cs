using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveStateMenuInfo
{
    private const int NUMBER_OF_SAVES = 3;
    private readonly SaveStateManager[] saves = new SaveStateManager[NUMBER_OF_SAVES];

    public SaveStateMenuInfo() {
        for (var i = 0; i < NUMBER_OF_SAVES; i++) {
            saves[i] = SaveStateManager.TryLoadSaveState(SaveStateManagerGameObject.GetSaveLocation(i));
        }
    }

    public DateTime GetStartTime(int saveNumber) => saves[saveNumber].GetStartTime();

    public DateTime GetLastPlayedTime(int saveNumber) => saves[saveNumber].GetLastPlayedTime();

    public TimeSpan GetTimePlayed(int saveNumber) => saves[saveNumber].GetTimePlayed();

    public string GetLatestLevelName(int saveNumber) => saves[saveNumber].GetLatestLevelName();

    public string GetLatestLevelNameFormatted(int saveNumber) {
        var sceneName = saves[saveNumber].GetLatestLevelName();
        var levelNumber = -1;

        if (sceneName.Contains('1')) {
            levelNumber = 1;
        }
        else if (sceneName.Contains('2')) {
            levelNumber = 2;
        }
        else if (sceneName.Contains('3')) {
            levelNumber = 3;
        }
        else if (sceneName.Contains('4')) {
            levelNumber = 4;
        }
        else if (sceneName.Contains('5')) {
            levelNumber = 5;
        }
        else if (sceneName.Contains('6')) {
            levelNumber = 6;
        }
        else {
            Debug.LogWarning("GetLatestLevelNameFormatted(): could not find level number!");
        }

        return $"Level {levelNumber}";
    }

    public bool DoesSaveExist(int saveNumber) => saves[saveNumber] != null;

    public HashSet<SaveStateManager.CharacterOption> GetUnlockedCharacters(int saveNumber) =>
        saves[saveNumber].GetUnlockedCharacters();
}