using UnityEngine;

public class SphericalMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravityStrength = 10f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;
    private float rotationX = 0f;

    private Rigidbody rb;
    private Transform planetTransform; // We'll need a reference to your planet's transform

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // We'll handle gravity ourselves

        // Assuming your planet is the first object tagged "Planet" in the scene
        GameObject planet = GameObject.FindGameObjectWithTag("Planet");
        if (planet != null)
        {
            planetTransform = planet.transform;
        }
        else
        {
            Debug.LogError("No GameObject with the tag 'Planet' found in the scene. Please tag your planet object.");
            enabled = false; // Disable the script if no planet is found
        }

        // Make sure the camera transform is assigned
        if (cameraTransform == null)
        {
            cameraTransform = GetComponentInChildren<Camera>().transform;
            if (cameraTransform == null)
            {
                Debug.LogError("No Camera found as a child of the Player. Please assign the cameraTransform in the Inspector.");
                enabled = false; // Disable the script if no camera is found
            }
        }

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle player input for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate the movement direction relative to the planet's surface
        if (planetTransform != null)
        {
            Vector3 playerToCenter = (transform.position - planetTransform.position).normalized;
            Vector3 moveDirection = transform.forward * moveVertical + transform.right * moveHorizontal;

            // Project the movement direction onto the tangent plane of the planet's surface
            Vector3 projectedMoveDirection = Vector3.ProjectOnPlane(moveDirection, playerToCenter).normalized;

            // Apply force to the Rigidbody
            rb.AddForce(projectedMoveDirection * moveSpeed, ForceMode.VelocityChange);

            // Apply custom gravity towards the planet's center
            rb.AddForce(-playerToCenter * gravityStrength, ForceMode.Acceleration);

            // Rotate the player to align with the planet's surface normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, playerToCenter) * transform.rotation;
            transform.rotation = targetRotation;
        }

        // Handle camera rotation based on mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Prevent looking too far up or down

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}