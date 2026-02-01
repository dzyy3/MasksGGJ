using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [Header("UI")]
    public Image GameOverImage;

    private const float MAX_HEALTH = 1f;

    private void Start()
    {
        healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            Debug.Log("Hit by enemy");
            GameData.Health = Mathf.Clamp01(GameData.Health - 0.2f);
            healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
            if (GameData.Health <= 0.1f)
            {
                GameOverImage.gameObject.SetActive(true);
                GameData.Health = 1f;
            }
        }
        if (collision.gameObject.CompareTag("boss"))
        {
            Debug.Log("Hit by boss");
            // GameData.Health = Mathf.Clamp01(GameData.Health - 0.2f);
            // healthBar.UpdateHealthBar(MAX_HEALTH, GameData.Health);
            // if (GameData.Health <= 0.1f)
            // {
            //     GameOverImage.gameObject.SetActive(true);
            //     GameData.Health = 1f;
            // }
        }
    }
}