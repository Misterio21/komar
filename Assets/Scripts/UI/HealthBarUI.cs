using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Jednoduchý health bar. Umísti na Canvas UI Slider, přiřaď Health komponentu
/// hráče (spawnutého runtime - viz metoda BindTo, kterou zavolá PlayerSpawner
/// nebo si to propoj ručně přes UnityEvent OnHealthChanged v Inspectoru).
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void BindTo(Health health)
    {
        if (health == null) return;
        slider.maxValue = health.MaxHealth;
        slider.value = health.CurrentHealth;
        health.OnHealthChanged.AddListener(UpdateBar);
    }

    private void UpdateBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
