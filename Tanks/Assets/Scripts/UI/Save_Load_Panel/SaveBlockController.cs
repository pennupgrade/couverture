using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveBlockController : MonoBehaviour
{
    [SerializeField] private TMP_Text highestUnlockedLevel;
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

        var latestLevelName = state.GetLatestLevelName(i);
        if (latestLevelName == "NOT_A_LEVEL") {
            // This means we haven't played any level yet i.e. just created the save.
            // Set it to "Level 1" because it's unlocked by default
            highestUnlockedLevel.text = "Level 1";
        }
        else {
            highestUnlockedLevel.text = state.GetLatestLevelNameFormatted(i);
        }
        
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
        SaveStateManagerGameObject.DeleteSaveSlot(index);
        parent.reloadPanel();
    }

    public void loadSave()
    {
        SaveStateManagerGameObject.LoadSaveSlot(index);
        Debug.Log("Latest Level: " + state.GetLatestLevelName(index));
        SceneManager.LoadScene("LevelSelect");
    }
}
