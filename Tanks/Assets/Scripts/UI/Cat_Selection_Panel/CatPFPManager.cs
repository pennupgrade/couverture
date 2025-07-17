// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatPFPManager : MonoBehaviour
{
    //if new cats are added, need to update the sprite, the list, the setPFP function
    public static CatPFPManager instance;

    public GameObject CatPFP_obj;
    public Button btn_Confirm;
    public Transform CatsPFPPanel;
    [SerializeField] private TMP_Text confirmationText;
    [SerializeField] private CanvasGroup confirmBtnCg;

    public Sprite sprite_BubbleCat;
    public Sprite sprite_RocketCat;

    private CatPFP currentPFP;

    // NOTE: this Canvas Group references the overlay from the Pause Menu Canvas!!
    public CanvasGroup canvasGroup;

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start() {
        LoadPFP();
    }

    public void LoadPFP() {
        // optimize later
        foreach (Transform child in CatsPFPPanel) {
            Destroy(child.gameObject);
        }

        var unlockedCharacters = SaveStateManagerGameObject.GetUnlockedCharacters();
        var currentCharacter = SaveStateManagerGameObject.GetCurrentCharacter();

        foreach (var character in unlockedCharacters) {
            // DEFAULT_CAT has no ability, so why make it an option? - Anthony
            if (character == SaveStateManager.CharacterOption.DEFAULT_CAT) {
                OnSelectionMade(null);
                continue;
            }

            var catPFP = Instantiate(CatPFP_obj, CatsPFPPanel);
            var pfp = catPFP.GetComponent<CatPFP>();

            switch (character) {
            case SaveStateManager.CharacterOption.BUBBLE_CAT:
                pfp.Init("Bubble Cat", sprite_BubbleCat, character);
                break;

            case SaveStateManager.CharacterOption.ROCKET_CAT:
                pfp.Init("Rocket Cat", sprite_RocketCat, character);
                break;

            default:
                throw new ArgumentOutOfRangeException($"{character} is not handled!");
            }

            // Pre-select the current tank character
            if (character == currentCharacter) {
                OnSelectionMade(pfp);
            }
        }

        btn_Confirm.onClick.AddListener(ConfirmBtnOnClick);
    }

    public void OnSelectionMade(CatPFP newCat) {
        if (currentPFP != null) {
            currentPFP.Reset();
        }

        currentPFP = newCat;
        btn_Confirm.interactable = newCat != null;

        if (newCat == null) {
            confirmationText.text = "You haven't selected a cat yet.";
            confirmBtnCg.alpha = 0.5f;
            return;
        }

        confirmBtnCg.alpha = 1;
        newCat.Select();

        // Default cat should not be selectable
        var catName = currentPFP.character switch {
            SaveStateManager.CharacterOption.ROCKET_CAT => "Rocket Cat",
            SaveStateManager.CharacterOption.BUBBLE_CAT => "Bubble Cat",
            _ => throw new ArgumentOutOfRangeException()
        };

        confirmationText.text = $"You've selected the <b>{catName}</b>!";
    }

    private void ConfirmBtnOnClick() {
        if (currentPFP != null) {
            var gameplayHudManager = UIManager.Instance.Gameplay_Panel.GetComponentInChildren<GameplayHUDManager>();
            gameplayHudManager.readyTag.gameObject.SetActive(true);

            if (Tank.FindPlayer().character is BubbleChar) {
                Destroy(((BubbleChar)Tank.FindPlayer().character).obj);
                gameplayHudManager.readyTag.GetComponentInChildren<TMP_Text>().text = "Ready!";
            }

            gameplayHudManager.abilityBarFill.fillAmount = 1;
            SaveStateManagerGameObject.SwitchCharacter(currentPFP.character);
        }

        FadeInSelectionPanel(false);
        UIManager.Instance.QuitFrom_CatSelectionPanel_DuringGame(Tank.FindPlayer().character is not DefaultChar);
    }

    public void FadeInSelectionPanel(bool newEnabled) {
        if (newEnabled) {
            canvasGroup.alpha = 0f;
            LeanTween.alphaCanvas(canvasGroup, 1f, 0.15f).setIgnoreTimeScale(true);
        }
        else {
            canvasGroup.alpha = 1f;
            LeanTween.alphaCanvas(canvasGroup, 0f, 0.15f).setIgnoreTimeScale(true)
                     .setOnComplete(() => UIManager.Instance.Cat_Selection_Panel.SetActive(false));
        }
    }
}