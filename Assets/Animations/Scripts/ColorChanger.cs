using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material material;
    public float changeSpeed = 1.0f;

    void Update()
    {
        float r = Mathf.PingPong(Time.time * changeSpeed, 1);
        float g = Mathf.PingPong(Time.time * changeSpeed + 1 / 3f, 1);
        float b = Mathf.PingPong(Time.time * changeSpeed + 2 / 3f, 1);
        material.SetColor("_Color", new Color(r, g, b));
    }
}
