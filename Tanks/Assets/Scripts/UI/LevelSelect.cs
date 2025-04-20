using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Awake()
    {
        string name = SaveStateManagerGameObject.GetLatestLevelName();
        Debug.Log("Latestlevel: " + name);
        if (name.Contains("1"))
            unlockedLevelCount = 1;
        else if (name.Contains("2"))
            unlockedLevelCount = 2;
        else if (name.Contains("3"))
            unlockedLevelCount = 3;
        else if (name.Contains("4"))
            unlockedLevelCount = 4;
        else if (name.Contains("5"))
            unlockedLevelCount = 5;
        else if (name.Contains("6"))
            unlockedLevelCount = 6;
        else
        {
            unlockedLevelCount = 1;
        }
    }

    private void Start()
    {
        for (int i = 0; i < levelDisplays.Length; i++)
        {
            levelDisplays[i].LockLevel(i >= unlockedLevelCount);
        }
        nextPageButton.interactable = currentPage < pages.Length - 1;
        prevPageButton.interactable = currentPage > 0;
    }

    public void ShiftPages(bool left)
    {
        currentPage = Mathf.Clamp(currentPage + (left ? -1 : 1), 0, pages.Length - 1);
        for (int i = 0; i < pages.Length; i++)
        {
            LeanTween.move(pages[i], new Vector2(1920f * i - 1920f * currentPage, pages[i].anchoredPosition.y), 0.7f).setEase(LeanTweenType.easeInOutQuart).setIgnoreTimeScale(true);
        }
        nextPageButton.interactable = currentPage < pages.Length - 1;
        prevPageButton.interactable = currentPage > 0;
    }
    
    public void SelectLevel(int level)
    {
        if (level >= levels.Length) return;
        SceneManager.LoadScene(levels[level]);
    }

    public void ReturnToSaveSelectScreen() {
        SaveStateManagerGameObject.ExitCurrentSave();
        SceneManager.LoadScene("Save Select");
    }
}
