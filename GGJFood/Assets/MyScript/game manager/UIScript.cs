using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events; // ADDED: so you can hook this in Inspector without writing extra code

public class UIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;

    [Header("Narration")]
    [TextArea] [SerializeField] private string[] narration;
    [SerializeField] private int narrationIndex = 0;

    [Header("Typing")]
    [SerializeField] private float charDelay = 0.03f;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "3_stage3"; // put your First stage scene name here
    // ADDED: We keep nextSceneName so you don't lose your setup,
    // but scene switching is now handled by GameFlowManager for cleaner scaling.

    // ADDED: this gets fired when the player clicks on the last line
    [Header("Handoff (GameFlowManager listens to this)")]
    public UnityEvent onLastLineFinished;

    private Coroutine typingRoutine;
    private bool isTyping = false;
    private bool skipRequested = false;
    private bool readyForInput = false;

    private void Awake()
    {
        if (textUI == null) textUI = GetComponent<TextMeshProUGUI>();
        if (textUI != null) textUI.text = "";
    }

    // Call this when secondUI is fully visible
    public void StartTyping(float delaySeconds)
    {
        if (textUI == null) return;
        if (narration == null || narration.Length == 0) return;

        readyForInput = true;
        narrationIndex = Mathf.Clamp(narrationIndex, 0, narration.Length - 1);

        StartLine(delaySeconds);
    }

    private void Update()
    {
        if (!readyForInput) return;
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // If still typing -> skip to full line
            if (isTyping)
            {
                skipRequested = true;
                return;
            }

            // If finished typing:
            // If we're at the last line -> load next scene
            if (narrationIndex >= narration.Length - 1)
            {
                // ADDED: instead of loading scene here, we notify the manager
                // This keeps this script focused on UI typing only.
                readyForInput = false; // ADDED: prevent double firing from extra clicks
                Debug.Log("Typewriter: last line finished -> invoke event");
                onLastLineFinished?.Invoke();
                return;
            }

            // Otherwise go to next line
            narrationIndex++;
            StartLine(0f);
        }
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

        // finish line (also handles skip)
        textUI.text = fullText;

        isTyping = false;
        skipRequested = false;
    }
}
