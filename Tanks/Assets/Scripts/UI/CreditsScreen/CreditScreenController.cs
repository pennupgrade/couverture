using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

public class CreditScreenController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed;
    public GameObject prefab_CreditsBlock;
    public GameObject prefab_SectionTitle;
    public GameObject content;
    public TextAsset jsonFile;

    private bool readyToScroll = false;

    private void Awake()
    {
        GenerateCredits();
        StartCoroutine(InitAndStartScroll());
    }

    private void GenerateCredits()
    {
        content.SetActive(true); // Must be active before building
        GameObject temp1 = Instantiate(prefab_SectionTitle, content.transform);
        temp1.GetComponent<TMP_Text>().text = "  ";

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
                    texts[1].text = string.Join(", ", entry.names);
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
        if (readyToScroll && scrollRect.verticalNormalizedPosition > 0)
        {
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime / 100f;
        }
    }
}