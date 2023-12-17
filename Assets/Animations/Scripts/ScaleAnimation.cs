using UnityEngine;
using System.Collections;

public class ScaleAnimation : MonoBehaviour
{
    public Vector2 scaleIncrease = new Vector2(1.1f, 1.1f);
    public float duration = 0.1f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(StartWithRandomDelay());
    }

    private IEnumerator StartWithRandomDelay()
    {
        float randomDelay = Random.Range(0f, 0.3f);
        yield return new WaitForSeconds(randomDelay);
        StartCoroutine(AnimateScale());
    }

    private IEnumerator AnimateScale()
    {
        while (true)  // Loop infinito
        {
            float elapsed = 0f;

            while (elapsed < duration / 2)
            {
                transform.localScale = Vector3.Lerp(originalScale, new Vector3(originalScale.x * scaleIncrease.x, originalScale.y * scaleIncrease.y, originalScale.z), elapsed / (duration / 2));
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;

            while (elapsed < duration / 2)
            {
                transform.localScale = Vector3.Lerp(new Vector3(originalScale.x * scaleIncrease.x, originalScale.y * scaleIncrease.y, originalScale.z), originalScale, elapsed / (duration / 2));
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;

            float randomDelayBetweenCycles = Random.Range(0f, 0.1f);
            if (randomDelayBetweenCycles > 0f)
            {
                yield return new WaitForSeconds(randomDelayBetweenCycles);
            }
        }
    }
}
