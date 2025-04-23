using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private string[] levels;
    [SerializeField] private int unlockedLevelCount;
    [SerializeField] private LevelSelectDisplay[] levelDisplays;
    [SerializeField] private RectTransform[] pages;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    private int currentPage;

    private void Awake() {
        var sceneName = SaveStateManagerGameObject.GetLatestLevelName();
        var levelNumber = SaveStateManager.GetLevelNumberFromSceneName(sceneName);
        unlockedLevelCount = levelNumber != -1 ? levelNumber : 1;
    }

    private void Start() {
        for (var i = 0; i < levelDisplays.Length; i++) {
            levelDisplays[i].LockLevel(i >= unlockedLevelCount);
        }

        nextPageButton.interactable = currentPage < pages.Length - 1;
        prevPageButton.interactable = currentPage > 0;
    }

    public void ShiftPages(bool left) {
        currentPage = Mathf.Clamp(currentPage + (left ? -1 : 1), 0, pages.Length - 1);
        for (var i = 0; i < pages.Length; i++) {
            LeanTween.move(pages[i], new Vector2(1920f * i - 1920f * currentPage, pages[i].anchoredPosition.y), 0.7f)
                     .setEase(LeanTweenType.easeInOutQuart).setIgnoreTimeScale(true);
        }

        nextPageButton.interactable = currentPage < pages.Length - 1;
        prevPageButton.interactable = currentPage > 0;
    }

    public void SelectLevel(int level) {
        if (level < 0 || level >= levels.Length) return;

        StartCoroutine(_SelectLevel(level));
    }

    private IEnumerator _SelectLevel(int level) {
        var rt = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

        SceneTransition.I.UpdateIrisPosition(rt.position.x / Screen.width, rt.position.y / Screen.height);
        SceneTransition.I.Appear(SceneTransition.TransitionType.Level, level + 1);

        var operation = SceneManager.LoadSceneAsync(levels[level])!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }

    public void ReturnToSaveSelectScreen() => StartCoroutine(_ReturnToSaveSelectScreen());

    private static IEnumerator _ReturnToSaveSelectScreen() {
        SaveStateManagerGameObject.ExitCurrentSave();
        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("Save Select")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }
}