using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingWall : Activatable
{
    public Vector3 slideOffset;
    [SerializeField] private int activatesNeeded = 1;
    private Coroutine slideCor;
    // Start is called before the first frame update
    public override void activate() {
        if (slideCor != null) return;
        activatesNeeded--;
        if (activatesNeeded > 0) return;
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
            timer += Time.deltaTime * 1.5f;
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
