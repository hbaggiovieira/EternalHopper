using UnityEngine;

public class SeesawAnimation : MonoBehaviour
{
    public float angle = 30.0f;
    public float speed = 2.0f;

    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time - startTime, 1) * 2 - 1;
        float rotationAngle = t * angle;
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }
}
