using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int unlockedLevelCount;
    [SerializeField] private LevelSelectDisplay[] levelDisplays;
    [SerializeField] private RectTransform[] pages;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    private int currentPage;

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
        Debug.Log("loading level " + level);
    }
}
