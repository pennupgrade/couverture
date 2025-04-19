using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Canvas[] panels;
    public GameObject Gameplay_Panel;
    public GameObject Cat_Selection_Panel;
    public GameObject Save_Load_Panel;
    public PauseMenu pauseMenu;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Disable all canvases except for the pause canvas
        panels = GetComponentsInChildren<Canvas>().Where(c => c.gameObject.name != "Pause Menu Canvas").ToArray();
        StartGame();
    }

    public void StartGame()
    {
        CloseAllPanels();
        Gameplay_Panel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        for (int i = 1; i < panels.Length; i++)
        {
            panels[i].gameObject.SetActive(false);
        }
    }

    public void QuitFrom_CatSelectionPanel_DuringGame()
    {
        Cat_Selection_Panel.SetActive(false);
        Gameplay_Panel.SetActive(true);
        Gameplay_Panel.transform.GetChild(0).GetComponent<GameplayHUDManager>().EnableAbilityBar(true);
        if (GameManager.Instance != null) {
            GameManager.Instance.ResumeGame();
        } else {
            RoomManager.Instance.ResumeGame();
        }
    }

    public void Update_CatSelectionPanel_DuringGame()
    {
        CatPFPManager PFPManager = Cat_Selection_Panel.GetComponent<CatPFPManager>();
        PFPManager.LoadPFP();
    }

    public void Open_CatSelectionPanel_DuringGame()
    {
        if (GameManager.Instance != null) {
            // Campaign mode only
            GameManager.Instance.PauseGame();
        } else {
            // Classic mode only
            RoomManager.Instance.PauseGame();
        }
        Cat_Selection_Panel.SetActive(true);
        Gameplay_Panel.SetActive(false);
    }
}
