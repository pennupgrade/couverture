// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatPFP : MonoBehaviour
{
    public SaveStateManager.CharacterOption character = SaveStateManager.CharacterOption.DEFAULT_CAT;

    public Image ring;
    public Image checkMark;
    public Image name_bg;
    public TMP_Text catName;
    public Button btn;

    public static readonly Color32 Cream = new(255, 238, 229, 255);
    public static readonly Color32 Brown = new(78, 63, 58, 255);
    private static readonly Color32 Green = new(134, 186, 135, 255);

    private bool selected;

    // Start is called before the first frame update
    private void Start() {
        btn.onClick.AddListener(btnOnClick);
    }

    public void Init(string initCatName, Sprite initPFP, SaveStateManager.CharacterOption initCharacter) {
        catName.text = initCatName;
        GetComponent<Image>().sprite = initPFP;
        character = initCharacter;
    }

    public void Reset() {
        ring.color = Cream;
        catName.color = Cream;
        checkMark.gameObject.SetActive(false);
        name_bg.color = Brown;
        selected = false;
    }

    public void Select() {
        ring.color = Green;
        catName.color = Brown;
        checkMark.gameObject.SetActive(true);
        name_bg.color = Green;
        selected = true;
    }

    private void btnOnClick() {
        CatPFPManager.instance.OnSelectionMade(selected ? null : this);
    }
}