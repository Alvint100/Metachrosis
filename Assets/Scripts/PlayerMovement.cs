using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Walk")]
    public float walkSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float gravity = -20f;
    public float coyoteTimeWindow = 0.15f;
    public float preInputJumpWindow = 0.15f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 1f;

    private CharacterController _cc;
    private Vector3 _velocity;
    private bool _isDashing;
    private bool _usedAirDash;
    

    //these countdown from the "max" values above
    // use these for checking if we're in the time window for our jump forgiveness etc.
    private float _coyoteTimer;
    private float _preInputJumpTimer;
    private float _dashCooldownTimer;

    //we set up the world wierd so we just have to deal with the mismatch of directions, don't edit these values!!
    private static Vector3 _forwardAxis = Vector3.right;
    private static Vector3 _rightAxis   = -Vector3.forward;

    private float _attackSpeedMultiplier = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.Instance != null && PauseMenu.Instance.isPaused)
            return;

        if (_isDashing)
            return;

        HandleTimers();
        HandleWalk();
        HandleJump();
        HandleDash();
        ApplyGravity();

    }

    Vector3 GetInputDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return (_forwardAxis * v + _rightAxis * h).normalized;
    }

    public Vector3 CurrentMoveDirection
    {
        get { return GetInputDirection(); }
    }

    public bool IsDashing
    {
        get { return _isDashing; }
    }

    void HandleWalk()
    {
        Vector3 move = GetInputDirection();
       // Debug.Log("it is all over");
        _cc.Move(move * walkSpeed * _attackSpeedMultiplier * Time.deltaTime);


        //if (move != Vector3.zero) {
        //    transform.rotation = Quaternion.Slerp(
        //        transform.rotation,
        //        Quaternion.LookRotation(move, Vector3.up),
        //        20f * Time.deltaTime);
        //}
    }
    void HandleJump()
    {
        bool grounded = _cc.isGrounded;

        if (grounded)
        {
            _coyoteTimer = coyoteTimeWindow;
            _usedAirDash = false;
            //if (_velocity.y < 0f)
            //    _velocity.y = -2f;
        } else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            _preInputJumpTimer = preInputJumpWindow;
        }

        if (_preInputJumpTimer > 0f)
        {
            if (_coyoteTimer > 0f)
            {
                _velocity.y = jumpForce;
                _preInputJumpTimer = 0f;
                _coyoteTimer = 0f;
            }
        }
    }

    void HandleDash()
    {
        if ((_dashCooldownTimer > 0f) || (_usedAirDash && !_cc.isGrounded))
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Vector3 dashDir = GetDashDirection();
            StartCoroutine(DashRoutine(dashDir));
        }
    }

    Vector3 GetDashDirection()
    {
        Vector3 input = GetInputDirection();

        if (input.sqrMagnitude > 0.01f)
            return input.normalized;

        Vector3 backward = -transform.forward;
        backward.y = 0f;
        return backward.normalized;
    }

    IEnumerator DashRoutine(Vector3 dir)
    {
        _isDashing = true;

        if (!_cc.isGrounded)
            _usedAirDash = true;

        float elapsed = 0f;
        float savedY  = _velocity.y;
        _velocity.y   = 0f;

        while (elapsed < dashDuration)
        {
            if (PauseMenu.Instance != null && PauseMenu.Instance.isPaused) {
                yield return null;
                continue;
            }
            _cc.Move(dir * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _velocity.y = savedY;
        _isDashing = false;
        _dashCooldownTimer = dashCooldown;
    }

    void ApplyGravity()
    {
        if (_cc.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    void HandleTimers()
    {
        if (_dashCooldownTimer > 0f)
            _dashCooldownTimer -= Time.deltaTime;

        if (_preInputJumpTimer  > 0f)
            _preInputJumpTimer  -= Time.deltaTime;
    }

    public void SetAttackSpeedMultiplier(float multiplier)
    {
        _attackSpeedMultiplier = multiplier;
    }


}