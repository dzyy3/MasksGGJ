using UnityEngine;

public class Fish : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("bullet")) return;

        Destroy(gameObject);
    }
}
