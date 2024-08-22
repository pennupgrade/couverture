using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private Tank tank;

    private Slider slider;

    private void Awake() {
        slider = gameObject.GetComponent<Slider>();
    }

    // Update is called once per frame
    private void Update() {
        slider.value = tank.health / maxHealth;
    }

    // TODO: animate slider going up and down?
}