using TMPro;
using UnityEngine;

public class BulletCount : MonoBehaviour
{
    [SerializeField] private Tank tank;
    [SerializeField] private TMP_Text bullets;

    private void Update() {
        bullets.text = $"Bullets: <b>{tank.numBullets}</b>";
    }
}