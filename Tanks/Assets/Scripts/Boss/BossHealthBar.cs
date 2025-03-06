using UnityEngine;
using UnityEngine.UI;

public class BossHealthbar : MonoBehaviour
{
    [SerializeField] private Boss boss;

    private float maxHealth;
    private Slider slider;

    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        maxHealth = boss.health;
    }

    // Update is called once per frame
    private void Update()
    {
        if (boss == null)
            gameObject.SetActive(false);
        else
            slider.value = boss.health / maxHealth;
    }
}