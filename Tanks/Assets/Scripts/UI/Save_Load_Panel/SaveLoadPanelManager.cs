// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
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
        btn_Back.onClick.AddListener(ExitButtonOnClick);
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

    private void ExitButtonOnClick() => StartCoroutine(_ExitButtonOnClick());

    private static IEnumerator _ExitButtonOnClick() {
        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("TitleScreen")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }
}