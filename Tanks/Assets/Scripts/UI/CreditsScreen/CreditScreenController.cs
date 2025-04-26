using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class CreditEntry
{
    public string role;
    public List<string> names;
}

[Serializable]
public class CreditSection
{
    public string section;
    public List<CreditEntry> credits;
}

[Serializable]
public class CreditsData
{
    public string title;
    public string subtitle;
    public List<CreditSection> data;
}

public class CreditScreenController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public float scrollSpeed;
    public GameObject prefab_CreditsBlock;
    public GameObject prefab_SectionTitle;
    public GameObject content;
    public TextAsset jsonFile;
    public Button returnBtn;
    private bool isScrolling;
    public GameObject terribleHackToAddPadding;

    private bool readyToScroll;

    private void Awake() {
        isScrolling = false;
        GenerateCredits();
        StartCoroutine(InitAndStartScroll());
        returnBtn.onClick.AddListener(() => StartCoroutine(ReturnToMain()));
    }

    private static IEnumerator ReturnToMain() {
        SceneTransition.I.Appear(SceneTransition.TransitionType.Fade);

        var operation = SceneManager.LoadSceneAsync("TitleScreen")!;
        operation.allowSceneActivation = false;

        yield return new WaitWhile(() => SceneTransition.I.IsAnimating);

        operation.allowSceneActivation = true;
    }

    private void GenerateCredits() {
        content.SetActive(true); // Must be active before building

        var creditsData = JsonUtility.FromJson<CreditsData>(jsonFile.text);
        foreach (var section in creditsData.data) {
            var sectionTitle = Instantiate(prefab_SectionTitle, content.transform);
            sectionTitle.transform.GetChild(0).GetComponent<TMP_Text>().text = section.section;

            foreach (var entry in section.credits) {
                var block = Instantiate(prefab_CreditsBlock, content.transform);
                var texts = block.GetComponentsInChildren<TMP_Text>();
                if (texts.Length >= 2) {
                    texts[0].text = entry.role;
                    texts[1].text = string.Join("\n", entry.names);
                }
            }
        }

        Instantiate(terribleHackToAddPadding, content.transform);
    }

    private IEnumerator InitAndStartScroll() {
        // Force layout rebuild
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        // Reset to top
        scrollRect.verticalNormalizedPosition = 1f;

        // Wait another frame
        yield return null;

        readyToScroll = true;
    }

    private void Update() {
        if (readyToScroll && scrollRect.verticalNormalizedPosition > 0 && !isScrolling) {
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime / 100f;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        isScrolling = true;
    }

    public void OnEndDrag(PointerEventData eventData) {
        isScrolling = false;
    }
}