using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Damage Over Time")]
    public float damagePerTick = 0.01f; // 1%
    public float tickInterval = 0.5f;   // half second

    private float nextDamageTime = 0f;

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (Time.time >= nextDamageTime)
        {
            GameData.Health = Mathf.Clamp01(GameData.Health - damagePerTick);
            nextDamageTime = Time.time + tickInterval;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Optional: reset so it doesn't instantly tick on re-contact
        nextDamageTime = 0f;
    }
}
