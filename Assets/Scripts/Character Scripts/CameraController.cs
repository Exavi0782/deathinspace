using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform parentObject;
    [SerializeField] private Animator animator;
    [SerializeField] private float cameraSpeed = 1.0f;
    [SerializeField] private float maxCameraAngleUp = 60.0f;
    [SerializeField] private float maxCameraAngleDown = -60.0f;
    [SerializeField] private float minCameraDistance = 2.0f;
    [SerializeField] private float maxCameraDistance = 10.0f;
    [SerializeField] private float smoothingFactor = 0.1f;
    private float cameraAngle = 0.0f;
    [SerializeField] private float currentCameraDistance = 2.5f;
    private Vector2 smoothedMouseInput = Vector2.zero;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        smoothedMouseInput = Vector2.Lerp(smoothedMouseInput, new Vector2(mouseX, mouseY), smoothingFactor);

        cameraAngle -= smoothedMouseInput.y * cameraSpeed;
        cameraAngle = Mathf.Clamp(cameraAngle, maxCameraAngleDown, maxCameraAngleUp);

        Quaternion parentRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
        parentObject.Rotate(Vector3.up, smoothedMouseInput.x * cameraSpeed); 

        Quaternion rotation = Quaternion.Euler(cameraAngle, parentObject.eulerAngles.y, 0); 
        Vector3 offset = new Vector3(0, 0, currentCameraDistance);
        Vector3 cameraPosition = target.position - (rotation * offset);
        transform.position = cameraPosition;

        float additionalRotation = smoothedMouseInput.y * cameraSpeed; 
        transform.RotateAround(target.position, Vector3.up, additionalRotation);

        float rotationForAnimator = smoothedMouseInput.x * cameraSpeed;
        animator.SetFloat("Turn", rotationForAnimator);

        transform.LookAt(target);

        currentCameraDistance -= Input.GetAxis("Mouse ScrollWheel") * cameraSpeed;
        currentCameraDistance = Mathf.Clamp(currentCameraDistance, minCameraDistance, maxCameraDistance);


    }
}
