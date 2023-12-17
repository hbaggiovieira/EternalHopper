using UnityEngine;

public class InfiniteScrolling : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    private GameObject[] backgrounds;
    private float[] startPositions;
    private float[] imageHeights;
    [SerializeField]
    private float parallaxEffect = 0.3f;

    void Start()
    {
        startPositions = new float[backgrounds.Length];
        imageHeights = new float[backgrounds.Length];

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].GetComponent<SpriteRenderer>() != null)
            {
                // Calculando a altura do sprite
                float height = backgrounds[i].GetComponent<SpriteRenderer>().bounds.size.y;
                imageHeights[i] = height;
            }
            else
            {
                Debug.LogError("SpriteRenderer not found on background object.");
            }

            startPositions[i] = backgrounds[i].transform.position.y;
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float temp = (camera.transform.position.y * (1 - parallaxEffect));
            float distance = (camera.transform.position.y * parallaxEffect);

            backgrounds[i].transform.position = new Vector3(backgrounds[i].transform.position.x, startPositions[i] + distance, backgrounds[i].transform.position.z);

            if (temp < startPositions[i] - imageHeights[i])
            {
                startPositions[i] -= imageHeights[i] * backgrounds.Length;
            }
            else if (temp > startPositions[i] + imageHeights[i])
            {
                startPositions[i] += imageHeights[i] * backgrounds.Length;
            }
        }
    }
}
