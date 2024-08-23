using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash // DamageFlash is a terrible name
{
    // Used this tutorial: https://www.youtube.com/watch?v=rq6yGh-piIU
    // Someone please refactor this later so that it's actually good

    [SerializeField] private float _flashTime = 0.5f;
    [SerializeField] private Color _flashColor;

    private Material[] materials;
    private Coroutine _damageFlashCorountine;

    public DamageFlash()
    {
        Init();
    }

    public DamageFlash(GameObject root)
    {
        Init();
        AddMaterialFromGameObject(root);
    }

    private void Init()
    {
        materials = new Material[0];
    }

    public void CallDamageFlash(MonoBehaviour mono)
    {
        _damageFlashCorountine = mono.StartCoroutine(DamageFlasher());
    }

    public void CallDissolve(MonoBehaviour mono, float dissolveTime)
    {
        mono.StartCoroutine(Dissolve(dissolveTime));
    }

    public void AddMaterialFromGameObject(GameObject gameObject)
    {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        materials = new Material[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            materials[i] = meshRenderers[i].material;
        }
    }

    private IEnumerator DamageFlasher()
    {
        //SetFlashColor();
        Debug.Log("Damage coroutine activated");

        float elapsedTime = 0f;

        while (elapsedTime < _flashTime)
        {
            elapsedTime += Time.deltaTime;

            float currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime) / _flashTime);
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private IEnumerator Dissolve(float dissolveTime)
    {
        float elapsedTime = 0f;
        float reducedDissolveTime = Mathf.Max(0.01f, dissolveTime - 0.1f);

        while (elapsedTime < reducedDissolveTime - 0.02f)
        {
            elapsedTime += Time.deltaTime * 1.2f;

            float dissolveAmount = Mathf.Lerp(0f, 1f, (elapsedTime) / reducedDissolveTime);
            SetFloatUniform("_Dissolve", dissolveAmount);

            yield return null;
        }

        SetFloatUniform("_Dissolve", 1.0f);
    }
    

    private void SetFloatUniform(string uniform, float amount)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat(uniform, amount); // _FlashAmount uniform determined in ShaderGraph
        }
    }

    private void SetFlashAmount(float amount)
    {
        SetFloatUniform("_FlashAmount", amount);
    }
}
