using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatPFPManager : MonoBehaviour
{
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
        SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.BUBBLE_CAT);
        SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.ROCKET_CAT);
        SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.NONE);
    }

    // Start is called before the first frame update
    void Start()
    {
        HashSet<SaveStateManager.CharacterOption> characters =
            SaveStateManagerGameObject.GetUnlockedCharacters();
        Debug.Log("# of characters: " + characters.Count);
        foreach (SaveStateManager.CharacterOption character in characters)
        {
            GameObject catPFP = GameObject.Instantiate(CatPFP_obj, CatsPFPPanel);
            CatPFP pfp = catPFP.GetComponent<CatPFP>();
            setPFP(character, pfp);
            pfps.Add(pfp);
        }
        btn_Confirm.onClick.AddListener(ConfirmBtnOnClick);
    }

    private void setPFP(SaveStateManager.CharacterOption character, CatPFP catPFP)
    {
        if (character == SaveStateManager.CharacterOption.NONE)
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
        SaveStateManagerGameObject.SwitchCharacter(currentPFP.character);
        Debug.Log("selected: " + currentPFP.character.ToString());
        gameObject.SetActive(false);
    }

}
