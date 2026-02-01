using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events; // ADDED

public class FirstStageNarration : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;

    [Header("Narration")]
    [TextArea] [SerializeField] private string[]  narration;
    [SerializeField] private int narrationIndex = 0;

    [Header("Typing")]
    [SerializeField] private float charDelay = 0.03f;
    [SerializeField] private float startDelay = 0.2f;

    [Header("Canvas Fade Out")]
    [SerializeField] private CanvasGroup uiGroup;
    [SerializeField] private float fadeOutTime = 0.6f;
    [SerializeField] private bool disableAfterFade = true;

    // ADDED: manager hook
    [Header("Handoff (GameFlowManager listens to this)")]
    public UnityEvent onNarrationFinished;

    private Coroutine typingRoutine;
    private bool isTyping;
    private bool skipRequested;
    private bool isEnding;

    private void Awake()
    {
        if (textUI == null) textUI = GetComponentInChildren<TextMeshProUGUI>();
        if (textUI != null) textUI.text = "";

        if (uiGroup != null)
        {
            uiGroup.alpha = 1f;
            uiGroup.interactable = true;
            uiGroup.blocksRaycasts = true;
        }

        if (narration != null && narration.Length > 0)
            narrationIndex = Mathf.Clamp(narrationIndex, 0, narration.Length - 1);
        else
            narrationIndex = 0;
    }

    private void Start()
    {
        if (textUI == null) return;
        if (narration == null || narration.Length == 0) return;

        StartLine(startDelay);
    }

    private void Update()
    {
        if (isEnding) return;
        if (Mouse.current == null) return;

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // If still typing, skip to full line
        if (isTyping)
        {
            skipRequested = true;
            return;
        }

        // If last line is already shown, fade out UI, 
        // then start the ACTIVATE THE SPAWN ENEMY
        if (narrationIndex >= narration.Length - 1)
        {
            // ADDED: notify manager first (so it can switch camera + start gameplay)
            // onNarrationFinished?.Invoke();

            // StartCoroutine(FadeOutAndDisable());
            StartCoroutine(EndNarrationRoutine());

            //here to activate the enemy spawn script
            // ADDED COMMENT:
            // We do not start the spawner directly here anymore.
            // GameFlowManager listens to onNarrationFinished and calls spawner.StartSpawning().

            return;
        }

        // Otherwise go to next line
        narrationIndex++;
        StartLine(0f);
    }

    private void StartLine(float delaySeconds)
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeRoutine(delaySeconds));
    }

    private IEnumerator TypeRoutine(float delaySeconds)
    {
        yield return new WaitForSecondsRealtime(delaySeconds);

        string fullText = narration[narrationIndex];

        isTyping = true;
        skipRequested = false;

        textUI.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            if (skipRequested) break;

            textUI.text += fullText[i];
            yield return new WaitForSecondsRealtime(charDelay);
        }

        // Ensure full text is shown (handles skip)
        textUI.text = fullText;

        isTyping = false;
        skipRequested = false;
    }

    private IEnumerator FadeOutAndDisable()
    {
        isEnding = true;

        // Stop typing if somehow still running
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        isTyping = false;

        if (uiGroup == null)
        {
            gameObject.SetActive(false);
            yield break;
        }

        uiGroup.interactable = false;
        uiGroup.blocksRaycasts = false;

        float startAlpha = uiGroup.alpha;
        float t = 0f;

        while (t < fadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            uiGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeOutTime);
            yield return null;
        }

        uiGroup.alpha = 0f;

        if (disableAfterFade)
            uiGroup.gameObject.SetActive(false);
    }
    private IEnumerator EndNarrationRoutine()
{
    isEnding = true;

    // Fade out first UI completely first
    yield return FadeOutAndDisable();

    // NOW notify listeners (second canvas, gameplay, etc.)
    onNarrationFinished?.Invoke();
}

}
