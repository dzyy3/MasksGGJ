using UnityEngine;

public class CameraRigManager : MonoBehaviour
{
    [Header("Cameras (drag from Hierarchy)")]
    // CHANGED: These are now VCam GameObjects (IntroVcam, StageNarrationVcam, GameplayVcam)
    // This avoids Cinemachine namespace issues and works in both CM2 and CM3.
    [SerializeField] private GameObject introCamera;
    [SerializeField] private GameObject gameplayCamera;
    [SerializeField] private GameObject[] extraCamerasOff;

    // Call this from your story/scene manager (or button flow)
    public void UseIntroCamera()
    {
        SetActive(introCamera);
    }

    // Call this from your story/scene manager
    public void UseGameplayCamera()
    {
        SetActive(gameplayCamera);
    }

    // Call this for any custom camera you want
    public void UseCamera(GameObject cam)
    {
        SetActive(cam);
    }

    // ADDED: clean switching by enabling only one VCam object
    // CinemachineBrain will automatically pick the enabled VCam.
    private void SetActive(GameObject camToEnable)
    {
        // turn off any extras
        if (extraCamerasOff != null)
        {
            for (int i = 0; i < extraCamerasOff.Length; i++)
                if (extraCamerasOff[i] != null) extraCamerasOff[i].SetActive(false);
        }

        // turn off main ones
        if (introCamera != null) introCamera.SetActive(false);
        if (gameplayCamera != null) gameplayCamera.SetActive(false);

        // turn on the chosen one
        if (camToEnable != null) camToEnable.SetActive(true);
    }
}
