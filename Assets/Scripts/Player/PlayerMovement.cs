using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Movement
    public float moveSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;

    private float currentspeed;

    // Ground Check
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // Crouch
    public float crouchHeight = 1f;
    public float standingheight = 2f; // original height of Player

    // Shove
    public float shoveForce = 3f;  // Knocks force
    public float shoveRadius = 3f;  // How far out the shove reach
    public float shoveAngle = 60f;  // Width of player shove cone
    public LayerMask zombieLayer;   // Zombie Layer here

    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private Transform mainCameraTransform; // Tracks where the player looks

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        if(Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        // Start game at nomral walking speed
        currentspeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        checkGround();
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            capsuleCollider.height = crouchHeight;
            currentspeed = crouchSpeed;
        }
        else
        {
            capsuleCollider.height = standingheight;
            currentspeed = moveSpeed;
        }
    }

    void OnShove()
    {
        if (mainCameraTransform == null) return;

        // Find all colllider within the shove radius
        Collider[] hitColliders = Physics.OverlapSphere(mainCameraTransform.position, shoveRadius, zombieLayer);

        // Calculate direction from player to the zombie
        foreach (var hitCollider in hitColliders)
        {

            Vector3 directionTozombie = (hitCollider.transform.position - mainCameraTransform.position).normalized;

            // Compare against camera's looking direction
            float angle = Vector3.Angle(mainCameraTransform.forward, directionTozombie);

            if(angle <= shoveAngle * 0.5f)
            {
                // Find zombie script
                var zombie = hitCollider.GetComponent<Enemy>();

                if (zombie != null)
                {
                    // Calcuate horizontal push direction away from player
                    Vector3 shoveDirection = hitCollider.transform.position - transform.position;
                    shoveDirection.y = 0;
                    shoveDirection.Normalize();

                    // Trigger stumble rotine on zombie
                    zombie.Stumble(shoveDirection, shoveForce);
                }
            }
        }
    }

    void checkGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction.Normalize();
        rb.linearVelocity = new Vector3(direction.x * currentspeed, rb.linearVelocity.y, direction.z * currentspeed);
    }
}
