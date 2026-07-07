using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Univerzální komponenta pro HP. Používá ji Člověk i Komár.
/// Umožňuje brnění (armor), které snižuje příchozí poškození.
/// Přidej tuto komponentu na Player_Human i Player_Mosquito prefab.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Nastavení HP")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Brnění")]
    [Tooltip("Kolik plochého poškození brnění odečte z každého zásahu (min. poškození je vždy 1).")]
    [SerializeField] private float armorFlatReduction = 0f;
    [Tooltip("Procentuální snížení poškození (0 = žádné, 0.5 = poloviční poškození).")]
    [SerializeField] [Range(0f, 0.9f)] private float armorPercentReduction = 0f;

    [Header("Eventy (napoj na UI, animace, zvuky...)")]
    public UnityEvent<float, float> OnHealthChanged; // (current, max)
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamaged; // kolik reálně proteklo

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>Přidá armor (např. při sebrání helmy).</summary>
    public void AddArmor(float flatBonus, float percentBonus)
    {
        armorFlatReduction += flatBonus;
        armorPercentReduction = Mathf.Clamp(armorPercentReduction + percentBonus, 0f, 0.9f);
    }

    public void TakeDamage(float rawDamage)
    {
        if (IsDead || rawDamage <= 0f) return;

        float afterPercent = rawDamage * (1f - armorPercentReduction);
        float afterFlat = afterPercent - armorFlatReduction;
        float finalDamage = Mathf.Max(afterFlat, 1f); // vždy proteče aspoň 1 dmg

        currentHealth = Mathf.Max(currentHealth - finalDamage, 0f);
        OnDamaged?.Invoke(finalDamage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
