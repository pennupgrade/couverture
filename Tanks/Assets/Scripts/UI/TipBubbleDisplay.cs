// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipBubbleDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rect;
    [SerializeField] private RectTransform rectChild;
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private PlayerTipTrigger playerTipTrigger;
    [SerializeField] private Camera playerCamera;
    private bool tipActive;
    private Vector3 lastTipPosition;

    private void Start()
    {
        // assume only one camera and playerTipTrigger in scene
        playerCamera = Camera.main;
        playerTipTrigger = FindObjectOfType<PlayerTipTrigger>();

        canvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (tipActive && !playerTipTrigger.CheckTipActive())
        {
            LeanTween.alphaCanvas(canvasGroup, 0f, 0.2f).setIgnoreTimeScale(true);
            tipActive = false;
        }
        else if (!tipActive && playerTipTrigger.CheckTipActive())
        {
            LeanTween.alphaCanvas(canvasGroup, 1f, 0.25f).setIgnoreTimeScale(true);
            rectChild.localScale = new Vector3(0.5f, 1f, 1f);
            LeanTween.scale(rectChild, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack).setIgnoreTimeScale(true);
            tipText.text = playerTipTrigger.GetTipText();
            tipActive = true;
        }

        if (tipActive)
        {
            lastTipPosition = playerTipTrigger.GetTipWorldPosition();
        }

        if (tipActive || canvasGroup.alpha > 0)
        {
            Vector3 viewportPoint = playerCamera.WorldToViewportPoint(lastTipPosition);
            rect.anchoredPosition = new Vector3(1920f * viewportPoint.x, 1080f * viewportPoint.y, 0f);
        }
    }
}