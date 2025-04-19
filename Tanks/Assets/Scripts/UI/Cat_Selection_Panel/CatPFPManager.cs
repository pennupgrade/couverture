using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatPFPManager : MonoBehaviour
{
    //if new cats are added, need to update the sprite, the list, the setPFP function
    public static CatPFPManager instance;

    public GameObject CatPFP_obj;
    public Button btn_Confirm;
    public Transform CatsPFPPanel;

    public Sprite sprite_OrangeCat;
    public Sprite sprite_BubbleCat;
    public Sprite sprite_RocketCat;
    public Sprite sprite_UnknownCat;

    private List<CatPFP> pfps = new List<CatPFP>();
    private CatPFP currentPFP = null;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadPFP();
    }

    public void LoadPFP()
    {
        // optimize later
        foreach (Transform child in CatsPFPPanel)
        {
            Destroy(child.gameObject);
        }

        HashSet<SaveStateManager.CharacterOption> characters =
            SaveStateManagerGameObject.GetUnlockedCharacters();
        foreach (SaveStateManager.CharacterOption character in characters)
        {
            // DEFAULT_CAT has no ability, so why make it an option?
            // - Anthony
            if (character == SaveStateManager.CharacterOption.DEFAULT_CAT) continue;

            GameObject catPFP = Instantiate(CatPFP_obj, CatsPFPPanel);
            CatPFP pfp = catPFP.GetComponent<CatPFP>();
            setPFP(character, pfp);
            pfps.Add(pfp);
        }

        btn_Confirm.onClick.AddListener(ConfirmBtnOnClick);
    }

    private void setPFP(SaveStateManager.CharacterOption character, CatPFP catPFP)
    {
        if (character == SaveStateManager.CharacterOption.DEFAULT_CAT)
        {
            catPFP.init("Orange Cat", sprite_OrangeCat, character);
        }
        else if (character == SaveStateManager.CharacterOption.BUBBLE_CAT)
        {
            catPFP.init("Bubble Cat", sprite_BubbleCat, character);
        }
        else if (character == SaveStateManager.CharacterOption.ROCKET_CAT)
        {
            catPFP.init("Rocket Cat", sprite_RocketCat, character);
        }
        else
        {
            catPFP.init("Orange Cat", sprite_OrangeCat, character);
        }
    }

    public void OnSelectionMade(CatPFP newCat)
    {
        if(currentPFP != null)
            currentPFP.Reset();
        newCat.Select();
        currentPFP = newCat;
    }

    public void ConfirmBtnOnClick()
    {
        if (currentPFP != null)
        {
            UIManager.instance.Gameplay_Panel.GetComponentInChildren<GameplayHUDManager>().
                    readyTag.gameObject.SetActive(true);
            if (Tank.FindPlayer().character is BubbleChar)
            {
                Destroy(((BubbleChar)(Tank.FindPlayer().character)).obj);
                UIManager.instance.Gameplay_Panel.GetComponentInChildren<GameplayHUDManager>().
                    readyTag.GetComponentInChildren<TMP_Text>().text = "Ready!";
            }
            UIManager.instance.Gameplay_Panel.GetComponentInChildren<GameplayHUDManager>().
                abilityBarFill.fillAmount = 1;
            SaveStateManagerGameObject.SwitchCharacter(currentPFP.character);
            UIManager.instance.characterJustSwitched = true;
        }
        UIManager.instance.QuitFrom_CatSelectionPanel_DuringGame();
    }

}
