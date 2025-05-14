using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private Canvas[] panels;
    public GameObject Gameplay_Panel;
    public GameObject Cat_Selection_Panel;
    public PauseMenu pauseMenu;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start() {
        // Disable all canvases except for the pause canvas
        panels = GetComponentsInChildren<Canvas>().Where(c => c.gameObject.name != "Pause Menu Canvas").ToArray();
        StartGame();
    }

    private void StartGame() {
        CloseAllPanels();
        Gameplay_Panel.SetActive(true);
        GameplayHUDManager hudManager = Gameplay_Panel.GetComponentInChildren<GameplayHUDManager>();
        hudManager.ActivateAbilityIcon();
    }

    private void CloseAllPanels() {
        for (var i = 1; i < panels.Length; i++) {
            panels[i].gameObject.SetActive(false);
        }
    }

    public void QuitFrom_CatSelectionPanel_DuringGame(bool hasAbility) {
        Gameplay_Panel.SetActive(true);
        Gameplay_Panel.transform.GetChild(0).GetComponent<GameplayHUDManager>().ActivateAbilityIcon();
        if (GameManager.Instance != null) {
            GameManager.Instance.ResumeGame();
        }
        else {
            RoomManager.Instance.ResumeGame();
        }
    }

    public void Update_CatSelectionPanel_DuringGame() {
        Cat_Selection_Panel.GetComponent<CatPFPManager>().LoadPFP();
    }

    public void Open_CatSelectionPanel_DuringGame() {
        if (GameManager.Instance != null) {
            // Campaign mode only
            GameManager.Instance.PauseGame();
        }
        else {
            // Classic mode only
            RoomManager.Instance.PauseGame();
        }

        Cat_Selection_Panel.SetActive(true);
        Cat_Selection_Panel.GetComponent<CatPFPManager>().FadeInSelectionPanel(true);
        Gameplay_Panel.SetActive(false);
    }
}