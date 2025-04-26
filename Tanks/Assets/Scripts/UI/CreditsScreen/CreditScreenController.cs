using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CreditEntry
{
    public string role;
    public List<string> names;
}

[System.Serializable]
public class CreditSection
{
    public string section;
    public List<CreditEntry> credits;
}

[System.Serializable]
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

    private bool readyToScroll = false;

    private void Awake()
    {
        isScrolling = false;
        GenerateCredits();
        StartCoroutine(InitAndStartScroll());
        returnBtn.onClick.AddListener(returnToMain);
    }

    public void returnToMain()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    private void GenerateCredits()
    {
        content.SetActive(true); // Must be active before building

        for (int i = 0; i < 4; i++)
        {
            GameObject temp1 = Instantiate(prefab_SectionTitle, content.transform);
            temp1.GetComponent<TMP_Text>().text = "  ";
        }

        CreditsData creditsData = JsonUtility.FromJson<CreditsData>(jsonFile.text);
        foreach (CreditSection section in creditsData.data)
        {
            GameObject sectionTitle = Instantiate(prefab_SectionTitle, content.transform);
            sectionTitle.GetComponent<TMP_Text>().text = section.section;
            foreach (CreditEntry entry in section.credits)
            {
                GameObject block = Instantiate(prefab_CreditsBlock, content.transform);
                TMP_Text[] texts = block.GetComponentsInChildren<TMP_Text>();
                if (texts.Length >= 2)
                {
                    texts[0].text = entry.role;
                    texts[1].text = string.Join("\n", entry.names);
                }
            }
        }
    }

    

    private IEnumerator InitAndStartScroll()
    {
        // Force layout rebuild
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        // Reset to top
        scrollRect.verticalNormalizedPosition = 1f;

        // Wait another frame
        yield return null;

        readyToScroll = true;
    }

    private void Update()
    {
        if (readyToScroll && scrollRect.verticalNormalizedPosition > 0 && !isScrolling)
        {
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime / 100f;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isScrolling = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isScrolling = false;
    }
}