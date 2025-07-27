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
            _animator.SetFloat("yVelocity", _rb.linearVelocity.y);
        }
    }

    private void FlipSprite()
    {
        if (_isFacingRight && _horizontalInput < 0f || !_isFacingRight && _horizontalInput > 0f)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
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

    
}
