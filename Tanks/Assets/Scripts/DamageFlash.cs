// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash
{
    // Used this tutorial: https://www.youtube.com/watch?v=rq6yGh-piIU
    // Someone please refactor this later so that it's actually good

    [SerializeField] public float _flashTime = 0.5f;
    [SerializeField] public Color _flashColor;

    private Material[] materials;
    private Coroutine _damageFlashCorountine;
    private Coroutine invisFlickerCor;
    private Coroutine invisDamageCor;

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

    public void CallDissolve(MonoBehaviour mono, float dissolveTime, bool reverse = false)
    {
        mono.StartCoroutine(Dissolve(dissolveTime, reverse));
    }
    public void CallInvisFlicker(MonoBehaviour mono, float duration)
    {
        if (invisFlickerCor == null) {
            invisFlickerCor = mono.StartCoroutine(InvisFlicker(duration));
        }
    }
    public void CallInvisDamage(MonoBehaviour mono, float duration)
    {
        if (invisDamageCor == null) {
            invisDamageCor = mono.StartCoroutine(InvisDamage(duration));
        }
    }
    public void CallElectricity(MonoBehaviour mono, float duration)
    {
        mono.StartCoroutine(Electricity(duration));
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
        //Debug.Log("Damage coroutine activated");

        float elapsedTime = 0f;

        while (elapsedTime < _flashTime)
        {
            elapsedTime += Time.deltaTime;

            float currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime) / _flashTime);
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }
    private IEnumerator InvisDamage(float duration)
    {
        SetFloatUniform("_Electricity", 1f);
        SetFloatUniform("_Dissolve", 0.2f);
        yield return new WaitForSeconds(duration);
        SetFloatUniform("_Electricity", 0f);

        float elapsedTime = 0f;
        float reducedDissolveTime = 0.3f;

        while (elapsedTime < reducedDissolveTime - 0.02f)
        {
            elapsedTime += Time.deltaTime;

            float dissolveAmount = Mathf.Lerp(0.2f, 1f, (elapsedTime) / reducedDissolveTime);
            SetFloatUniform("_Dissolve", dissolveAmount);

            yield return null;
        }

        SetFloatUniform("_Dissolve", 1.0f);
        invisDamageCor = null;
    }

    private IEnumerator Dissolve(float dissolveTime, bool reverse)
    {
        float elapsedTime = 0f;
        float reducedDissolveTime = Mathf.Max(0.01f, dissolveTime - 0.1f);

        while (elapsedTime < reducedDissolveTime - 0.02f)
        {
            elapsedTime += Time.deltaTime * 1.2f;

            float dissolveAmount = Mathf.Lerp(0f, 1f, (elapsedTime) / reducedDissolveTime);
            if (reverse) {
                dissolveAmount = 1 - dissolveAmount;
            }
            SetFloatUniform("_Dissolve", dissolveAmount);

            yield return null;
        }
        if (reverse) {
            SetFloatUniform("_Dissolve", 0f);
        } else {
            SetFloatUniform("_Dissolve", 1.0f);
        }
    }
    private IEnumerator InvisFlicker(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float amount = Mathf.Lerp(0f, 1f, (elapsedTime) / (duration / 2));
            SetFloatUniform("_Outline", amount);
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float amount = Mathf.Lerp(1, 0, (elapsedTime) / (duration / 2));
            SetFloatUniform("_Outline", amount);
            yield return null;
        }

        SetFloatUniform("_Outline", 0);
        invisFlickerCor = null;
    }
    private IEnumerator Electricity(float duration)
    {
        SetFloatUniform("_Electricity", 1f);
        yield return new WaitForSeconds(duration);
        SetFloatUniform("_Electricity", 0f);
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
