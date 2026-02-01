using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject BossPrefab;
    public int TotalSpawns = 10;

    [Header("Spawn Settings")]
    public float spawnDelay = 2f;
    public float spawnInterval = 3f;

    [Header("Boss Settings")]
    public float bossSpawnDelay = 3f;

    private int spawnCount = 0;
    private bool bossSpawnScheduled = false;

    private bool isSpawning = false;

    private void Update()
    {
        if (!isSpawning) return;

        if (spawnCount >= TotalSpawns && !bossSpawnScheduled)
        {
            CancelInvoke(nameof(SpawnEnemy));
            bossSpawnScheduled = true;
            Invoke(nameof(SpawnBoss), bossSpawnDelay);
        }
    }

    public void BeginSpawning()
{
    if (isSpawning) return;

    if (enemyPrefab == null)
    {
        Debug.LogError("enemyPrefab is NOT assigned in EnemySpawner.");
        return;
    }

    Debug.Log("BeginSpawning() started InvokeRepeating");
    isSpawning = true;
    InvokeRepeating(nameof(SpawnEnemy), spawnDelay, spawnInterval);
}

private void SpawnEnemy()
{
    Debug.Log("SpawnEnemy fired");
    var e = Instantiate(enemyPrefab, transform.position, transform.rotation);
    e.tag = "enemy";
    SetLayerRecursively(e, LayerMask.NameToLayer("enemy"));
    spawnCount++;
}


    private void SpawnBoss()
    {
        var b = Instantiate(BossPrefab, transform.position, transform.rotation);
        b.tag = "boss";
        SetLayerRecursively(b, LayerMask.NameToLayer("boss"));
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
