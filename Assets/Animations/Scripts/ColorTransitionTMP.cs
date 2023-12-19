using System.Collections;
using UnityEngine;
using TMPro;

public class ColorTransitionTMP : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public float transitionTime = 2f;

    private void Start()
    {
        StartCoroutine(ColorTransition());
    }

    private IEnumerator ColorTransition()
    {
        Color[] colors = new Color[] { Color.red, Color.green, Color.blue };

        while (true)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                int nextColorIndex = (i + 1) % colors.Length;
                float startTime = Time.time;

                while (Time.time < startTime + transitionTime)
                {
                    tmpText.color = Color.Lerp(colors[i], colors[nextColorIndex], (Time.time - startTime) / transitionTime);
                    yield return null;
                }
            }
        }
    }
}
