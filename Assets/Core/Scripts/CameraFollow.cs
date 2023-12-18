using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.175f;
    public Vector3 offset;

    private bool hasFallen = false;

    private void LateUpdate()
    {
        if (target == null || !target.gameObject.activeSelf) return;

        if (target.position.y < transform.position.y - 2f - Camera.main.orthographicSize && !hasFallen)
        {
            GameEvents.TriggerEndGameEvent();
            hasFallen = true;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.y = Mathf.Max(desiredPosition.y, transform.position.y + GameManager.Instance.CameraSpeed * Time.deltaTime);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.position = new Vector3(0, transform.position.y, -10);
    }
}
