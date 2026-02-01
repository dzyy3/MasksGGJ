using UnityEngine;

public class BananaStartButtonClick : MonoBehaviour
{
    [SerializeField] private GameObject startUIPanel;
    [SerializeField] private BananaSpawner enemySpawner;

    public void StartGame()
    {
        if (startUIPanel != null)
            startUIPanel.SetActive(false);

        if (enemySpawner != null)
            enemySpawner.BeginSpawning(); // <-- match BananaSpawner's method
        else
            Debug.LogError("enemySpawner is null");
    }
}
