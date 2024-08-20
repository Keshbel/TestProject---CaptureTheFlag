using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private static JoystickContainer JoystickContainer => Singleton.Instance.JoystickContainer;
    
    [field: Header("Settings")] 
    
    [field: SerializeField] 
    public float Speed { get; private set; } = 10f;
    
    [field: SerializeField] 
    public float JumpForce { get; private set; } = 300f;
    
    private bool _isGrounded;
    private Rigidbody _rb;

    private Vector3 movementVector
    {
        get
        {
            var horizontal = JoystickContainer.JoystickMovement.Horizontal;
            var vertical = JoystickContainer.JoystickMovement.Vertical;

            var move = new Vector3(horizontal, 0, vertical);
            if (move.sqrMagnitude > 1) move.Normalize();

            return move;
        }
    }

    #region Unity Events
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    
    [Client]
    private void OnCollisionEnter(Collision collision)
    {
        IsGroundedUpdate(collision, true);
    }

    [Client]
    private void OnCollisionExit(Collision collision)
    {
        IsGroundedUpdate(collision, false);
    }

    [Client]
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        
        Movement();
        Jump();
    }
    
    #endregion


    #region Private Methods
    
    private void Movement()
    {
        var moveVector = movementVector;
        
        if (moveVector == Vector3.zero) return;
        
        _rb.AddForce(moveVector * Speed, ForceMode.Impulse);
        _rb.MoveRotation(Quaternion.LookRotation(moveVector));
    }

    private void Jump()
    {
        if (_isGrounded && Input.GetAxis("Jump") > 0) 
            _rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    private void IsGroundedUpdate(Collision collision, bool value)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
            _isGrounded = value;
    }
    
    #endregion
}
