using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStateManagerGameObject : MonoBehaviour
{
    public static SaveStateManagerGameObject Instance = null;

    public SaveStateManager statemanager;

    [SerializeField]
    public SaveStateManager.CharacterOption defaultTestingCat;


    // THIS IS MEANT FOR TESTING, WILL AUTOMATICALLY UNLOCK AND USE THE CAT SPECIFIED IN defaultTestingCat!!!!
    // TO HAVE THIS DO NOTHING, HAVE defaultCat set to NONE
    void setDefaultCat() {
        if (defaultTestingCat != SaveStateManager.CharacterOption.NONE) {
            Instance.statemanager.unlockCharacter(defaultTestingCat);
            Instance.statemanager.switchCharacter(FindObjectOfType<Tank>(), defaultTestingCat);
        }
    }

    void Awake() {
        if (Instance is null) {
            Instance = this;
            statemanager = SaveStateManager.loadInventory();
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
}
