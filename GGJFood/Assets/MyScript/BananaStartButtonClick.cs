using UnityEngine;

public class BananaStartButtonClick : MonoBehaviour
{
    [SerializeField] private GameObject startUIPanel;
    [SerializeField] private BananaSpawner enemySpawner;

    public void StartGame()
    {
        Debug.Log("StartGame() clicked");

        if (startUIPanel != null)
        {
            startUIPanel.SetActive(false);
            Debug.Log("UI panel disabled");
        }
        else Debug.LogWarning("startUIPanel is null");

        if (enemySpawner != null)
        {
            Debug.Log("Calling BeginSpawning()");
            enemySpawner.BeginSpawning();
        }
        else Debug.LogError("enemySpawner is null (not assigned in Inspector)");
    }
}
