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

    private void Awake()
    {
        GameObject temp1 = Instantiate(prefab_SectionTitle, content.transform);
        temp1.GetComponent<TMP_Text>().text = "  ";


        CreditsData creditsData = JsonUtility.FromJson<CreditsData>(jsonFile.text);
        content.SetActive(false);
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
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        content.SetActive(true);
    }

    void Update()
    {
        if (scrollRect.verticalNormalizedPosition > 0)
        {
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime / 100f;
        }
    }
}
