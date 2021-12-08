using UnityEngine;

public class Player : MonoBehaviour
{

    public CharacterController CharacterController;
    public Transform PlayerCamera;
    public Transform GroundCheck;
    public UI UIController;
    public SpawnFinder SpawnFinderController;

    public float MouseSensitivity = 100f;
    public float Speed = 12f;
    public float MaxSpeed = 140f;
    public float JumpHeight = 3f;
    public float JetpackDelay = 0.5f;
    public float JetpackHeight = 1f;
    public float GroundDistance = 0.5f;
    public LayerMask GroundMask;
    public LayerMask BarnMask;
    public float Gravity = -9.81f;
    public bool DebugMovement = false;
    public Transform CameraLow;
    public Transform CameraHigh;
    public float CameraPositionFactor = 0.5f;
    public float CameraHighAngle = 80f;
    public float CameraLowAngle = -20f;
    public static Player Instance;

    private float xRotation = 0f;
    private bool isGrounded = false;
    private float lastJump = float.MaxValue;
    private Vector3 velocity = Vector3.zero;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Instance = this;
    }

    void Update()
    {
        if (Pause.isPaused)
            return;
        Look();
        Shoot();
        if (DebugMovement)
        {
            DebugMove();
        }
        else
        {
            Move();
            Jump();
            Jetpack();
            Fall();
        }
    }

    private void Shoot()
    {
        if (Input.GetButton("Fire1") && Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out RaycastHit raycastHit))
            SpawnFinder.Spawnable(raycastHit.point, 5, 5);
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;
        xRotation = Mathf.Clamp(xRotation - mouseY, CameraLowAngle, CameraHighAngle);
        float angleRange = CameraHighAngle - CameraLowAngle;
        float angleRatio = (CameraLow.localRotation.eulerAngles.x + xRotation) / angleRange;
        Vector3 position = Vector3.Lerp(CameraLow.position, CameraHigh.position, angleRatio);
        PlayerCamera.transform.position = InFrontOfObscurances(position);
        PlayerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private Vector3 InFrontOfObscurances(Vector3 positon)
    {
        Vector3 toCamera = positon - transform.position;
        return Physics.Raycast(transform.position, toCamera, out RaycastHit raycastHit, toCamera.magnitude, ~1 << gameObject.layer) ? raycastHit.point : positon;
    }

    private void DebugMove()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float z = Input.GetAxis("Jump") - Input.GetAxis("Crouch");
        Vector3 move = transform.right * x + transform.forward * y + transform.up * z;
        CharacterController.Move(move * Speed * 10f);
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * y;
        CharacterController.Move(move * Speed);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -2 * Gravity);
            lastJump = Time.time;
        }
    }

    private void Jetpack()
    {
        if (Input.GetButton("Jump") && !isGrounded && lastJump + JetpackDelay < Time.time)
            velocity.y -= 2 * Gravity * Time.deltaTime;
    }

    private void Fall()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        if (isGrounded && velocity.y <= 0)
            velocity.y = Gravity;
        velocity.y += Gravity * Time.deltaTime;
        velocity.y = Mathf.Min(velocity.y, MaxSpeed);
        CharacterController.Move(velocity * Time.deltaTime);
    }

}
