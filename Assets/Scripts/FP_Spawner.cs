using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonSpawner : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float jumpForce = 5f;

    [Header("Camera Settings")]
    // Manually assign this Camera in the Inspector if desired.
    public Camera playerCamera;
    private Transform cameraTransform;

    private CharacterController characterController;
    // Used for vertical velocity (jump & gravity)
    private Vector3 moveDirection; 
    private float rotationX = 0f;

    private Terrain terrain;

    IEnumerator Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing!");
        }

        // Wait until the terrain is available.
        while (Terrain.activeTerrain == null)
        {
            yield return null;
        }
        terrain = Terrain.activeTerrain;

        // Position the character on the terrain.
        PositionOnTerrain();

        // Ensure a camera is assigned: if not, try to find one.
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
        }
        if (playerCamera != null)
        {
            cameraTransform = playerCamera.transform;
        }
        else
        {
            Debug.LogError("No camera found for first person view!");
        }

        // Lock the cursor for a first-person experience.
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    /// <summary>
    /// Positions the character at the center of the terrain.
    /// Adjust this method if you need a different spawn location.
    /// </summary>
    void PositionOnTerrain()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is not available to position character.");
            return;
        }

        // Get terrain position and size.
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        // Calculate the center of the terrain.
        float centerX = terrainPos.x + terrainSize.x * 0.5f;
        float centerZ = terrainPos.z + terrainSize.z * 0.5f;
        float sampleHeight = terrain.SampleHeight(new Vector3(centerX, 0, centerZ)) + terrainPos.y;

        // Adjust spawn position so that the bottom of the CharacterController sits on the terrain.
        float bottomOffset = characterController.height * 0.5f;
        Vector3 spawnPosition = new Vector3(centerX, sampleHeight + bottomOffset, centerZ);
        transform.position = spawnPosition;

        Debug.Log("Spawn position: " + spawnPosition);
    }

    /// <summary>
    /// Handles WASD movement, jump, and applies gravity.
    /// </summary>
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        // Calculate horizontal movement relative to character orientation.
        Vector3 horizontalMovement = (transform.right * moveX + transform.forward * moveZ) * moveSpeed;

        if (characterController.isGrounded)
        {
            // Reset vertical movement if grounded.
            moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }
        // Apply gravity continuously.
        moveDirection.y += Physics.gravity.y * Time.deltaTime;

        // Combine horizontal and vertical movement.
        Vector3 combinedMovement = horizontalMovement + new Vector3(0, moveDirection.y, 0);
        characterController.Move(combinedMovement * Time.deltaTime);
    }

    /// <summary>
    /// Handles mouse look for both vertical (camera) and horizontal (player) rotation.
    /// </summary>
    void HandleLook()
    {
        if (cameraTransform == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // Adjust vertical rotation and clamp it.
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Rotate the character horizontally.
        transform.Rotate(Vector3.up * mouseX);
    }
}
