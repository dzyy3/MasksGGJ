using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimAndShoot : MonoBehaviour
{
    [SerializeField] private Transform gun;

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed = 25f;

    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Aim Tuning")]
    [SerializeField] private float gunAngleOffset = 0f;

    private GameObject bulletInstance;

    private Vector3 worldPosition;
    private Vector3 direction;
    private float angle;
    private Vector3 gunBaseScale;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        gunBaseScale = gun.localScale;
    }

    private void Update()
    {
        HandleGunRotation();
        HandleGunShooting();
    }

    private void HandleGunRotation()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mouseScreen);

        Plane plane = new Plane(Vector3.forward, gun.position);

        if (!plane.Raycast(ray, out float hitDist)) return;

        worldPosition = ray.GetPoint(hitDist);

        direction = worldPosition - gun.position;
        direction.z = 0f;

        if (direction.sqrMagnitude < 0.0001f) return;

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(-(angle + gunAngleOffset), 90f, 0f);


        Vector3 localScale = gunBaseScale;
        if (angle > 90f || angle < -90f)
            localScale.y = -Mathf.Abs(gunBaseScale.y);
        else
            localScale.y = Mathf.Abs(gunBaseScale.y);
        gun.localScale = localScale;
    }

    private void HandleGunShooting()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            bulletInstance = Instantiate(bullet, bulletSpawnPoint.position, gun.transform.rotation);

            Vector3 shootDir = worldPosition - bulletSpawnPoint.position;
            shootDir.z = 0f;

            if (shootDir.sqrMagnitude < 0.0001f) return;

            shootDir.Normalize();

            Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = shootDir * bulletSpeed;
            }
        }
    }

}
