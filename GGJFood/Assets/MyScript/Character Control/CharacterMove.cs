using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class CharacterMove : MonoBehaviour
{
    [Header("Player")]
    public float MoveSpeed = 3.0f;

    [Space(10)]
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.5f;

    [Header("Grounded")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;
    public LayerMask EnemyLayers;  

    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float _jumpTimeoutDelta;

    private CharacterController _controller;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _jumpTimeoutDelta = JumpTimeout;
    }

    private void Update()
    {
        GroundedCheck();
        JumpAndGravity();
        Move();
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y - GroundedOffset,
            transform.position.z
        );

        bool onGround = Physics.CheckSphere(
            spherePosition,
            GroundedRadius,
            GroundLayers,
            QueryTriggerInteraction.Ignore
        );

        bool onEnemy = Physics.CheckSphere(
            spherePosition,
            GroundedRadius,
            EnemyLayers,
            QueryTriggerInteraction.Ignore
        );

        Grounded = onGround || onEnemy;
    }

    private void Move()
    {
#if ENABLE_INPUT_SYSTEM
        float x = Keyboard.current != null
            ? (Keyboard.current.aKey.isPressed ? -1f :
               Keyboard.current.dKey.isPressed ? 1f : 0f)
            : 0f;
#else
        float x = Input.GetAxisRaw("Horizontal");
#endif

        // rotate character
        if (x > 0f)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (x < 0f)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        Vector3 move = new Vector3(x * MoveSpeed, 0f, 0f);

        _controller.Move(
            (move + Vector3.up * _verticalVelocity) * Time.deltaTime
        );
    }

    private void JumpAndGravity()
    {
#if ENABLE_INPUT_SYSTEM
        bool jumpPressed = Keyboard.current != null &&
                           Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        bool jumpPressed = Input.GetButtonDown("Jump");
#endif

        if (Grounded)
        {
            _jumpTimeoutDelta = 0f;

            if (_verticalVelocity < 0f)
                _verticalVelocity = -2f;

            if (jumpPressed)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;
        }

        if (_verticalVelocity < _terminalVelocity)
            _verticalVelocity += Gravity * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Grounded
            ? new Color(0f, 1f, 0f, 0.35f)
            : new Color(1f, 0f, 0f, 0.35f);

        Gizmos.DrawSphere(
            new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z
            ),
            GroundedRadius
        );
    }
}
