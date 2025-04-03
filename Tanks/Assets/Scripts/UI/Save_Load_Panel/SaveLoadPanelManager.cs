using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadPanelManager : MonoBehaviour
{
    public GameObject SaveBlock;
    public GameObject NewBlock;
    public GameObject[] Save123;
    public Button btn_Back;
    // Start is called before the first frame update
    void Start()
    {
        btn_Back.onClick.AddListener(exitButtonOnClick);
        SaveStateMenuInfo saveState = new SaveStateMenuInfo();
        for (int i = 0; i < 3; i++)
        {
            if (saveState.DoesSaveExist(i))
            {
                GameObject saveBlock = GameObject.Instantiate(SaveBlock, Save123[i].transform);
                saveBlock.GetComponent<SaveBlockController>().updateBlock(i, saveState);
                Save123[i].GetComponent<Image>().color = new Color(1, 1, 1);

            }
            else
            {
                GameObject newBlock = GameObject.Instantiate(NewBlock, Save123[i].transform);
                Save123[i].GetComponent<Image>().color = new Color32(164, 164, 164, 255);
                newBlock.GetComponent<NewSave>().index = i;
            }
        }
    }

    public void exitButtonOnClick()
    {
        UIManager.instance.QuitFrom_SaveLoadPanel_DuringGame();
    }
}
