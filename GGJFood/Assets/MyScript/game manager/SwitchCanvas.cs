using System.Collections;
using UnityEngine;

public class SwitchCanvas : MonoBehaviour
{
    [Header("Second Canvas")]
    [SerializeField] private CanvasGroup secondUI;
    [SerializeField] private float fadeInTime = 0.8f;

    private void Awake()
    {
        // Second canvas starts hidden
        if (secondUI != null)
        {
            secondUI.gameObject.SetActive(true);
            secondUI.alpha = 0f;
            secondUI.interactable = false;
            secondUI.blocksRaycasts = false;
        }
    }

    // 🔹 Hook this to FirstStageNarration.onNarrationFinished
    public void ShowSecondCanvas()
    {
        StartCoroutine(FadeInRoutine());
        Debug.Log("ShowSecondCanvas CALLED");

    }

    private IEnumerator FadeInRoutine()
    {
        if (secondUI == null) yield break;

        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            secondUI.alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            yield return null;
        }

        secondUI.alpha = 1f;
        secondUI.interactable = true;
        secondUI.blocksRaycasts = true;
    }
}
