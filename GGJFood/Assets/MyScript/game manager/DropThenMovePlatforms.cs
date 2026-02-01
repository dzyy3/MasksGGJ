using System.Collections;                  // lets us use IEnumerator / coroutines
using UnityEngine;                         // Unity base namespace

public class DropThenMovePlatforms : MonoBehaviour
{
    [Header("Drop Settings")]

    //start above (how high can it will be), drop time, delay between drops
    [SerializeField] float startAbove = 20f;
    [SerializeField] float dropTime = 1.0f;
    [SerializeField] float delayBetween = 0.15f;

    [Header("Move Settings (after drop)")]
    [SerializeField] float moveRange = 3f;          
    [SerializeField] float minSpeed = 0.8f;  
    [SerializeField] float maxSpeed = 2.2f;

    Transform[] platforms; 
    Vector3[] finalPos;
    float[] moveSpeed;
    float[] phase;

    void Awake()
    {
        int count = transform.childCount;

        //store data for each platform
        platforms = new Transform[count];
        finalPos = new Vector3[count];
        moveSpeed = new float[count];
        phase = new float[count];

        // loop through each child platform and save info
        for (int i = 0; i < count; i++)
        {
            platforms[i] = transform.GetChild(i);              
            finalPos[i] = platforms[i].position;               
            moveSpeed[i] = Random.Range(minSpeed, maxSpeed);    
            phase[i] = Random.Range(0f, 1000f);           
        }
    }

    void Start()
    {
        //drop at start
        StartCoroutine(DropAllPlatforms());
    }

    IEnumerator DropAllPlatforms()
    {
        //start from above, then drop down
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].position = finalPos[i] + Vector3.up * startAbove;
        }

        // drop each platform one by one
        for (int i = 0; i < platforms.Length; i++)
        {
            yield return StartCoroutine(DropOnePlatform(i));

            yield return new WaitForSeconds(delayBetween);
        }
    }

    IEnumerator DropOnePlatform(int i)
    {
       
        Vector3 startPos = platforms[i].position;

        Vector3 targetPos = finalPos[i];

        float t = 0f;

        while (t < dropTime)
        {
            t += Time.deltaTime;               
            float a = t / dropTime;  
            a = Mathf.Clamp01(a);      

            // smoothly move 
            platforms[i].position = Vector3.Lerp(startPos, targetPos, a);

            yield return null;       
        }

        platforms[i].position = targetPos;
    }

    void Update()
    {
        // after they land, start move left or right
        for (int i = 0; i < platforms.Length; i++)
        {
            float centerX = finalPos[i].x;

            float xOffset = Mathf.Sin((Time.time + phase[i]) * moveSpeed[i]) * moveRange;

            Vector3 p = platforms[i].position;

            platforms[i].position = new Vector3(centerX + xOffset, p.y, p.z);
        }
    }
}
