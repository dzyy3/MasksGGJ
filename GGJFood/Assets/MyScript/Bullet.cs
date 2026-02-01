using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        DestroyIfOutOfCamera();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return; // don't hit yourself
        if (collision.gameObject.CompareTag("bullet")) return;

        Destroy(gameObject);
    }


    void DestroyIfOutOfCamera()
    {
        if (mainCam == null) return;

        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);

        bool isOutOfView =
            viewportPos.x < 0f || viewportPos.x > 1f ||
            viewportPos.y < 0f || viewportPos.y > 1f ||
            viewportPos.z < 0f;

        if (isOutOfView)
        {
            Destroy(gameObject);
        }
    }

}
