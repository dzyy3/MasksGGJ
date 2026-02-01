using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float normalBulletSpeed = 15f;
    [SerializeField] private float bulletLifetime = 3f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        SetDestroyTime();//destroy after certain seconds

        SetStraightVelocity();
    }

    private void SetStraightVelocity()
    {
        rb.linearVelocity = transform.right * normalBulletSpeed;
    }
    private void SetDestroyTime()
    {
        Destroy(gameObject, bulletLifetime);
    }
}
