using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Tank tank;
    
    private float maxHealth;
    private Slider slider;

    private void Awake() {
        slider = gameObject.GetComponent<Slider>();
        maxHealth = tank.health;
    }

    // Update is called once per frame
    private void Update() {
        slider.value = tank.health / maxHealth;
    }

    // TODO: animate slider going up and down?
}