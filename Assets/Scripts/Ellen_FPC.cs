using UnityEngine;
using UnityEngine.UI;

public class Ellen_FPC : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float jumpForce = 5f;
    public Transform cameraTransform;
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;
    public int damagePerSecond = 5;
    public int healAmount = 10;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float rotationX = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
            Debug.LogError("CharacterController component is missing!");

        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTransform == null)
            Debug.LogError("Camera Transform is not assigned in the Inspector!");

        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.minValue = 0;
            healthBar.value = currentHealth;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleJump();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleLook()
    {
        if (cameraTransform == null) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }

        moveDirection.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grenadier"))
        {
            TakeDamage(10);
        }
        else if (other.CompareTag("Heart"))
        {
            Heal(healAmount);
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Grenadier"))
        {
            TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= Mathf.RoundToInt(damage);
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Current Health: " + currentHealth);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            Debug.Log("Slider Value: " + healthBar.value);
        }

        if (currentHealth == 0)
        {
            Debug.Log("Ellen is dead!");
            // Implement death behavior such as disabling movement or triggering game over UI
        }
    }

    void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        Debug.Log("Ellen healed! Current Health: " + currentHealth);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }
}
