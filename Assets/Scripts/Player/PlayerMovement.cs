using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private float _horizontalInput;
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _jumpPower = 5f;
    private bool _isFacingRight = false;
    private bool _isGrounded = false;

    // private bool isCurrentlyMoving = false; // Unused field - removed to fix warning
    private bool _movementEnabled = true;
    private Rigidbody2D _rb;

    private Animator _animator;



    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

    }
    private void Update()
    {
        if (!_movementEnabled) return;
        
        _horizontalInput = Input.GetAxis("Horizontal");
        FlipSprite();

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpPower);
            _isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        if (!_movementEnabled) return;
        
        _rb.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rb.linearVelocity.y);
        
        // Only set animation parameters if they exist
        if (_animator != null)
        {
            _animator.SetFloat("xVelocity", Math.Abs(_rb.linearVelocity.x));
            // Only set yVelocity if the parameter exists
            if (HasParameter("yVelocity", _animator))
            {
                _animator.SetFloat("yVelocity", _rb.linearVelocity.y);
            }
        }
    }

    private void FlipSprite()
    {
        // Flip sprite based on movement direction
        if (_horizontalInput > 0f && !_isFacingRight)
        {
            // Moving right, flip to face right
            _isFacingRight = true;
            Vector3 ls = transform.localScale;
            ls.x = Mathf.Abs(ls.x); // Make sure it's positive (facing right)
            transform.localScale = ls;
        }
        else if (_horizontalInput < 0f && _isFacingRight)
        {
            // Moving left, flip to face left
            _isFacingRight = false;
            Vector3 ls = transform.localScale;
            ls.x = -Mathf.Abs(ls.x); // Make sure it's negative (facing left)
            transform.localScale = ls;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isGrounded = true;
    }

    public void StopMovement()
    {
        _movementEnabled = false;
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        
        // Only set animation parameters if they exist
        if (_animator != null)
        {
            _animator.SetFloat("xVelocity", 0);
        }
    }

    public void ResumeMovement()
    {
        _movementEnabled = true;
    }

    // Helper method to check if animator parameter exists
    private bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}
