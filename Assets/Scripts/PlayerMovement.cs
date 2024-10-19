using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [field: SerializeField] public PlayerData PlayerData { get; private set; }
    [SerializeField] private Transform orientation;
    private CharacterController characterController;
    private PlayerDashing playerDashing;
    private LayerMask whatIsGround;
    private float speed;
    private bool hasLanded;
    private bool isGrounded;
    private Vector3 moveVector;
    private float gravity = -1f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerDashing = GetComponent<PlayerDashing>();
        whatIsGround = ~LayerMask.GetMask("Player");
        speed = PlayerData.walkSpeed;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        isGrounded = Physics.CheckBox(transform.position, new Vector3(0.3f, 0.1f, 0.3f), Quaternion.identity, whatIsGround);
        speed = Mathf.Lerp(speed, GetSpeed(), 10f * Time.deltaTime);

        ApplyGravity();

        moveVector = GetMoveInput();
        moveVector *= speed;
        moveVector = Vector3.ClampMagnitude(moveVector, speed);
        moveVector.y = gravity;

        characterController.Move(orientation.TransformDirection(moveVector * Time.deltaTime));
    }

    private void ApplyGravity()
    {
        if (isGrounded)
        {
            if (!hasLanded)
            {
                gravity = -1f;
                hasLanded = true;
            }
        }
        else
        {
            hasLanded = false;
            gravity += Physics.gravity.y * Time.deltaTime;
        }
    }

    private Vector3 GetMoveInput()
    {
        if (CanBlockMoveInput())
        {
            return moveVector;
        }

        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    public bool IsMoving() => new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude > 0.1f && isGrounded;

    public bool CanBlockMoveInput() => playerDashing.isDashing;

    private float GetSpeed() => playerDashing.isDashing ? PlayerData.dashSpeed : PlayerData.walkSpeed;

    public void OnDash()
    {
        moveVector = GetMoveInput();
    }

    public Vector3 GetMoveVector() => moveVector;
}