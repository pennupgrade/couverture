using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveBlockController : MonoBehaviour
{
    public TMP_Text totalPlayTime;
    public TMP_Text started;
    public TMP_Text lastPlayed;
    public GameObject catsUnlocked;
    public GameObject unlockedSign;
    public GameObject lockedSign;
    public Button btn_Delete;
    public Button btn_LoadSave;
    int index;
    public SaveLoadPanelManager parent;
    SaveStateMenuInfo state;

    public void updateBlock(int i, SaveStateMenuInfo state, SaveLoadPanelManager parent)
    {
        this.state = state;
        this.index = i;
        this.parent = parent;
        lastPlayed.text = state.GetLastPlayedTime(i).ToLongDateString();
        totalPlayTime.text = state.GetTimePlayed(i).ToString();
        started.text = state.GetStartTime(i).ToLongDateString();
        btn_Delete.onClick.AddListener(delete);
        btn_LoadSave.onClick.AddListener(loadSave);
        for (int j = 0; j < 5; j++)
        {
            if(j < state.GetUnlockedCharacters(i).Count)
            {
                GameObject.Instantiate(unlockedSign, catsUnlocked.transform);
            }
            else
            {
                GameObject.Instantiate(lockedSign, catsUnlocked.transform);
            }
        }
    }

    public void delete()
    {
        SaveStateManagerGameObject.DeleteSave(index);
        parent.reloadPanel();
    }

    public void loadSave()
    {
        SaveStateManagerGameObject.LoadSave(index);
        Debug.Log("Latest Level: " + state.GetLatestLevelName(index));
        SceneManager.LoadScene("LevelSelect");
    }
}
