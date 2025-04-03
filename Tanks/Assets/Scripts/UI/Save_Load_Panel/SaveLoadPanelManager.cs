using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadPanelManager : MonoBehaviour
{
    public GameObject SaveBlock;
    public GameObject NewBlock;
    public GameObject[] Save123;
    public List<GameObject> blocks = new List<GameObject>();
    public Button btn_Back;
    // Start is called before the first frame update
    void Start()
    {
        btn_Back.onClick.AddListener(exitButtonOnClick);
        reloadPanel();
    }

    public void reloadPanel()
    {
        foreach (GameObject block in blocks)
        {
            if(block != null)
            {
                GameObject.Destroy(block);
            }
        }
        blocks = new List<GameObject>();

        SaveStateMenuInfo saveState = new SaveStateMenuInfo();
        for (int i = 0; i < 3; i++)
        {
            if (saveState.DoesSaveExist(i))
            {
                GameObject saveBlock = GameObject.Instantiate(SaveBlock, Save123[i].transform);
                saveBlock.GetComponent<SaveBlockController>().updateBlock(i, saveState, this);
                blocks.Add(saveBlock);
                Save123[i].GetComponent<Image>().color = new Color(1, 1, 1);

            }
            else
            {
                GameObject newBlock = GameObject.Instantiate(NewBlock, Save123[i].transform);
                blocks.Add(newBlock);
                newBlock.GetComponent<NewSave>().parent = this;
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
