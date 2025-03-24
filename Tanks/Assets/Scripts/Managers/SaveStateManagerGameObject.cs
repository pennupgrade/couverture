using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveStateManagerGameObject : MonoBehaviour
{
    private const string SAVE_FILE_PREFIX = "save_data_";
    public static SaveStateManagerGameObject Instance = null;

    private SaveStateManager stateManager = null;

    // TODO: should i optimize this (FindTank only at loadlevel)?
    private static Tank FindTank() {
        return FindObjectOfType<Tank>();
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

    public static SaveStateManager LoadSaveToManager(int saveNumber) {
        return SaveStateManager.LoadInventory(GetSaveLocation(saveNumber));
    }

    public static void LoadSave(int saveNumber) {
        try {
            Instance.stateManager = LoadSaveToManager(saveNumber);
        } catch (FileNotFoundException) {
            Instance.stateManager = SaveStateManager.CreateSave(GetSaveLocation(saveNumber));
        }
        // start the session
        Instance.stateManager.BeginSession();
    }

    public static void UnlockCharacter(SaveStateManager.CharacterOption c) {
        Instance.stateManager.UnlockCharacter(c);
    }

    // load save data for level that is currently in
    public static void LoadLevel(string levelName) {
        Instance.stateManager.LoadLevel(levelName, FindTank());
    }

    public static void SwitchCharacter (SaveStateManager.CharacterOption c) {
        Instance.stateManager.SwitchCharacter(FindTank(), c);
    }

    public static HashSet<SaveStateManager.CharacterOption> GetUnlockedCharacters() {
        return Instance.stateManager.GetUnlockedCharacters();
    }

    public static void FinishLevel(string nextLevelName) {
        Instance.stateManager.FinishLevel(nextLevelName, new TankStats(FindTank()));
    }

    public static void ExitLevel() {
        Instance.stateManager.OnExitLevel();
    }

    // debug method so tests can be run from Unity editor from simply starting scene
    public static void DebugLoadSave() {
        if (Instance.stateManager is null) {
            LoadSave(1);
        }
    }
    
    public static void CreateSave(int saveSlot) {
        Instance.stateManager = SaveStateManager.CreateSave(GetSaveLocation(saveSlot));
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
}
