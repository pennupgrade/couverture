using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastFadeOut : MonoBehaviour
{
    // Start is called before the first frame update
    private float spawnTime;
    public float dur;
    public float fadeOutTime;
    private bool notStarted;
    void Start()
    {
        spawnTime = Time.time;
        notStarted = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > spawnTime + dur && notStarted) {
            notStarted = false;
            List<Material> materials = new List<Material>();
            this.GetComponent<MeshRenderer>().GetMaterials(materials);
            StartCoroutine(FadeOut(materials[0]));
        }
        if (Time.time > spawnTime + dur + fadeOutTime) {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator FadeOut(Material material)
    {
        if (!material.HasProperty("_Color"))
        {
            Debug.LogError("Material does not support color modifications!");
            yield break;
        }

        Color startColor = material.color;
        float startAlpha = startColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutTime);
            material.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        material.color = new Color(startColor.r, startColor.g, startColor.b, 0f); // Ensure it's fully transparent
    }
}
