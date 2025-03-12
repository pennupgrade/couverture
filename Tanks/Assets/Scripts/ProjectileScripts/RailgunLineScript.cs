using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunLineScript : MonoBehaviour
{
    public float dist;
    public Vector3 startPos, dir;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(beamLineRenderer());
    }
    private IEnumerator beamLineRenderer() {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.enabled = true;
        lr.SetPosition(0, startPos - 0.1f * dir);
        lr.SetPosition(1, startPos + dist * dir);
        float fadeOutSpeed = 0;
        while (fadeOutSpeed < 1) {
            fadeOutSpeed += Time.deltaTime;
            float m_color = Mathf.Lerp(1, 0, fadeOutSpeed);
            lr.materials[0].SetFloat("_Transparency", m_color);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
