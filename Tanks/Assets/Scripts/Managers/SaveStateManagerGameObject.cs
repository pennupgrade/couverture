using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStateManagerGameObject : MonoBehaviour
{
    public static SaveStateManagerGameObject Instance = null;

    private SaveStateManager stateManager;

    [SerializeField]
    public SaveStateManager.CharacterOption defaultTestingCat;

    private static Tank findTank() {
        return FindObjectOfType<Tank>();
    }


    // THIS IS MEANT FOR TESTING, WILL AUTOMATICALLY UNLOCK AND USE THE CAT SPECIFIED IN defaultTestingCat!!!!
    // TO HAVE THIS DO NOTHING, HAVE defaultCat set to NONE
    void setDefaultCat() {
        if (defaultTestingCat != SaveStateManager.CharacterOption.NONE) {
            Instance.stateManager.unlockCharacter(defaultTestingCat);
            Instance.stateManager.switchCharacter(findTank(), defaultTestingCat);
        }
    }

    void Awake() {
        if (Instance is null) {
            Instance = this;
            loadState();
            DontDestroyOnLoad(gameObject);
            setDefaultCat();
        } else {
            setDefaultCat();
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
    public void unlockCharacter(SaveStateManager.CharacterOption c) {
        if (stateManager.unlockCharacter(c)) {
            saveState();
        }
    }

    public void switchCharacter(SaveStateManager.CharacterOption c) {
        stateManager.switchCharacter(findTank(), c);
    }

    // SAVE AND LOAD
    public void loadState() {
        stateManager = SaveStateManager.loadInventory();
    }

    public void saveState() {
        stateManager.saveGameState();
    }
}
