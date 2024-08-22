using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private Tank tank;
    [SerializeField] private TMP_Text percentage;

    private Slider slider;

    private void Awake() {
        slider = gameObject.GetComponent<Slider>();
    }

    // Update is called once per frame
    private void Update() {
        var value = tank.health / maxHealth;

        slider.value = value;
        percentage.text = $"{Math.Round(value * 100)}%";
    }

    // TODO: animate slider going up and down?
}