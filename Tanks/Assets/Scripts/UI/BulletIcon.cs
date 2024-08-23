using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BulletIcon : MonoBehaviour
{
    private const float ANIM_TIME = 0.12f;

    [SerializeField] private Image filled;

    private Coroutine fadeCoroutine;

    public void fadeTo(Color color) {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        StartCoroutine(animFade(color));
    }

    private IEnumerator animFade(Color color) {
        if (filled.color == color) yield break;

        var initial = filled.color;
        var progress = 0f;

        while (progress <= ANIM_TIME) {
            filled.color = Color.Lerp(initial, color, progress / ANIM_TIME);
            progress += Time.deltaTime;

            yield return null;
        }

        filled.color = color;
    }
}