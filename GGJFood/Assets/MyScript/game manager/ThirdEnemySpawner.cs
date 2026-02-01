using UnityEngine;

public class ThirdEnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject enemyPrefab;                    // enemy prefab to spawn

    [Header("Bounds Reference (X width + lane Z)")]
    public BoxCollider boundsBox;                     // bounds object collider (use for X + Z)

    [Header("Camera Reference (for visible Y)")]
    public Camera cam;                                // drag your active camera here
    [Range(0f, 1f)] public float minViewportY = 0.2f; // min Y inside screen
    [Range(0f, 1f)] public float maxViewportY = 0.8f; // max Y inside screen

    [Header("Spawn Timing")]
    public float spawnInterval = 2f;                  // how often to spawn

    [Header("Spawn Offset")]
    public float outsideX = 1.0f;                     // how far outside left/right to spawn

    [Header("Random Patrol + Speed")]
    public Vector2 patrolHalfRange = new Vector2(3f, 8f); // half distance A<->B
    public Vector2 speedRange = new Vector2(1.5f, 4f);    // random speed

    // ADDED: track spawning state so narration can block it
    private bool isSpawning = false;

    void Start()
    {
        if (cam == null) cam = Camera.main;           // fallback if not assigned

        // CHANGED:
        // We no longer auto-start spawning here.
        // GameFlowManager will call StartSpawning() after narration ends.
        //InvokeRepeating(nameof(SpawnEnemy), 0.5f, spawnInterval);
    }

    // ADDED: start spawning when gameplay begins
    public void StartSpawning()
    {
        if (isSpawning) return; // ADDED: prevents double InvokeRepeating calls

        isSpawning = true;
        InvokeRepeating(nameof(ThirdSpawnEnemy), 0.5f, spawnInterval);
    }

    // ADDED: stop spawning during narration / pause / stage end
    public void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(ThirdSpawnEnemy));
    }

    void ThirdSpawnEnemy()
    {
        // ADDED: extra safety guard
        if (!isSpawning) return;

        if (enemyPrefab == null) return;
        if (boundsBox == null) return;
        if (cam == null) return;

        // read world bounds from the box collider (for X width)
        Bounds b = boundsBox.bounds;

        float leftX  = b.min.x;                       // left edge of playable area
        float rightX = b.max.x;                       // right edge of playable area

        // use the bounds object's Z as the gameplay lane Z
        float laneZ = boundsBox.transform.position.z;

        // choose spawn side (left or right)
        bool spawnLeft = Random.value < 0.5f;

        // spawn X just outside the bounds
        float spawnX = spawnLeft ? (leftX - outsideX) : (rightX + outsideX);

        // pick a random viewport Y inside the screen
        float vy = Random.Range(minViewportY, maxViewportY);

        // convert viewport Y -> world Y on the laneZ plane (works in Perspective)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, vy, 0f)); // x=0.5 means center
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, laneZ));

        if (!plane.Raycast(ray, out float enter)) return;

        float spawnY = ray.GetPoint(enter).y;

        // final spawn world position (X from bounds, Y from camera-visible range, Z from bounds)
        Vector3 spawnWorld = new Vector3(spawnX, spawnY, laneZ);

        // spawn enemy
        GameObject enemyObj = Instantiate(enemyPrefab, spawnWorld, Quaternion.identity);

        // configure patrol + speed
        if (enemyObj.TryGetComponent<EnemyPatrol>(out var patrol))
        {
            float half = Random.Range(patrolHalfRange.x, patrolHalfRange.y); // random half-range
            float spd  = Random.Range(speedRange.x, speedRange.y);           // random speed

            // choose a patrol center X INSIDE bounds so A and B stay inside
            float centerX = Random.Range(leftX + half, rightX - half);

            // build patrol points at the same Y and Z as spawn
            Vector3 A = new Vector3(centerX - half, spawnY, laneZ);
            Vector3 B = new Vector3(centerX + half, spawnY, laneZ);

            // EnemyPatrol always goes to pointB first.
            // Make pointB be "inward" so the enemy enters the screen immediately.
            if (spawnLeft)
            {
                // from left -> go right first
                patrol.pointA = A;
                patrol.pointB = B;
            }
            else
            {
                // from right -> go left first (so pointB should be A)
                patrol.pointA = B;
                patrol.pointB = A;
            }

            patrol.moveSpeed = spd;
        }
    }

    // optional: visualize X bounds in Scene view
    void OnDrawGizmosSelected()
    {
        if (boundsBox == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boundsBox.bounds.center, boundsBox.bounds.size);
    }
}
