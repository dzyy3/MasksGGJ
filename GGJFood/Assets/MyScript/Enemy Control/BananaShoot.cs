using UnityEngine;

public class BananaShoot : MonoBehaviour
{
    [SerializeField] private Transform gun;

    [Header("Bullet")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed = 25f;
    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Auto Fire")]
    [Tooltip("Seconds between each shot.")]
    [SerializeField] private float fireInterval = 2f;

    [Header("Aim Offset")]
    [Tooltip("Degrees to tilt the shot downward.")]
    [SerializeField] private float downwardAngle = 10f;

    private float nextFireTime;

    private void OnEnable()
    {
        nextFireTime = Time.time + Mathf.Max(0.01f, fireInterval);
    }

    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            FireForward();
            nextFireTime = Time.time + Mathf.Max(0.01f, fireInterval);
        }
    }

    private void FireForward()
    {
        GameObject bulletInstance =
            Instantiate(bullet, bulletSpawnPoint.position, gun.rotation);

        Vector3 shootDir = gun.forward;

        shootDir = Quaternion.AngleAxis(downwardAngle, gun.right) * shootDir;
        shootDir.Normalize();

        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootDir * bulletSpeed;
        }
    }
}
