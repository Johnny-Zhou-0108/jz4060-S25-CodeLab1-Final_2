using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.2f; // Duration of the camera shake
    [SerializeField] private float shakeMagnitude = 0.1f; // Magnitude of the shake

    private Transform cameraTransform;
    private Vector3 originalPosition;

    private void Awake()
    {
        // Cache the camera's transform
        cameraTransform = transform;
        originalPosition = cameraTransform.localPosition;
    }

    // Method to trigger the shake
    public void TriggerShake()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Generate a random offset for the camera
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            // Apply the shake offset
            cameraTransform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Reset the camera to its original position
        cameraTransform.localPosition = originalPosition;
    }
}
