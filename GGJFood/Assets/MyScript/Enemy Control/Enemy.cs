using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public string playerTag = "Player";

    private Transform target;
    private Rigidbody rb;
    private Camera mainCam;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        mainCam = Camera.main;

        if (player != null)
        {
            target = player.transform;
            // Debug.Log("Enemy targeting: " + player.name);
        }
        else
        {
            Debug.LogError("No GameObject tagged 'Player' found.");
        }
    }

    void Update()
    {
        if (target != null)
        {
            FollowTarget();
        }

        DestroyIfOutOfCamera();
    }

    private void FollowTarget()
    {
        Vector3 pos = rb.position;

        float dx = target.position.x - pos.x;
        if (Mathf.Abs(dx) < 0.01f) return;

        float step = Mathf.Sign(dx) * moveSpeed * Time.fixedDeltaTime;

        Vector3 nextPos = new Vector3(pos.x + step, pos.y, pos.z);
        rb.MovePosition(nextPos);

        rb.rotation = step > 0f
            ? Quaternion.Euler(0f, -90f, 0f)
            : Quaternion.Euler(0f, 90f, 0f);
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
