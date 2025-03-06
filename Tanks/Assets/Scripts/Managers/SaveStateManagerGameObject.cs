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
            Instance.stateManager.SwitchCharacter(FindTank(), defaultTestingCat);
        }
    }

    void Awake() {
        if (Instance is null) {
            Instance = this;
            LoadState();
            DontDestroyOnLoad(gameObject);
            SetDefaultCat();
        } else {
            SetDefaultCat();
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
        Instance.stateManager.SaveGameState();
    }

    public static void SetCurrentCheckpoint(string levelName, int checkpointNum) {
        Instance.stateManager.currCheckpointLevelName = levelName;
        Instance.stateManager.currCheckpoint = checkpointNum;
    }

    public static void SetupCheckpointManager() {
        // load checkpoint if correct scene and checkpoint exists
        if (String.Equals(SceneManager.GetActiveScene().name, Instance.stateManager.currCheckpointLevelName) && Instance.stateManager.currCheckpoint != -1) {
            CheckpointManager.ForceSetCurrentCheckpoint(Instance.stateManager.currCheckpoint);
        }
    }

    public static void SetupGameManager() {
        if (String.Equals(SceneManager.GetActiveScene().name, Instance.stateManager.currCheckpointLevelName) && Instance.stateManager.numLives != -1) {
            GameManager.Instance.SetLives(Instance.stateManager.numLives);
        }
    }

}
