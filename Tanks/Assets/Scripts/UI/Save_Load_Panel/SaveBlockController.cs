using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private int index;
    public SaveLoadPanelManager parent;
    private SaveStateMenuInfo state;

    public void updateBlock(int i, SaveStateMenuInfo state, SaveLoadPanelManager parent) {
        this.state = state;
        index = i;
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
        btn_LoadSave.onClick.AddListener(() => StartCoroutine(_LoadSave()));
        for (var j = 0; j < 5; j++) {
            if (j < state.GetUnlockedCharacters(i).Count) {
                Instantiate(unlockedSign, catsUnlocked.transform);
            }
            else {
                Instantiate(lockedSign, catsUnlocked.transform);
            }
        }
    }

    private void delete() {
        SaveStateManagerGameObject.DeleteSaveSlot(index);
        parent.reloadPanel();
    }

    private IEnumerator _LoadSave() {
        SaveStateManagerGameObject.LoadSaveSlot(index);
        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("LevelSelect")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }
}