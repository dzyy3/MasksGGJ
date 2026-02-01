using UnityEngine;

public class ThirdEnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject enemyPrefab;                    // enemy prefab to spawn
    public GameObject bossPrefab;                     // BOSS prefab to spawn

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

    [Header("Wave Limit + Boss")]
    public int maxEnemies = 10;
    public float bossSpawnDelay = 3f;

    private bool isSpawning = false;

    private int spawnedCount = 0;
    private bool bossScheduled = false;

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    public void StartSpawning()
    {
        if (isSpawning) return;

        // reset wave state when starting
        spawnedCount = 0;
        bossScheduled = false;

        isSpawning = true;
        InvokeRepeating(nameof(ThirdSpawnEnemy), 0.5f, spawnInterval);
    }

    public void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(ThirdSpawnEnemy));
        CancelInvoke(nameof(SpawnBoss));
    }

    void ThirdSpawnEnemy()
    {
        if (!isSpawning) return;

        // stop once we hit the limit
        if (spawnedCount >= maxEnemies)
        {
            CancelInvoke(nameof(ThirdSpawnEnemy));
            isSpawning = false;

            if (!bossScheduled)
            {
                bossScheduled = true;
                Invoke(nameof(SpawnBoss), bossSpawnDelay);
            }
            return;
        }

        if (enemyPrefab == null) return;
        if (boundsBox == null) return;
        if (cam == null) return;

        Bounds b = boundsBox.bounds;
        float leftX = b.min.x;
        float rightX = b.max.x;
        float laneZ = boundsBox.transform.position.z;

        bool spawnLeft = Random.value < 0.5f;
        float spawnX = spawnLeft ? (leftX - outsideX) : (rightX + outsideX);

        float vy = Random.Range(minViewportY, maxViewportY);

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, vy, 0f));
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, laneZ));
        if (!plane.Raycast(ray, out float enter)) return;

        float spawnY = ray.GetPoint(enter).y;
        Vector3 spawnWorld = new Vector3(spawnX, spawnY, laneZ);

        GameObject enemyObj = Instantiate(enemyPrefab, spawnWorld, Quaternion.identity);
        spawnedCount++;

        if (enemyObj.TryGetComponent<EnemyPatrol>(out var patrol))
        {
            float half = Random.Range(patrolHalfRange.x, patrolHalfRange.y);
            float spd = Random.Range(speedRange.x, speedRange.y);

            float centerX = Random.Range(leftX + half, rightX - half);

            Vector3 A = new Vector3(centerX - half, spawnY, laneZ);
            Vector3 B = new Vector3(centerX + half, spawnY, laneZ);

            if (spawnLeft)
            {
                patrol.pointA = A;
                patrol.pointB = B;
            }
            else
            {
                patrol.pointA = B;
                patrol.pointB = A;
            }

            patrol.moveSpeed = spd;
        }
    }

    void SpawnBoss()
    {
        if (bossPrefab == null) return;
        if (boundsBox == null) return;
        if (cam == null) return;

        // Spawn boss at center X, random visible Y, same laneZ
        Bounds b = boundsBox.bounds;
        float laneZ = boundsBox.transform.position.z;
        float centerX = (b.min.x + b.max.x) * 0.5f;

        float vy = Random.Range(minViewportY, maxViewportY);
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, vy, 0f));
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, laneZ));
        if (!plane.Raycast(ray, out float enter)) return;

        float spawnY = ray.GetPoint(enter).y;
        Vector3 bossPos = new Vector3(centerX, spawnY, laneZ);

        Instantiate(bossPrefab, bossPos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        if (boundsBox == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boundsBox.bounds.center, boundsBox.bounds.size);
    }
}
