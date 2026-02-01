using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonClick: MonoBehaviour
{
    [SerializeField] private string targetSceneName;

     public void NewSceneOpen()
    {
        GameManager.Instance.MoveScene(targetSceneName);
    }
}