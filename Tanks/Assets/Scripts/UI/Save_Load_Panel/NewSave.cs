using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSave : MonoBehaviour
{
    public int index;
    public Button btn_CreateNewSave;
    public SaveLoadPanelManager parent;
    // Start is called before the first frame update
    void Start()
    {
        btn_CreateNewSave.onClick.AddListener(createNewSave);
    }

    private void createNewSave()
    {
        SaveStateManagerGameObject.LoadSaveSlot(index);
        Debug.Log("save created");
        parent.reloadPanel();
    }
}
