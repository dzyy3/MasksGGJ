using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string sampleSceneName = "intro";
    [SerializeField] private string StageSceneName = "3_stage3";

    // ADDED: cached references we can re-find when scenes change
    private CameraRigManager cameraRig;
    private ThirdEnemySpawner spawner;

    private void Awake()
    {
        // ADDED: singleton so you keep one manager across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // ADDED: listen to scene loads so we can re-hook references each time
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // ADDED: in case you press Play from SampleScene
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ADDED: find scene-specific objects
        cameraRig = FindObjectOfType<CameraRigManager>(true);
        spawner = FindObjectOfType<ThirdEnemySpawner>(true);

        if (scene.name == sampleSceneName)
        {
            // ADDED: intro scene should use intro vcam
            if (cameraRig != null) cameraRig.UseIntroCamera();
        }

        if (scene.name == StageSceneName)
        {
            // ADDED: first stage starts on narration camera if you set it as introCamera in that scene
            // If you want separate "narration vcam" and "gameplay vcam", put narration vcam in introCamera slot.
            if (cameraRig != null) cameraRig.UseIntroCamera();

            // ADDED: spawner must NOT run during narration
            if (spawner != null) spawner.StopSpawning();
        }
    }

    // ADDED: called by TypewriterText when player clicks the last line in SampleScene
    public void GoToNextStage()
    {
        SceneManager.LoadScene(StageSceneName);
    }

    // ADDED: called by FirstStageNarration when player clicks the last narration line
    public void StageGameplay()
    {
        // switch to gameplay vcam
        if (cameraRig != null) cameraRig.UseGameplayCamera();

        // start spawning enemies
        if (spawner != null) spawner.StartSpawning();
    }
}
