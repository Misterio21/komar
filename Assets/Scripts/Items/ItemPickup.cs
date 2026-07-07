using UnityEngine;

/// <summary>
/// Sbíratelný item po mapě (helma, vesta, léčivo...). Sbírá pouze Human.
/// Umísti na GameObject itemu s Collider nastaveným na "Is Trigger".
/// </summary>
[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    public enum ItemType { ArmorFlat, ArmorPercent, Heal }

    [Header("Typ itemu")]
    [SerializeField] private ItemType itemType = ItemType.ArmorFlat;

    [Tooltip("Pro ArmorFlat: kolik plochého poškození to sníží. Pro Heal: kolik HP doplní.")]
    [SerializeField] private float value = 5f;

    [Tooltip("Pro ArmorPercent: 0.1 = sníží poškození o 10 %.")]
    [SerializeField] [Range(0f, 0.9f)] private float percentValue = 0.1f;

    [Header("Respawn (volitelné)")]
    [SerializeField] private bool respawns = true;
    [SerializeField] private float respawnTime = 20f;

    private Collider col;
    private MeshRenderer[] renderers;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Human")) return;

        Health humanHealth = other.GetComponentInParent<Health>();
        if (humanHealth == null) return;

        switch (itemType)
        {
            case ItemType.ArmorFlat:
                humanHealth.AddArmor(value, 0f);
                break;
            case ItemType.ArmorPercent:
                humanHealth.AddArmor(0f, percentValue);
                break;
            case ItemType.Heal:
                humanHealth.Heal(value);
                break;
        }

        if (respawns)
        {
            SetVisible(false);
            col.enabled = false;
            Invoke(nameof(Respawn), respawnTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Respawn()
    {
        SetVisible(true);
        col.enabled = true;
    }

    private void SetVisible(bool visible)
    {
        foreach (var r in renderers) r.enabled = visible;
    }
}
