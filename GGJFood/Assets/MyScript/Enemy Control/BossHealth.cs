// using UnityEngine;
// using UnityEngine.UI;

// public class BossHealth : MonoBehaviour
// {
//     [Header("UI")]
//     private Image stageCompleteImage;
//     private bool dead;

//     [SerializeField] private HealthBar healthBar;

//     [Header("Health (per enemy instance)")]
//     [SerializeField] private float maxHealth = 1f;

//     private float currentHealth;

//     private void Awake()
//     {
//         if (healthBar == null)
//             healthBar = GetComponentInChildren<HealthBar>();
//             GameObject obj = GameObject.Find("Stage Complete popup");

//         if (obj != null)
//             stageCompleteImage = obj.GetComponent<Image>();

//         if (stageCompleteImage != null)
//             stageCompleteImage.gameObject.SetActive(false);
//     }

//     private void Start()
//     {
//         currentHealth = maxHealth;

//         if (healthBar != null)
//             healthBar.UpdateHealthBar(maxHealth, currentHealth);
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         if (!collision.gameObject.CompareTag("bullet")) return;

//         TakeDamage(maxHealth / 10f);

//         Destroy(collision.gameObject);
//     }

//     private void TakeDamage(float amount)
//     {
//         currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);

//         if (healthBar != null)
//             healthBar.UpdateHealthBar(maxHealth, currentHealth);

//         if (currentHealth <= 0f)
//         {
//             Destroy(gameObject);
//             if (stageCompleteImage != null)
//             stageCompleteImage.gameObject.SetActive(true);

//         }
//     }

//     private void Die()
//     {
//         if (dead) return;
//         dead = true;

//         if (stageCompleteImage != null)
//             stageCompleteImage.gameObject.SetActive(true);

//         Destroy(gameObject);
//     }
// }

using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("UI")]
    private UIManager ui;
    private bool dead;

    [SerializeField] private HealthBar healthBar;

    [Header("Health (per enemy instance)")]
    [SerializeField] private float maxHealth = 1f;

    private float currentHealth;

    private void Awake()
    {
        if (healthBar == null)
            healthBar = GetComponentInChildren<HealthBar>();

        ui = FindFirstObjectByType<UIManager>(); 
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

        TakeDamage(maxHealth / 10f);
        Destroy(collision.gameObject);
    }

    private void TakeDamage(float amount)
    {
        if (dead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);

        if (healthBar != null)
            healthBar.UpdateHealthBar(maxHealth, currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (dead) return;
        dead = true;

        if (ui != null)
            ui.ShowStageComplete();

        Destroy(gameObject);
    }
}
