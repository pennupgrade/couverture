using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadPanelManager : MonoBehaviour
{
    public GameObject SaveBlock;
    public GameObject NewBlock;
    public GameObject[] Save123;
    public List<GameObject> blocks = new();
    public Button btn_Back;

    // Start is called before the first frame update
    private void Start() {
        btn_Back.onClick.AddListener(exitButtonOnClick);
        reloadPanel();
    }

    public void reloadPanel() {
        foreach (var block in blocks) {
            if (block != null) {
                Destroy(block);
            }
        }

        blocks = new List<GameObject>();

        var saveState = new SaveStateMenuInfo();
        for (var i = 0; i < 3; i++) {
            if (saveState.DoesSaveExist(i)) {
                var saveBlock = Instantiate(SaveBlock, Save123[i].transform);
                saveBlock.GetComponent<SaveBlockController>().updateBlock(i, saveState, this);
                blocks.Add(saveBlock);
                Save123[i].GetComponent<Image>().color = new Color(1, 1, 1);
            }
            else {
                var newBlock = Instantiate(NewBlock, Save123[i].transform);
                blocks.Add(newBlock);
                newBlock.GetComponent<NewSave>().parent = this;
                Save123[i].GetComponent<Image>().color = new Color32(164, 164, 164, 255);
                newBlock.GetComponent<NewSave>().index = i;
            }
        }
    }

    public void exitButtonOnClick() => SceneManager.LoadScene("TitleScreen");
}