using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private new Rigidbody rigidbody;

    private PlayerActions _playerActions;
    private Camera _camera;

    private Vector3 _lastDirection;

    private void Awake()
    {
        _camera = Camera.main;
        _playerActions = new PlayerActions();
        if (!rigidbody) rigidbody.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _lastDirection = transform.forward;
    }

    private void OnEnable()
    {
        _playerActions.Ground.Enable();
    }

    private void FixedUpdate()
    {
        MoveRelativeToCamera();
    }

    private void MoveRelativeToCamera()
    {
        // Get player input
        var inputDirection = _playerActions.Ground.Move.ReadValue<Vector2>();

        // Get camera normalised forward and right vectors
        var cameraTransform = _camera.transform;
        var cameraForward = cameraTransform.forward;
        var cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // (Copilot) var moveDirection = cameraForward * inputDirection.y + cameraRight * inputDirection.x;

        var forwardRelativeVerticalInput = cameraForward * inputDirection.y;
        var rightRelativeHorizontalInput = cameraRight * inputDirection.x;

        var cameraRelativeDirection = forwardRelativeVerticalInput + rightRelativeHorizontalInput;

        var moveDirection = Vector3.ClampMagnitude(cameraRelativeDirection, 1f);
        rigidbody.velocity = moveDirection * speed + Vector3.up * rigidbody.velocity.y;

        if (moveDirection != Vector3.zero)
            _lastDirection = moveDirection;

        transform.forward = Vector3.Slerp(transform.forward, _lastDirection, turnSpeed);
    }
}
