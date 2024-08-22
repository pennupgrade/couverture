using System.Collections.Generic;
using UnityEngine;

public class BulletCount : MonoBehaviour
{
    [SerializeField] private Tank tank;
    [SerializeField] private List<BulletIcon> bulletIcons;

    private int prevCount;

    private void Update() {
        var count = tank.numBullets;

        if (prevCount == count) return;

        for (var i = 0; i < count; i++) bulletIcons[i].fadeTo(Color.white);
        for (var i = count; i < 5; i++) bulletIcons[i].fadeTo(Color.clear);

        prevCount = count;
    }
}