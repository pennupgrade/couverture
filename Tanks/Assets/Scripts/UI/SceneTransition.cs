using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition I;
    
    [SerializeField] private Image overlay;
    
    private Material mat;

    private const float Init = 0f;
    private const float Final = 4.5f;
    
    private static readonly int SizeId = Shader.PropertyToID("_Size");
    private static readonly int PositionXId = Shader.PropertyToID("_Position_X");
    private static readonly int PositionYId = Shader.PropertyToID("_Position_Y");

    private void Awake() {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        mat = overlay.material;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start() => Disappear();

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        UpdatePosition();
        Disappear();
    }

    public void UpdatePosition() {
        var tankObj = GameObject.FindGameObjectWithTag("Player");

        if (tankObj != null) {
            var viewportPos = Camera.main!.WorldToViewportPoint(tankObj.transform.position);
            mat.SetFloat(PositionXId, Mathf.Clamp01(viewportPos.x));
            mat.SetFloat(PositionYId, Mathf.Clamp01(viewportPos.y));
        }
    }
    
    public bool IsAnimating => LeanTween.isTweening(overlay.gameObject);

    public void Appear() => LeanTween.value(overlay.gameObject, value => {
        mat.SetFloat(SizeId, value);
    }, Final, Init, 2f).setEaseInOutExpo();
    
    public void Disappear() =>LeanTween.value(overlay.gameObject, value => {
        mat.SetFloat(SizeId, value);
    }, Init, Final, 2f).setEaseInOutExpo();
}
