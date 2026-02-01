using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Runtime Patrol Data (set by spawner)")]
    public Vector3 pointA; // left point (set in code)
    public Vector3 pointB; // right point (set in code)
    public float moveSpeed = 3f; // speed (set in code)

    [Header("Tuning")]
    public float reachDistance = 0.05f; // how close to switch direction

    private Rigidbody rb; // rigidbody reference
    private Vector3 targetPoint; // current target point

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // cache rigidbody

        rb.constraints = RigidbodyConstraints.FreezeRotation; // stop tipping
    }

    void Start()
    {
        targetPoint = pointB; // always go to B first
        FaceTarget(targetPoint); // face correct direction at start
    }

    void FixedUpdate()
    {
        Vector3 pos = rb.position; // current position

        Vector3 toTarget = targetPoint - pos; // vector toward target

        // if close enough, switch target
        if (toTarget.magnitude <= reachDistance)
        {
            targetPoint = (targetPoint == pointB) ? pointA : pointB; // swap A <-> B
            FaceTarget(targetPoint); // rotate when turning around
            return;  // stop this frame
        }

        Vector3 dir = toTarget.normalized; // normalize direction

        Vector3 nextPos = pos + dir * moveSpeed * Time.fixedDeltaTime; // next step

        rb.MovePosition(nextPos);// move using rigidbody
    }

    private void FaceTarget(Vector3 target)
    {
        float dx = target.x - transform.position.x; // check if target is left/right

        if (Mathf.Abs(dx) < 0.001f) return;  

        transform.rotation = (dx > 0f)
            ? Quaternion.Euler(0f, -90f, 0f) // face right (adjust if needed)
            : Quaternion.Euler(0f, 90f, 0f); // face left
    }
}
