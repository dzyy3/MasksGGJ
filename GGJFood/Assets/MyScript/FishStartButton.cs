using UnityEngine;

public class FishStartButton : MonoBehaviour
{
    [SerializeField] private GameObject startUIPanel;
    [SerializeField] private ThirdEnemySpawner enemySpawner; // <-- match the component type

    public void StartGame()
    {
        Debug.Log("StartGame() clicked");

        if (startUIPanel != null)
            startUIPanel.SetActive(false);

        if (enemySpawner != null)
            enemySpawner.StartSpawning();  // <-- match the method name
        else
            Debug.LogError("enemySpawner is null (not assigned in Inspector)");
    }
}
