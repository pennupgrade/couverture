using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveStateManagerGameObject : MonoBehaviour
{
    private static readonly string SAVE_FILE_PREFIX = "save_data_";
    public static readonly string CLASSIC_MODE_SAVE_FILE = SaveStateManager.GetFullSavePath(SAVE_FILE_PREFIX + "static.json");
    public static SaveStateManagerGameObject Instance;

    public SaveStateManager StateManager {get; private set;}

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
        if (Instance.StateManager != null) {
            throw new InvalidOperationException("A Save State is Already Open!");
        }

        var wasLoaded = true;
        Instance.StateManager = CampaignSaveStateManager.TryLoadSaveState(saveLocation);
        if (Instance.StateManager is null) {
            Instance.StateManager = CampaignSaveStateManager.CreateCampaignSave(saveLocation);
            wasLoaded = false;
        }
        return wasLoaded;
    }

    public static void LoadSaveSlot(int saveNumber) {
        LoadCampaignSave(GetSaveLocation(saveNumber));
    }

    public static void UnlockCharacter(SaveStateManager.CharacterOption c) {
        Instance.StateManager.UnlockCharacter(c);
    }

    // load save data for level that is currently in
    public static void LoadLevel(string levelName) {
        Instance.StateManager.LoadLevel(levelName, Tank.FindPlayer());
        Instance.staticStateManager.LoadLevelAchievementCheck(levelName);
    }

    public static void SwitchCharacter(SaveStateManager.CharacterOption c) {
        Instance.StateManager.SwitchCharacter(Tank.FindPlayer(), c);
        Instance.staticStateManager.SwitchCharacterAchievementCheck(c);
    }

    public static HashSet<SaveStateManager.CharacterOption> GetUnlockedCharacters() =>
        Instance.StateManager.GetUnlockedCharacters();

    public static void FinishLevel(string nextLevelName, bool toSave) {
        string finishedLevelName = Instance.StateManager.GetCurrentLevelName();
        Instance.StateManager.FinishLevel(nextLevelName, new TankStats(Tank.FindPlayer()), toSave);
        Instance.staticStateManager.FinishLevelAchievementCheck(finishedLevelName);
    }

    public static void ExitLevel() {
        Instance.StateManager.OnExitLevel();
        Instance.staticStateManager.ExitLevelAchievementCheck();
    }

    // debug method so tests can be run from Unity editor from simply starting scene
    public static void DebugLoadSave() {
        if (Instance.StateManager is null) {
            LoadSaveSlot(1);
        }
    }

    public static void CreateSave(int saveSlot) {
        CampaignSaveStateManager.CreateCampaignSave(GetSaveLocation(saveSlot));
    }

    public static void ExitCurrentSave() {
        if (Instance is null) {
            return;
        }
        if (Instance.StateManager != null) {
            Instance.StateManager.ExitSaveFile();
        }
        if (Instance.StateManager != Instance.staticStateManager) {
            Instance.staticStateManager.ExitSaveFile();
        }
        Instance.StateManager = null;
    }

    public static void PlayerDied() {
        Instance.StateManager.OnPlayerDeath();
        Instance.staticStateManager.PlayerDiedAchievementCheck();
    }

    public static void DeleteSaveSlot(int saveNumber) {
        SaveStateManager.DeleteSaveFile(GetSaveLocation(saveNumber));
    }

    public static void UpdateClassicModeHighScore(int newScore) {
        Instance.staticStateManager.UpdateClassicModeHighScore(newScore);
    }

    public static string GetLatestLevelName() => Instance.StateManager.GetLatestLevelName();

    public static void SaveToFile() {
        Instance.StateManager.SaveToFile();
    }

    public static int GetClassicModeHighScore() => Instance.staticStateManager.GetClassicModeHighScore();

    public static void LoadClassicModeSave() {
        if (Instance.StateManager != null) {
            // only load classic mode save once
            return;
        }

        Instance.staticStateManager.inClassicMode = true;
        Instance.StateManager = Instance.staticStateManager;
    }

    public static void UnlockCheckpoint(int i) {
        Instance.StateManager.UnlockCheckpoint(i, new TankStats(Tank.FindPlayer()));
    }

    public static int? GetCurrentCheckpoint() => Instance.StateManager.GetCurrentCheckpoint();

    public static SaveStateManager.CharacterOption GetCurrentCharacter() => Instance.StateManager.CurrCharacter;

    public static HashSet<StaticSaveStateManager.Achievement> GetAchievements() {
        return Instance.staticStateManager.GetAchievements();
    }

    public static void EnemyKilledByPlayer() {
        Instance.staticStateManager.EnemyKilledAchievementCheck();
    }

    public static void EnemyDamagedByPlayer(int dmg) {
        Instance.staticStateManager.EnemyDamagedByPlayerAchievementCheck(dmg);
    }

    public static void PlayerDamaged(int dmg) {
        Instance.staticStateManager.PlayerDamagedAchievementCheck(dmg);
    }

    public static int GetLevelNumberFromSceneName(string sceneName) {
        for (int i = 1; i <= CampaignSaveStateManager.MAX_LEVEL_NUMBER; i++) {
            if (sceneName.Contains(i.ToString())) {
                return i;
            }
        }

        if (sceneName == SaveStateManager.ALL_LEVELS_UNLOCKED) { // return the max level number if all levels have been unlocked
            return CampaignSaveStateManager.MAX_LEVEL_NUMBER;
        }

        return -1;
    }

    public static float GetSFXVolume() {
        return Instance.staticStateManager.GetSFXVolume();
    }

    public static float GetMusicVolume() {
        return Instance.staticStateManager.GetMusicVolume();
    }

    public static void SetVolume(float sfxVolume, float musicVolume) {
        float oldSfx = GetSFXVolume();
        float oldMusic = GetMusicVolume();
        Instance.staticStateManager.SetVolume(sfxVolume, musicVolume);
        
        // If the volume was changed update currently playing sounds
        if (oldSfx != sfxVolume || oldMusic != musicVolume) {
            foreach (AudioManager i in FindObjectsByType<AudioManager>(FindObjectsSortMode.None)) {
                i.UpdateAllSoundVolume();
            }
        }

    }

    public static void SaveStaticSaveStateManager() {
        Instance.staticStateManager.SaveToFile();
    }
}