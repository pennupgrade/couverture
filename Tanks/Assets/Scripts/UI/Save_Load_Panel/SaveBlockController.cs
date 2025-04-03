using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveBlockController : MonoBehaviour
{
    public TMP_Text totalPlayTime;
    public TMP_Text started;
    public TMP_Text lastPlayed;
    public GameObject catsUnlocked;
    public GameObject unlockedSign;
    public GameObject lockedSign;
    public Button btn_Delete;
    int index;

    public void updateBlock(int i, SaveStateMenuInfo state)
    {
        this.index = i;
        lastPlayed.text = state.GetLastPlayedTime(i).ToLongDateString();
        totalPlayTime.text = state.GetTimePlayed(i).ToString();
        started.text = state.GetStartTime(i).ToLongDateString();
        btn_Delete.onClick.AddListener(delete);
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
    }
}
