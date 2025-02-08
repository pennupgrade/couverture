using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingWallToggleState : Activatable
{
    public Vector3 slideOffset;
    [SerializeField] private float speed = 1.5f;
    private Coroutine slideCor;
    public bool startActive;

    // Start is called before the first frame update
    public override void activate() {
        if (slideCor != null) return;
        if (!activated) {
            slideCor = StartCoroutine(slideWall(transform.position + slideOffset));
        } else {
            slideCor = StartCoroutine(slideWall(transform.position - slideOffset));
        }
        activated = !activated;
    }

    private IEnumerator slideWall(Vector3 dest) {
        float timer = 0;
        Vector3 startPos = transform.position;
        while (timer <= 1) {
            transform.position = Vector3.Lerp(startPos, dest, timer);
            timer += Time.deltaTime * speed;
            yield return null;
        }
        transform.position = dest;
        slideCor = null;
    }
}