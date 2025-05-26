using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveStateManagerGameObject : MonoBehaviour
{
    private static readonly string SAVE_FILE_PREFIX = "save_data_";
    public static readonly string CLASSIC_MODE_SAVE_FILE = SaveStateManager.GetFullSavePath(SAVE_FILE_PREFIX + "static.json");
    public static SaveStateManagerGameObject Instance;

    private SaveStateManager stateManager;

    private StaticSaveStateManager staticStateManager;

    private void Awake() {
        if (Instance is null) {
            Instance = this;
            staticStateManager = StaticSaveStateManager.LoadStaticSave();
            Application.quitting += ExitCurrentSave;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public static string GetSaveLocation(int saveNumber) => SaveStateManager.GetFullSavePath(SAVE_FILE_PREFIX + saveNumber + ".json");

    // returns true if save was loaded, false if save was created
    private static bool LoadCampaignSave(string saveLocation) {
        if (Instance.stateManager != null) {
            throw new InvalidOperationException("A Save State is Already Open!");
        }

        var wasLoaded = true;
        Instance.stateManager = CampaignSaveStateManager.TryLoadSaveState(saveLocation);
        if (Instance.stateManager is null) {
            Instance.stateManager = CampaignSaveStateManager.CreateCampaignSave(saveLocation);
            wasLoaded = false;
        }
        return wasLoaded;
    }

    public static void LoadSaveSlot(int saveNumber) {
        LoadCampaignSave(GetSaveLocation(saveNumber));
    }

    public static void UnlockCharacter(SaveStateManager.CharacterOption c) {
        Instance.stateManager.UnlockCharacter(c);
    }

    // load save data for level that is currently in
    public static void LoadLevel(string levelName) {
        Instance.stateManager.LoadLevel(levelName, Tank.FindPlayer());
        Instance.staticStateManager.LoadLevelAchievementCheck(levelName);
    }

    public static void SwitchCharacter(SaveStateManager.CharacterOption c) {
        Instance.stateManager.SwitchCharacter(Tank.FindPlayer(), c);
    }

    public static HashSet<SaveStateManager.CharacterOption> GetUnlockedCharacters() =>
        Instance.stateManager.GetUnlockedCharacters();

    public static void FinishLevel(string nextLevelName, bool toSave) {
        string finishedLevelName = Instance.stateManager.GetCurrentLevelName();
        Instance.stateManager.FinishLevel(nextLevelName, new TankStats(Tank.FindPlayer()), toSave);
        Instance.staticStateManager.FinishLevelAchievementCheck(finishedLevelName);
    }

    public static void ExitLevel() {
        Instance.stateManager.OnExitLevel();
    }

    // debug method so tests can be run from Unity editor from simply starting scene
    public static void DebugLoadSave() {
        if (Instance.stateManager is null) {
            LoadSaveSlot(1);
        }
    }

    public static void CreateSave(int saveSlot) {
        CampaignSaveStateManager.CreateCampaignSave(GetSaveLocation(saveSlot));
    }

    public static void ExitCurrentSave() {
        if (Instance is null || Instance.stateManager is null) {
            return;
        }

        Instance.stateManager.ExitSaveFile();
        if (Instance.stateManager != Instance.staticStateManager) {
            Instance.staticStateManager.ExitSaveFile();
        }
        Instance.stateManager = null;
    }

    public static void PlayerDied() {
        Instance.stateManager.OnPlayerDeath();
    }

    public static void DeleteSaveSlot(int saveNumber) {
        SaveStateManager.DeleteSaveFile(GetSaveLocation(saveNumber));
    }

    public static void UpdateClassicModeHighScore(int newScore) {
        Instance.staticStateManager.UpdateClassicModeHighScore(newScore);
    }

    public static string GetLatestLevelName() => Instance.stateManager.GetLatestLevelName();

    public static void SaveToFile() {
        Instance.stateManager.SaveToFile();
    }

    public static int GetClassicModeHighScore() => Instance.staticStateManager.GetClassicModeHighScore();

    public static void LoadClassicModeSave() {
        if (Instance.stateManager != null) {
            // only load classic mode save once
            return;
        }

        Instance.staticStateManager.inClassicMode = true;
        Instance.stateManager = Instance.staticStateManager;
    }

    public static void UnlockCheckpoint(int i) {
        Instance.stateManager.UnlockCheckpoint(i, new TankStats(Tank.FindPlayer()));
    }

    public static int? GetCurrentCheckpoint() => Instance.stateManager.GetCurrentCheckpoint();

    public static SaveStateManager.CharacterOption GetCurrentCharacter() => Instance.stateManager.CurrCharacter;

    public static HashSet<StaticSaveStateManager.Achievement> GetAchievements() {
        return Instance.staticStateManager.GetAchievements();
    }

    public static int GetLevelNumberFromSceneName(string sceneName) {
        if (sceneName.Contains('1')) {
            return 1;
        }

        if (sceneName.Contains('2')) {
            return 2;
        }

        if (sceneName.Contains('3')) {
            return 3;
        }

        if (sceneName.Contains('4')) {
            return 4;
        }

        if (sceneName.Contains('5')) {
            return 5;
        }

        if (sceneName.Contains('6')) {
            return 6;
        }

        if (sceneName == "ALL LEVELS UNLOCKED") {
            return 6;
        }

        return -1;
    }

}