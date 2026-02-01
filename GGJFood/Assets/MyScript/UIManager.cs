using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject stageCompletePopup;

    private void Awake()
    {
        if (stageCompletePopup != null)
            stageCompletePopup.SetActive(false);
    }

    public void ShowStageComplete()
    {
        if (stageCompletePopup != null)
            stageCompletePopup.SetActive(true);
    }
}
