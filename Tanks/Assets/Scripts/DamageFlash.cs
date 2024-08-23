using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash
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

    private void SetFlashColor()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_FlashColor", _flashColor); // _FlashColor uniform determined in ShaderGraph
        }
    }

    private void SetFlashAmount(float amount)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_FlashAmount", amount); // _FlashAmount uniform determined in ShaderGraph
        }
    }

}
