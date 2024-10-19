using UnityEngine;
using Mirror;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform cameraHolder;
    private Vector3 viewPunchDirection;
    private float yRotation;
    private Camera mainCamera;

    [Header("View Bobbing")]
    [SerializeField] private float bobbingSpeed = 0.1f;
    [SerializeField] private float bobbingAmount = 0.1f;
    private Vector3 targetPosition;
    private Vector3 defaultCameraPos;
    private float timer;

    [Header("Camera Tilt")]
    [SerializeField] private float maxTiltAngle;
    [SerializeField] private float tiltSpeed;
    private float tiltAngle;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultCameraPos = transform.localPosition;
        mainCamera = GetComponent<Camera>();
        gameObject.SetActive(isLocalPlayer);
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) return;

        Look();
        ViewBobbing();
        CameraTilt();

        viewPunchDirection = Vector3.Lerp(viewPunchDirection, Vector3.zero, 5f * Time.deltaTime);
        cameraHolder.localRotation = Quaternion.Lerp(cameraHolder.localRotation, Quaternion.Euler(-viewPunchDirection.y, viewPunchDirection.x, -viewPunchDirection.z), 4.5f * Time.deltaTime);
    }

    private void CameraTilt()
    {
        tiltAngle = Mathf.Lerp(tiltAngle, maxTiltAngle * -Input.GetAxis("Horizontal"), tiltSpeed * Time.deltaTime);
    }

    private void ViewBobbing()
    {
        if (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude > 0.1f)
        {
            targetPosition.y = Mathf.Sin(timer * bobbingSpeed * 2f) * bobbingAmount;
            timer += Time.deltaTime;

            if (timer > Mathf.PI * 2)
                timer -= Mathf.PI * 2;
        }
        else
        {
            timer = 0f;
            targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, 5f * Time.deltaTime);
        }

        transform.localPosition = defaultCameraPos + targetPosition;
    }

    public void ViewPunch(Vector3 direction) => viewPunchDirection += direction;

    private void Look()
    {
        yRotation -= Input.GetAxisRaw("Mouse Y");
        yRotation = Mathf.Clamp(yRotation, -90.0f, 90.0f);
        transform.localRotation = Quaternion.Euler(yRotation, 0.0f, tiltAngle);
        orientation.Rotate(Input.GetAxisRaw("Mouse X") * Vector3.up);
    }
}
