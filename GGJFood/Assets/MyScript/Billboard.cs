using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCam.forward);
    }
}
