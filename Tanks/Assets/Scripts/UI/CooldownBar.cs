using UnityEngine;
using UnityEngine.UI;

public class CooldownBar : MonoBehaviour
{
    [SerializeField] private Tank tank;
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;

    // Update is called once per frame
    private void Update() {
        slider.value = tank.cooldownProgress / Tank.COOLDOWN_TIME;
        fill.color = slider.value < 1f ? Color.white : Color.green;
    }
}