// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

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
        SaveStateManagerGameObject.CreateSave(index);
        Debug.Log("save created");
        parent.reloadPanel();
    }
}
