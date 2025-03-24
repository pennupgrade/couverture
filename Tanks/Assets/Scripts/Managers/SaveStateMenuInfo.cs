using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveStateMenuInfo : MonoBehaviour
{
    private SaveStateManager[] saves = new SaveStateManager[3];

    private static SaveStateManager TryLoadSaveState(int saveNumber) {
        if (File.Exists(SaveStateManagerGameObject.GetSaveLocation(saveNumber))) {
            return SaveStateManagerGameObject.LoadSaveToManager(saveNumber);
        }
        return null;
    }

    void Awake() {
        for (int i = 0; i < 3; i++) {
            saves[i] = TryLoadSaveState(i);
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
