using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatPFP : MonoBehaviour
{
    public SaveStateManager.CharacterOption character =
        SaveStateManager.CharacterOption.DEFAULT_CAT;

    public Image ring;
    public Image checkMark;
    public Image name_bg;
    public TMP_Text catName;
    public Button btn;
    private readonly Color32 white = new(253, 237, 230, 255);
    private readonly Color32 green = new(134, 186, 135, 255);
    private readonly Color32 black = new(74, 62, 57, 255);

    public bool selected;

    // Start is called before the first frame update
    private void Start() {
        Reset();
        selected = false;
        btn.onClick.AddListener(btnOnClick);
    }

    public void init(string catName, Sprite PFP,
                     SaveStateManager.CharacterOption character) {
        this.catName.text = catName;
        GetComponent<Image>().sprite = PFP;
        this.character = character;
    }

    public void Reset() {
        ring.color = white;
        checkMark.gameObject.SetActive(false);
        name_bg.color = black;
        selected = false;
    }

    public void Select() {
        ring.color = green;
        checkMark.gameObject.SetActive(true);
        name_bg.color = green;
        selected = true;
    }

    private void btnOnClick() {
        if (selected) {
            Reset();
        }
        else if (!selected) {
            CatPFPManager.instance.OnSelectionMade(this);
        }
    }
}