using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.175f;
    public Vector3 offset;

    public float cameraRiseSpeed = 0.5f; // Velocidade inicial de subida da câmera
    public float cameraRiseAcceleration = 0.01f; // Aceleração da subida da câmera
    private float currentCameraRiseSpeed;

    private bool hasFallen = false;

    private void Start()
    {
        currentCameraRiseSpeed = cameraRiseSpeed;
    }

    private void LateUpdate()
    {
        if (target == null || !target.gameObject.activeSelf) return;

        if (target.position.y < transform.position.y - 2f - Camera.main.orthographicSize && !hasFallen)
        {
            GameEvents.TriggerEndGameEvent();
            hasFallen = true;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.y = Mathf.Max(desiredPosition.y, transform.position.y + currentCameraRiseSpeed * Time.deltaTime);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.position = new Vector3(0, transform.position.y, -10);

        currentCameraRiseSpeed += cameraRiseAcceleration * Time.deltaTime;
    }
}