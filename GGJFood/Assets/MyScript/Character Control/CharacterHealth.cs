using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    private const float MAX_HEALTH = 1f;

    private void Start()
    {
        healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            GameData.Health = Mathf.Clamp01(GameData.Health - 0.1f);
            healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
        }
    }
}
