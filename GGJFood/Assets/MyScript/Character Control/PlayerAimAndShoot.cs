using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimAndShoot : MonoBehaviour
{
    [SerializeField] private Transform gun;

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;
    
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
       // rotate the gun towards the mouse position
       Vector2 mouseScreen = Mouse.current.position.ReadValue();
       Ray ray = cam.ScreenPointToRay(mouseScreen);


       // plane at the gun position, perpendicular to z
       Plane plane = new Plane(Vector3.forward, gun.position);


       if (!plane.Raycast(ray, out float hitDist)) return;


       worldPosition = ray.GetPoint(hitDist);




       direction = worldPosition - gun.position;
       direction.z = 0f;


       if (direction.sqrMagnitude < 0.0001f) return;


       //  rotate on z only
       angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
       gun.rotation = Quaternion.Euler(0f, 0f, angle);


       // flip  when it reaches a 90 degree threshold
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
        }
    }
}