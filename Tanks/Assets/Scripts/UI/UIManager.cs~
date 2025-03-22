using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Canvas[] panels;
    public GameObject Gameplay_Panel;
    public GameObject Cat_Selection_Panel;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        panels = GetComponentsInChildren<Canvas>();
        Debug.Log("len: " + panels.Length);
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
    }

    public void Open_CatSelectionPanel_DuringGame()
    {
        Cat_Selection_Panel.SetActive(true);
        Gameplay_Panel.SetActive(false);
    }
}
