// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;

public class SaveStateMenuInfo
{
    private const int NUMBER_OF_SAVES = 3;
    private readonly CampaignSaveStateManager[] saves = new CampaignSaveStateManager[NUMBER_OF_SAVES];

    public SaveStateMenuInfo() {
        for (var i = 0; i < NUMBER_OF_SAVES; i++) {
            saves[i] = CampaignSaveStateManager.TryLoadSaveState(SaveStateManagerGameObject.GetSaveLocation(i));
        }
    }

    public DateTime GetStartTime(int saveNumber) => saves[saveNumber].GetStartTime();

    public DateTime GetLastPlayedTime(int saveNumber) => saves[saveNumber].GetLastPlayedTime();

    public TimeSpan GetTimePlayed(int saveNumber) => saves[saveNumber].GetTimePlayed();

    public string GetLatestLevelName(int saveNumber) => saves[saveNumber].GetLatestLevelName();

    public string GetLatestLevelNameFormatted(int saveNumber) {
        var sceneName = saves[saveNumber].GetLatestLevelName();
        var levelNumber = SaveStateManagerGameObject.GetLevelNumberFromSceneName(sceneName);

        return $"Level {levelNumber}";
    }

    public bool DoesSaveExist(int saveNumber) => saves[saveNumber] != null;

    public HashSet<SaveStateManager.CharacterOption> GetUnlockedCharacters(int saveNumber) =>
        saves[saveNumber].GetUnlockedCharacters();
}