using UnityEngine;
using System.Collections.Generic;

public class BananaSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public GameObject prefab;
        [Min(0)] public int count = 5;

        [Tooltip("Optional: override tag/layer per enemy type. Leave blank to use defaults below.")]
        public string tagOverride = "";
        public string layerOverride = "";
    }

    [Header("Enemies (set different counts per type)")]
    public List<EnemySpawnEntry> enemies = new List<EnemySpawnEntry>();

    [Header("Defaults (used if overrides are blank)")]
    public string defaultEnemyTag = "enemy";
    public string defaultEnemyLayer = "enemy";

    [Header("Boss")]
    public GameObject bossPrefab;
    public float bossSpawnDelay = 3f;
    public string bossTag = "boss";
    public string bossLayer = "boss";

    [Header("Spawn Timing")]
    public float spawnDelay = 2f;
    public float spawnInterval = 3f;

    [Header("Spawn Mode")]
    [Tooltip("If true: random selection weighted by remaining quota. If false: spawn in list order.")]
    public bool randomWeightedByRemaining = true;

    private int totalToSpawn = 0;
    private int spawnedSoFar = 0;
    private bool isSpawning = false;
    private bool bossSpawnScheduled = false;

    private void Awake()
    {
        RecalculateTotal();
    }

    private void Update()
    {
        if (!isSpawning) return;

        if (spawnedSoFar >= totalToSpawn && !bossSpawnScheduled)
        {
            CancelInvoke(nameof(SpawnEnemy));
            bossSpawnScheduled = true;
            Invoke(nameof(SpawnBoss), bossSpawnDelay);
        }
    }

    public void BeginSpawning()
    {
        if (isSpawning) return;

        // Validate
        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogError("EnemySpawner: No enemy entries set in 'enemies' list.");
            return;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].prefab == null && enemies[i].count > 0)
            {
                Debug.LogError($"EnemySpawner: enemies[{i}] prefab is null but count > 0.");
                return;
            }
        }

        RecalculateTotal();

        if (totalToSpawn <= 0)
        {
            Debug.LogWarning("EnemySpawner: totalToSpawn is 0. Spawning boss immediately.");
            bossSpawnScheduled = true;
            Invoke(nameof(SpawnBoss), bossSpawnDelay);
            return;
        }

        isSpawning = true;
        InvokeRepeating(nameof(SpawnEnemy), spawnDelay, spawnInterval);
    }

    private void RecalculateTotal()
    {
        totalToSpawn = 0;
        if (enemies == null) return;

        foreach (var entry in enemies)
            totalToSpawn += Mathf.Max(0, entry.count);
    }

    private void SpawnEnemy()
    {
        if (spawnedSoFar >= totalToSpawn) return;

        int index = randomWeightedByRemaining
            ? PickIndexWeightedByRemaining()
            : PickNextIndexInOrder();

        if (index < 0)
        {
            // Shouldn’t happen if totals are correct, but safe guard
            spawnedSoFar = totalToSpawn;
            return;
        }

        var entry = enemies[index];

        // Spawn
        GameObject e = Instantiate(entry.prefab, transform.position, transform.rotation);

        // Apply tag/layer (use overrides if provided)
        string tagToUse = string.IsNullOrWhiteSpace(entry.tagOverride) ? defaultEnemyTag : entry.tagOverride;
        string layerToUse = string.IsNullOrWhiteSpace(entry.layerOverride) ? defaultEnemyLayer : entry.layerOverride;

        if (!string.IsNullOrEmpty(tagToUse))
            e.tag = tagToUse;

        int layerId = LayerMask.NameToLayer(layerToUse);
        if (layerId != -1)
            SetLayerRecursively(e, layerId);

        // Consume quota
        entry.count -= 1;
        spawnedSoFar += 1;
    }

    private int PickNextIndexInOrder()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].count > 0 && enemies[i].prefab != null)
                return i;
        }
        return -1;
    }

    private int PickIndexWeightedByRemaining()
    {
        int remaining = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].prefab != null)
                remaining += Mathf.Max(0, enemies[i].count);
        }

        if (remaining <= 0) return -1;

        int roll = Random.Range(0, remaining); // 0..remaining-1
        int cumulative = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            int c = (enemies[i].prefab != null) ? Mathf.Max(0, enemies[i].count) : 0;
            cumulative += c;
            if (roll < cumulative)
                return i;
        }

        return -1;
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: bossPrefab is null, skipping boss spawn.");
            return;
        }

        var b = Instantiate(bossPrefab, transform.position, transform.rotation);

        if (!string.IsNullOrEmpty(bossTag))
            b.tag = bossTag;

        int layerId = LayerMask.NameToLayer(bossLayer);
        if (layerId != -1)
            SetLayerRecursively(b, layerId);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
