using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveStateManagerGameObject : MonoBehaviour
{
    private const string SAVE_FILE_PREFIX = "save_data_";
    public const string CLASSIC_MODE_SAVE_FILE = SAVE_FILE_PREFIX + "classic_mode.json";
    public static SaveStateManagerGameObject Instance = null;

    private SaveStateManager stateManager = null;

    // TODO: should i optimize this (FindTank only at loadlevel)?
    private static Tank FindTank() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("SaveStateManagerGameObject: Could not find player");
        }
        return player.GetComponent<Tank>();
    }

    void Awake() {
        if (Instance is null) {
            Instance = this;
            Application.quitting += ExitCurrentSave;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public static string GetSaveLocation(int saveNumber) {
        return SAVE_FILE_PREFIX + saveNumber + ".json";
    }

    // returns true if save was loaded, false if save was created
    private static bool LoadSave(string saveLocation) {
        if (Instance.stateManager != null) {
            throw new InvalidOperationException("A Save State is Already Open!");
        }
        bool wasLoaded = true;
        Instance.stateManager = SaveStateManager.TryLoadSaveState(saveLocation);
        if (Instance.stateManager is null) {
            Instance.stateManager = SaveStateManager.CreateSave(saveLocation);
            wasLoaded = false;
        }
        // start the session
        Instance.stateManager.BeginSession();
        return wasLoaded;
    }

    public static void LoadSaveSlot(int saveNumber) {
        LoadSave(GetSaveLocation(saveNumber));
    }

    public static void UnlockCharacter(SaveStateManager.CharacterOption c) {
        Instance.stateManager.UnlockCharacter(c);
    }

    // load save data for level that is currently in
    public static void LoadLevel(string levelName) {
        Instance.stateManager.LoadLevel(levelName, Tank.FindPlayer());
    }

    public static void SwitchCharacter (SaveStateManager.CharacterOption c) {
        Instance.stateManager.SwitchCharacter(Tank.FindPlayer(), c);
    }

    public static HashSet<SaveStateManager.CharacterOption> GetUnlockedCharacters() {
        return Instance.stateManager.GetUnlockedCharacters();
    }

    public static void FinishLevel(string nextLevelName, bool toSave) {
        Instance.stateManager.FinishLevel(nextLevelName, new TankStats(Tank.FindPlayer()), toSave);
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
        SaveStateManager.CreateSave(GetSaveLocation(saveSlot)).SaveToFile();
    }

    public static void ExitCurrentSave() {
        if (Instance is null || Instance.stateManager is null) {
            return;
        }
        Instance.stateManager.ExitSaveFile();
        Instance.stateManager = null;
    }

    public static void PlayerDied() {
        Instance.stateManager.OnPlayerDeath();
    }

    public static void DeleteSaveSlot(int saveNumber) {
        SaveStateManager.DeleteSaveFile(GetSaveLocation(saveNumber));
    }

    public static void UpdateClassicModeHighScore(int newScore) {
        Instance.stateManager.UpdateClassicModeHighScore(newScore);
    }

    public static string GetLatestLevelName() {
        return Instance.stateManager.GetLatestLevelName();
    }

    public static void SaveToFile() {
        Instance.stateManager.SaveToFile();
    }

    public static int GetClassicModeHighScore() {
        return Instance.stateManager.GetClassicModeHighScore();
    }

    public static void LoadClassicModeSave() {
        if (Instance.stateManager != null) { // only load classic mode save once
            return;
        }
        LoadSave(CLASSIC_MODE_SAVE_FILE);
        SaveStateManager.CharacterOption[] allChars = (SaveStateManager.CharacterOption[]) Enum.GetValues(typeof(SaveStateManager.CharacterOption));
        if (!GetUnlockedCharacters().SetEquals(allChars)) { // if unlocked characters arent all characters, won't handle updates that remove characters well
            Instance.stateManager.ForceUnlockCharacters(allChars);
            LoadLevel("NULL"); // this works fine as long as there is no level with scene name "NULL", but it is a tad bit jank...
            ExitLevel();
        }
    }

    public static void UnlockCheckpoint(int i) {
        Instance.stateManager.UnlockCheckpoint(i, new(Tank.FindPlayer()));
    }

    public static int? GetCurrentCheckpoint() {
        return Instance.stateManager.GetCurrentCheckpoint();
    }

    public static SaveStateManager.CharacterOption GetCurrentCharacter() {
        return Instance.stateManager.CurrCharacter;
    }


    // helper function
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

        return -1;
    }
}
