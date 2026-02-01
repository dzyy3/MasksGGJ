using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StartDollyOnButton : MonoBehaviour
{
    [Header("Dolly")]
    [SerializeField] private Animator dollyAnimator;
    [SerializeField] private string stateName = "first camera";
    [SerializeField] private AnimationClip dollyClip;

    [Header("UI")]
    [SerializeField] private CanvasGroup firstUI;
    [SerializeField] private CanvasGroup secondUI;
    [SerializeField] private float fadeOutTime = 0.6f;
    [SerializeField] private float fadeInTime = 0.8f;

    [Header("Typing")]
    [SerializeField] private UIScript secondUIText;
    [SerializeField] private float typingDelayAfterVisible = 2f;

    [Header("Handoff (your scene/story manager listens to this)")]
    public UnityEvent onIntroFinished; // assign your manager method here in Inspector

    private void Awake()
    {
        if (dollyAnimator != null) dollyAnimator.enabled = false;

        if (secondUI != null)
        {
            secondUI.gameObject.SetActive(true);
            secondUI.alpha = 0f;
            secondUI.interactable = false;
            secondUI.blocksRaycasts = false;
        }
    }

    public void StartGame()
    {
        if (dollyAnimator != null)
        {
            dollyAnimator.enabled = true;
            dollyAnimator.Play(stateName, 0, 0f);
            dollyAnimator.Update(0f);
        }

        StartCoroutine(RunFlow());
    }

    private IEnumerator RunFlow()
    {
        if (firstUI != null)
            yield return Fade(firstUI, firstUI.alpha, 0f, fadeOutTime);

        if (firstUI != null) firstUI.gameObject.SetActive(false);

        if (dollyClip != null)
            yield return new WaitForSecondsRealtime(dollyClip.length);

        // show second UI
        if (secondUI != null)
        {
            secondUI.gameObject.SetActive(true);
            yield return Fade(secondUI, 0f, 1f, fadeInTime);

            secondUI.interactable = true;
            secondUI.blocksRaycasts = true;

            if (secondUIText != null)
                secondUIText.StartTyping(typingDelayAfterVisible);
        }

        // ADDED COMMENT:
        // This event should NOT load the next scene.
        // Use it only for things like switching to the intro vcam, enabling click input, etc.
        // The scene switch happens later when TypewriterText fires onLastLineFinished.
        // onIntroFinished?.Invoke();
    }

    private IEnumerator Fade(CanvasGroup cg, float from, float to, float time)
    {
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        cg.alpha = to;
    }
}
