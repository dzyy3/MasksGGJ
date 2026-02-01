// using UnityEngine;
// using UnityEngine.UI;

// public class CharacterHealth : MonoBehaviour
// {
//     [SerializeField] private HealthBar healthBar;
//     [Header("UI")]
//     public Image GameOverImage;

//     private const float MAX_HEALTH = 1f;

//     private void Start()
//     {
//         healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("enemy"))
//         {
//             Debug.Log("Hit by enemy");
//             GameData.Health = Mathf.Clamp01(GameData.Health - 0.2f);
//             healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
//             if (GameData.Health <= 0.1f)
//             {
//                 GameOverImage.gameObject.SetActive(true);
//                 GameData.Health = 1f;
//             }
//         }

//         else if (collision.gameObject.CompareTag("boss"))
//         {
//             Debug.Log("Hit by boss");
//             GameData.Health = Mathf.Clamp01(GameData.Health - 0.4f);
//             healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
//             if (GameData.Health <= 0.1f)
//             {
//                 GameOverImage.gameObject.SetActive(true);
//                 GameData.Health = 1f;
//             }
//         }
//     }
// }

using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    [Header("UI")]
    public Image GameOverImage;

    [Header("Damage Settings")]
    [Tooltip("How many seconds must pass before taking damage again from the boss box.")]
    [SerializeField] private float bossHitCooldown = 0.35f;
    [Tooltip("Prevents double damage when both trigger + collision fire.")]
    [SerializeField] private float globalHitCooldown = 0.08f;

    private float lastAnyHitTime = -999f;


    private const float MAX_HEALTH = 1f;
    private float lastBossHitTime = -999f;

    private void Start()
    {
        healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
        if (GameOverImage != null) GameOverImage.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryApplyDamageFrom(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryApplyDamageFrom(other.gameObject);
    }


    private void TryApplyDamageFrom(GameObject other)
    {
        if (Time.time - lastAnyHitTime < globalHitCooldown) return;

        float damage = 0f;

        if (other.CompareTag("boss"))
        {
            if (Time.time - lastBossHitTime < bossHitCooldown) return;
            lastBossHitTime = Time.time;
            damage = MAX_HEALTH / 3f;
        }
        else if (other.CompareTag("enemy"))
        {
            damage = MAX_HEALTH * 0.2f;
        }
        else return;

        lastAnyHitTime = Time.time;

        GameData.Health = Mathf.Clamp01(GameData.Health - damage);
        healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);

        if (GameData.Health <= 0f)
        {
            if (GameOverImage != null) GameOverImage.gameObject.SetActive(true);
            GameData.Health = 1f;
            healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
        }
    }


}
