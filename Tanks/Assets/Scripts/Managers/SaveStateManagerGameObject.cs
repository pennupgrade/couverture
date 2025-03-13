using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveStateManagerGameObject : MonoBehaviour
{
    public static SaveStateManagerGameObject Instance = null;

    private SaveStateManager stateManager;

    [SerializeField]
    public SaveStateManager.CharacterOption defaultTestingCat;

    private static Tank FindTank() {
        return FindObjectOfType<Tank>();
    }


    // THIS IS MEANT FOR TESTING, WILL AUTOMATICALLY UNLOCK AND USE THE CAT SPECIFIED IN defaultTestingCat!!!!
    // TO HAVE THIS DO NOTHING, HAVE defaultCat set to NONE
    void SetDefaultCat() {
        if (defaultTestingCat != SaveStateManager.CharacterOption.NONE) {
            Instance.stateManager.UnlockCharacter(defaultTestingCat);
            SwitchCharacter(defaultTestingCat);
        }
    }

    private static void SetCurrCat() {
        if (Instance.stateManager.currCharacter != SaveStateManager.CharacterOption.NONE) {
            SwitchCharacter(Instance.stateManager.currCharacter);
        }
    }

    void Awake() {
        if (Instance is null) {
            Instance = this;
            LoadState();
            DontDestroyOnLoad(gameObject);
            SetCurrCat();
            SetDefaultCat();
        } else {
            SetDefaultCat();
            SetCurrCat();
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Character setting/getting/unlocking
    public static void UnlockCharacter(SaveStateManager.CharacterOption c) {
        if (Instance.stateManager.UnlockCharacter(c)) {
            SaveState();
        }
    }

    public static void SwitchCharacter(SaveStateManager.CharacterOption c) {
        Instance.stateManager.SwitchCharacter(FindTank(), c);
    }

    public static HashSet<SaveStateManager.CharacterOption> GetUnlockedCharacters() {
        return Instance.stateManager.GetUnlockedCharacters();
    }

    // SAVE AND LOAD
    private static void LoadState() {
        Instance.stateManager = SaveStateManager.LoadInventory();
    }

    public static void SaveState() {
        if (GameManager.Instance is null) {
            throw new InvalidOperationException("GameManager Instance is Null!");
        }
        Instance.stateManager.numLives = GameManager.Instance.GetLives();
        Instance.stateManager.SaveGameState();
    }

    public static void SetCurrentCheckpoint(string levelName, int checkpointNum) {
        Instance.stateManager.currCheckpointLevelName = levelName;
        Instance.stateManager.currCheckpoint = checkpointNum;
    }

    private static bool IsCorrectScene() {
        return string.Equals(SceneManager.GetActiveScene().name, Instance.stateManager.currCheckpointLevelName);
    }

    public static void SetupCheckpointManager() {
        if (CheckpointManager.Instance is null) {
            return;
        }
        // load checkpoint if correct scene and checkpoint exists
        if (IsCorrectScene() && Instance.stateManager.currCheckpoint != -1) {
            CheckpointManager.ForceSetCurrentCheckpoint(Instance.stateManager.currCheckpoint);
        }
    }

    public static void SetupGameManager() {
        if (GameManager.Instance is null) {
            return;
        }
        if (IsCorrectScene() && Instance.stateManager.numLives != -1) {
            GameManager.Instance.SetLives(Instance.stateManager.numLives);
        }
    }

    public static void EndLevel() {
        Instance.stateManager.FullUnlockCurrCharacters();
        // a bit of repetative code (recreating oldUnlockedCharList), but it is not much
        Instance.stateManager.ResetToOld();
        Instance.stateManager.SaveGameState();
    }

    public static void RestartLevel() {
        Instance.stateManager.ResetToOld();
        Instance.stateManager.SaveGameState();
    }

    public static void StartLevel(bool continueGame) {
        if (!continueGame) {
            RestartLevel();
        }
    }
}
