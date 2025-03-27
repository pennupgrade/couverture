using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletBar : MonoBehaviour
{
    [SerializeField] private Tank tank;
    [SerializeField] private RectTransform[] bullets;
    [SerializeField] private TMP_Text bulletText;
    
    private RectTransform rect;
    private Vector2 startPosition;
    private int maxBulletCount;
    private int bulletCount;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startPosition = rect.anchoredPosition;
        maxBulletCount = tank.numBullets;
    }

    private void Update()
    {
        if (tank.numBullets < bulletCount)
        {
            AnimateFire();
            for (int i = 0; i < 5; i++)
            {
                bullets[i].gameObject.SetActive(i + 1 <= tank.numBullets);
            }
        }
        else if (tank.numBullets > bulletCount)
        {
            for (int i = 0; i < 5; i++)
            {
                bullets[i].gameObject.SetActive(i + 1 <= tank.numBullets);
            }
        }
        bulletCount = tank.numBullets;
        bulletText.text = $"{tank.numBullets}<size=\"14\">/{maxBulletCount}</size>";
    }

    private void AnimateFire()
    {
        LeanTween.cancel(gameObject);
        rect.anchoredPosition = startPosition;
        LeanTween.moveY(rect, startPosition.y - 20f, 0.07f).setEaseInQuart().setIgnoreTimeScale(true);
        LeanTween.moveY(rect, startPosition.y, 0.08f).setEaseOutQuart().setIgnoreTimeScale(true).setDelay(0.05f);
    }
}
