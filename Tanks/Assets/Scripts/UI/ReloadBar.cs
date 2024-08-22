using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    [SerializeField] private Tank tank;
    [SerializeField] private Slider slider;

    // Update is called once per frame
    private void Update() {
        slider.value = tank.reloadProgress / Tank.RELOAD_TIME;
    }
}