using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePanelBehavior : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private GameObject achievementsButton;

    private void Start()
    {
        var sizeDeltaX = rt.sizeDelta.x;

#if !(DISABLESTEAMWORKS || UNITY_EDITOR)
        // Decrease vertical layout group's height because achievements button is missing
        rt.sizeDelta = new Vector2(sizeDeltaX, 420f);
        achievementsButton.SetActive(false);
#else
        rt.sizeDelta = new Vector2(sizeDeltaX, 530f);
        achievementsButton.SetActive(true);
#endif
    }
}
