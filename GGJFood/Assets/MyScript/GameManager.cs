using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Singleton
public class GameManager : MonoBehaviour
{
    // Memory에 Instance라는 변수명으로 GameManager 자료형을 사전에 할당
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    // public string sceneName = null;

    // public int a = 20;
    // [SerializeField] private string[] moveSceneNames;
    public Image fadeImg;

    void Awake()
    {
        if (instance == null)
        {
            // when any data was not applied in Instance before - 사전에 한 번이라도 Instance에 데이터가 할당되지 않았으면
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // data is already assigned in Instance - 이미 Instance에 데이터가 할당되어 있음
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameData.PopupImageActive = false;

        if (fadeImg == null)
        {
            fadeImg = GameObject.Find("fadeImg")?.GetComponent<Image>();
            if (fadeImg == null)
            {
                Debug.LogWarning("[Fade] fadeImg is still null after scene load!");
            }
        }
    }

    // Scene move with fade effect
    public void MoveScene(string sceneName, int index)
    {
        Debug.Log("[TEST] MoveScene called with sceneName: " + sceneName + ", index: " + index);
        if (fadeImg == null)
        {
            return;
        }

        StartCoroutine(MoveSceneCoroutine(sceneName, index));
    }
    private IEnumerator MoveSceneCoroutine(string sceneName, int index)
    {
        // Fade In
        fadeImg.color = new Color(0, 0, 0, 0);
        fadeImg.gameObject.SetActive(true);
        GameData.SceneMoveEnabled = true;
        
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            time = Mathf.Clamp(time, 0, 1);
            fadeImg.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, time));

            yield return null;
        }

        // Ensure fadeImg is fully opaque after fade-in
        fadeImg.color = new Color(0, 0, 0, 1);

        // Loading Bar activate

        // 1. call Empty Scene(prevent Memory Burst)
        var emptyAsync = SceneManager.LoadSceneAsync("0.Empty");
        while (!emptyAsync.isDone)
        {
            //loading bar용 데이터 불러오기: var progress = emptyAsync.progress;
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        // 2. Scene move
        var moveAsync = SceneManager.LoadSceneAsync(sceneName);
        
        while (!moveAsync.isDone)
        {
            yield return null;
        }

        // Loading Bar deactivate
        // Fade Out
        time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time * 2); // [0 ~ 1]로 강제
            float alpha = Mathf.Lerp(1f, 0f, t);
            if (fadeImg != null)
            {
                fadeImg.color = new Color(0, 0, 0, 0); // transparent
                fadeImg.gameObject.SetActive(false);
            }
            yield return null;
        }


    }
}