using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingWall : Activatable
{
    public Vector3 slideOffset;
    private Coroutine slideCor;
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
        while (timer <= 1) {
            transform.position = Vector3.Lerp(transform.position, dest, timer);
            timer += Time.deltaTime * 2;
            yield return null;
        }
        transform.position = dest;
        slideCor = null;
    }
}

public abstract class Activatable : MonoBehaviour{
    protected bool activated;
    public abstract void activate();
}
