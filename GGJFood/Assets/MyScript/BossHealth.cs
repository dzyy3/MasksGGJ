using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private HealthBar healthBar;

    [Header("Health (per enemy instance)")]
    [SerializeField] private float maxHealth = 1f;

    private float currentHealth;

    private void Awake()
    {
        if (healthBar == null)
            healthBar = GetComponentInChildren<HealthBar>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.UpdateHealthBar(maxHealth, currentHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("bullet")) return;

        TakeDamage(maxHealth / 0.1f);

        Destroy(collision.gameObject);
    }

    private void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);

        if (healthBar != null)
            healthBar.UpdateHealthBar(maxHealth, currentHealth);

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
