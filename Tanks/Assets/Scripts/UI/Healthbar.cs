using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Tank tank;
    [SerializeField] Slider slider;
    [SerializeField] Slider delaySlider;
    [SerializeField] float healthSmoothTime;
    [SerializeField] float healthDelayTime;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private float shakeAmount;

    [Header("Boss")]
    [SerializeField] private bool bossMode;
    [SerializeField] private TMP_Text bossTitle;
    [SerializeField] private Boss boss;
    
    private RectTransform rect;
    private Vector2 startPosition;
    private int maxHealth;
    private float currentHealth;
    private float currentDelayHealth;
    private float targetHealth;
    private float healthDelayTimer;
    private float healthSmoothVel;
    private float healthDelaySmoothVel;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startPosition = rect.anchoredPosition;
        if (bossMode)
            maxHealth = (int)boss.health;
        else
            maxHealth = tank.maxHealth;
    }
    
    private void Update()
    {
        float newHealth;
        if (bossMode)
            newHealth = (float)boss.health / maxHealth;
        else
            newHealth = (float)tank.health / maxHealth;
            
        if (newHealth < targetHealth)
        {
            healthDelayTimer = healthDelayTime;
            ShakeBar();
        }
        targetHealth = newHealth;
        
        healthDelayTimer = Mathf.Max(healthDelayTimer - Time.deltaTime, 0f);
        currentHealth = Mathf.SmoothDamp(currentHealth, targetHealth, ref healthSmoothVel, healthSmoothTime);
        if (healthDelayTimer == 0f)
            currentDelayHealth = Mathf.SmoothDamp(currentDelayHealth, targetHealth, ref healthDelaySmoothVel, healthSmoothTime);

        slider.value = currentHealth;
        delaySlider.value = currentDelayHealth;

        if (!bossMode)
        {
            int displayHealth = Mathf.Max(0, tank.health) / 100;
            healthText.text = $"{displayHealth}<size=\"14\">/{maxHealth / 100}</size>";
        }
    }

    private void ShakeBar()
    {
        LeanTween.cancel(gameObject);
        rect.anchoredPosition = startPosition;
        LeanTween.moveX(rect, startPosition.x - shakeAmount, 0.05f).setEaseInQuart().setIgnoreTimeScale(true);
        LeanTween.moveX(rect, startPosition.x + shakeAmount, 0.1f).setEaseInOutQuart().setIgnoreTimeScale(true).setDelay(0.05f);
        LeanTween.moveX(rect, startPosition.x, 0.05f).setEaseOutQuart().setIgnoreTimeScale(true).setDelay(0.15f);
    }
}