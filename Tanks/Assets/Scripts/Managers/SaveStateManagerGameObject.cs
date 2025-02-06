using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

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

    public static HashSet<SaveStateManager.CharacterOption> getUnlockedCharacters() {
        HashSet<SaveStateManager.CharacterOption> outSet = new();
        // recreates a new hashset, so that is a bit of extra computation, but it means things are better encapsulated
        foreach (SaveStateManager.CharacterOption c in Instance.stateManager.unlockedCharList) {
            outSet.Add(c);
        }
        return outSet;
    }

    // SAVE AND LOAD
    public static void LoadState() {
        Instance.stateManager = SaveStateManager.LoadInventory();
    }

    public static void SaveState() {
        Instance.stateManager.SaveGameState();
    }
}
