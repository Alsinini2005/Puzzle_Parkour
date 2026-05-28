using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Vector3 heldItemRotation = new Vector3(0f, -100f, 0f);
    [Header("Animation Settings")]
    public Animator anim;

    [Header("Movement Settings")]
    public float walkSpeed = 10f;
    public float runSpeed = 15f;
    public float jumpForce = 9.5f;
    public float mouseSensitivity = 2f;
    public float gravityMultiplier = 4f;

    [Header("Crouch Settings")]
    public float crouchSpeed = 5f;
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 10f;

    [Header("UI Bars")]
    public Slider strengthSlider;
    public Slider boostSlider;

    [Header("Stats Settings")]
    public float maxStrength = 100f;
    public float maxBoost = 100f;
    [Range(0, 50)] public float runCost = 15f;
    [Range(0, 50)] public float jumpCost = 20f;
    public float regenRate = 10f;

    [Header("Interaction Settings")]
    public float interactDistance = 4f;
    public LayerMask interactableLayer;

    [Header("Pickup Settings")]
    public Transform holdPoint;
    public GameObject holdItem;

    [Header("Platform Settings")]
    public float rayDistance = 1.5f; 
    public LayerMask groundLayer; 

    [Header("UI Settings")]
    public GameObject interactionLabel; 
    public TMPro.TextMeshProUGUI interactionText;
    
    [Header("Stance UI Icons")]
    public Image stanceIconImage; 
    public Sprite standingSprite;  
    public Sprite crouchingSprite;
    [Header("Camera Settings")]
    public Transform cameraMount;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isRunning;
    private bool isCrouching;
    private float xRotation = 0f;
    private Transform cameraTransform;
    private CapsuleCollider playerCollider;
    private float currentStrength;
    private float currentBoost;
    [Header("Camera Positions")]
    public Transform camTransform;
    public Vector3 standPos = new Vector3(-0.05f, 1.7f, 0.07f);
    public Vector3 crouchPos = new Vector3(-0.05095164f, 0.857f, 0.616f);
    public Vector3 camRotation = new Vector3(-18.451f, -4.67f, 4.398f);

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip lowEnergySound;
    private bool hasPlayedLowEnergySound = false;

    [Header("Spawn Settings")]
    public Transform spawnPoint;

    void Start()
{
    GetComponent<PlayerInput>().actions["Throw"].performed += OnDrop;
    GhostRecorder recorder = GetComponent<GhostRecorder>();
    if (recorder != null)
    {
        recorder.StartRecording();
    }

    rb = GetComponent<Rigidbody>();
    playerCollider = GetComponent<CapsuleCollider>();
    cameraTransform = GetComponentInChildren<Camera>().transform;
    rb.freezeRotation = true;
    Cursor.lockState = CursorLockMode.Locked;

    currentStrength = maxStrength;
    currentBoost = maxBoost;

    if (strengthSlider != null)
    {
        strengthSlider.maxValue = maxStrength;
        strengthSlider.value = maxStrength;
        strengthSlider.fillRect.GetComponent<Image>().color = new Color32(255, 77, 0, 255);
    }

    if (boostSlider != null)
    {
        boostSlider.maxValue = maxBoost;
        boostSlider.value = maxBoost;
        boostSlider.fillRect.GetComponent<Image>().color = new Color32(0, 217, 255, 255);
    }

    Invoke(nameof(TeleportToSavedPosition), 0.1f);
    if (SceneManager.GetActiveScene().buildIndex == 5)
    {
        Invoke(nameof(TeleportToSavedPosition), 0.1f);
    }
}


public void TeleportToSavedPosition()
{
    if (GameManager.instance == null) return;

    int levelIndex = SceneManager.GetActiveScene().buildIndex;
    
    Vector3 defaultPos = spawnPoint != null ? spawnPoint.position : transform.position;
    Vector3 savedPosition = GameManager.instance.LoadCheckpoint(levelIndex, defaultPos);

    rb.isKinematic = true;
    transform.position = savedPosition;
    rb.isKinematic = false;
    rb.linearVelocity = Vector3.zero;
}
    void UpdateAnimations(){
    if (anim == null) return;

    bool isMoving = moveInput.magnitude > 0;

    anim.SetBool("isWalking", isMoving && !isRunning && !isCrouching);
    anim.SetBool("isRunning", isMoving && isRunning && !isCrouching);
    anim.SetBool("isGrounded", isGrounded);
    
    anim.SetBool("isCrouching", isCrouching);
    anim.SetBool("isCrouchingWalking", isCrouching && isMoving); 
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        UpdateInteractionUI();
        HandleStatsSystem();
        UpdateAnimations();
    }

void HandleStatsSystem()
{
    if (isRunning && moveInput.magnitude > 0 && isGrounded && currentBoost > 0)
    {
        currentBoost -= runCost * Time.deltaTime;
    }
    else if (moveInput.magnitude == 0 && isGrounded) 
    {
        currentBoost = Mathf.MoveTowards(currentBoost, maxBoost, regenRate * Time.deltaTime);
        currentStrength = Mathf.MoveTowards(currentStrength, maxStrength, regenRate * Time.deltaTime);
    }

    if (currentBoost <= 25f || currentStrength <= 25f)
    {
        if (!hasPlayedLowEnergySound && audioSource != null && lowEnergySound != null)
        {
            audioSource.PlayOneShot(lowEnergySound);
            hasPlayedLowEnergySound = true;
        }
    }
    else
    {
        hasPlayedLowEnergySound = false;
    }

    if (strengthSlider) strengthSlider.value = currentStrength;
    if (boostSlider) boostSlider.value = currentBoost;

    UpdateBarColor(strengthSlider, currentStrength, new Color32(255, 77, 0, 255));
    UpdateBarColor(boostSlider, currentBoost, new Color32(0, 217, 255, 255));

    if (currentBoost <= 0) isRunning = false;
}

void UpdateBarColor(Slider slider, float currentValue, Color originalColor)
{
    if (slider != null && slider.fillRect != null)
    {
        Image fillImage = slider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            fillImage.color = (currentValue <= 10f) ? Color.red : originalColor;
        }
    }
}

    void LateUpdate() 
    {
        if (Time.timeScale == 0f) return;
        HandleLook();
        HandleCrouchScale();
        UpdateStanceUI(); 
        HandleHeldItemRotation();
    }

    void HandleHeldItemRotation()
    {
        if (holdItem != null)
        {
            holdItem.transform.localRotation = Quaternion.Euler(heldItemRotation);
        }
    }

    void UpdateStanceUI()
    {
        if (stanceIconImage != null && standingSprite != null && crouchingSprite != null)
        {
            stanceIconImage.sprite = isCrouching ? crouchingSprite : standingSprite;
        }
    }

    void FixedUpdate()
{    
    if (Time.timeScale == 0f) return;
    CheckGroundWithRaycast();
    
    HandleMovement();
    ApplyExtraGravity();
}

    void UpdateInteractionUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                interactionLabel.SetActive(true);
                interactionText.text = "Press [E] to Interact";
            }
            else { interactionLabel.SetActive(false); }
        }
        else { interactionLabel.SetActive(false); }
    }

    void CheckGroundWithRaycast()
    {
    RaycastHit hit;
    bool layerHit = Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.2f, Vector3.down, out hit, rayDistance + 0.5f, groundLayer);
    isGrounded = layerHit || (Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.2f, Vector3.down, out hit, rayDistance + 0.5f) && hit.collider.CompareTag("Ground"));
    }

void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("MovingPlatform"))
    {
        transform.SetParent(collision.transform);
    }
}

void OnCollisionExit(Collision collision)
{
    if (collision.gameObject.CompareTag("MovingPlatform"))
    {
        transform.SetParent(null);
    }
}

    void HandleMovement()
{
    float currentSpeed = 0;

    if (currentBoost <= 0)
    {
        currentSpeed = 0;
    }
    else
    {
        bool canRun = isRunning && currentBoost > 2f;
        currentSpeed = canRun && !isCrouching ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed);
    }
    
    Vector3 moveDir = transform.forward * moveInput.y + transform.right * moveInput.x;
    Vector3 targetVelocity = moveDir.normalized * currentSpeed;

    float verticalVelocity = rb.linearVelocity.y;

    if (transform.parent != null && transform.parent.CompareTag("MovingPlatform"))
    {
        Rigidbody platformRb = transform.parent.GetComponent<Rigidbody>();
        if (platformRb != null)
        {
            Vector3 platformVelocity = platformRb.linearVelocity;
            rb.linearVelocity = new Vector3(targetVelocity.x + platformVelocity.x, verticalVelocity, targetVelocity.z + platformVelocity.z);
            return;
        }
    }

    rb.linearVelocity = new Vector3(targetVelocity.x, verticalVelocity, targetVelocity.z);
}

public void OnJump(InputValue value) 
{ 
    if (value.isPressed && isGrounded && currentStrength >= jumpCost) 
    { 
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        currentStrength -= jumpCost * 0.5f;

        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
        if (anim != null) anim.SetTrigger("JumpTrigger"); 
    } 
}
    void ApplyExtraGravity()
    {
        if (!isGrounded) rb.AddForce(Vector3.down * gravityMultiplier * 9.81f, ForceMode.Acceleration);
    }

    void HandleLook()
    {
        if (UIManager.isGamePaused) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * 0.05f;
        transform.Rotate(Vector3.up * mouseDelta.x);
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

void HandleCrouchScale()
{
    float targetHeight = isCrouching ? crouchHeight : standingHeight;
    playerCollider.height = Mathf.Lerp(playerCollider.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    
    Vector3 targetPos = isCrouching ? crouchPos : standPos;
    camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, targetPos, Time.deltaTime * crouchTransitionSpeed);
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnSprint(InputValue value) => isRunning = value.isPressed;

    public void OnCrouch(InputValue value) 
{ 
    if (value.isPressed) 
    {
        isCrouching = !isCrouching;
    }
}

    public void OnInteract(InputValue value)
    {
    if (!value.isPressed) return;

    RaycastHit hit;
    if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactDistance, interactableLayer))
    {
        IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
            return;
        }

        if (holdItem == null)
        {
            IPickupable pickup = hit.collider.GetComponentInParent<IPickupable>();
            if (pickup != null)
            {
                holdItem = hit.collider.transform.root.gameObject;
                pickup.OnPickedUp(holdPoint);
            }
        }
    }
}

    public void OnDrop(InputAction.CallbackContext context) 
{
    if (!context.performed || holdItem == null) return;
    
    IPickupable pickup = holdItem.GetComponent<IPickupable>();
    
    if (pickup != null) 
    { 
        pickup.DropBox(); 
        holdItem = null; 
    }
}
    public void BouncePlayer(float force)
    {
        if (rb != null)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;

            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        }
    }
}