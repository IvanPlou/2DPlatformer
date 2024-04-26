using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class BaseUnit : MonoBehaviour
{
    // tuning values for character movement
    [Header("Movement")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _acceleration = 6f;

    // tuning values for character jump
    [Header("Jump")]
    [SerializeField] private float _jumpSpeed = 5f;
    [SerializeField] private int _jumpCount = 2;
    [SerializeField] private float _airControl = 0.1f;
    [SerializeField] private bool _wallJump = true;
    [SerializeField] private string _wallJumpTag = "Ground";

    // tuning values for character grounding check
    [Header("Grounding")]
    [SerializeField] private float _groundCheckDistance = 0.125f;
    [SerializeField] private float _groundCheckOffset = 0.125f;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private LayerMask _groundLayer;

    // unity components (properties)
    protected Rigidbody2D Rigidbody {get; private set;}
    protected SpriteRenderer SpriteRenderer {get; private set;}
    protected Animator Animator {get; private set;}
    protected Collider2D Collider {get; private set;}

    // movement values
    private float _input;
    private bool _isTryingToMove;
    private int _currentJumps;
    private bool _isGrounded;

    protected virtual void Start()
    {
        // get components
        Rigidbody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        Collider = GetComponent<Collider2D>();
    }

    // set horizontal movement input
    protected void SetMoveInput(float input)
    {
        _input = input;
        _isTryingToMove = Mathf.Abs(_input) >= 0.05f;
    }

    // attempt a jump
    protected void TryJump()
    {
        if(_currentJumps < _jumpCount)
        {
            // set velocity based on input and jump speed
            Rigidbody.velocity = new Vector2(_input * Mathf.Abs(Rigidbody.velocity.x), _jumpSpeed);
            Animator.SetTrigger("Jump");

            // add to jump count
            _currentJumps++;
        }
    }

    protected virtual void Update()
    {
        // check if grounded first left then right
        bool grounded = GroundCheck(-_groundCheckOffset) || GroundCheck(_groundCheckOffset);

        // reset jump count if landing
        if (!_isGrounded && grounded)
        {
            _currentJumps = 0;
        }
        _isGrounded = grounded;
        Animator.SetBool("IsGrounded", grounded);
        Animator.SetFloat("Speed", Rigidbody.velocity.magnitude / _speed);

        if(_isTryingToMove) 
        {
            SpriteRenderer.flipX = Rigidbody.velocity.x < 0;
        }
    }

    protected virtual void FixedUpdate()
    {
        // move character
        if(_isGrounded)
        {
            // move or slow using full control
            Move(_input, 1f);
        }
        else if(_isTryingToMove)
        {
            // move using air control
            Move(_input, _airControl);
        }
    }

    // move character using input and control values
    private void Move(float input, float control)
    {
        // calculate acceleration for desired velocity
        float targetVelocity = input * _speed;
        float currentVelocity = Rigidbody.velocity.x;
        float velocityDifference = targetVelocity - currentVelocity;
        float acceleration = velocityDifference * _acceleration * control;

        // move only on horizontal axis
        Rigidbody.AddForce(new Vector2(acceleration, 0f));
    }

    // check for the ground from character position and offset
    private bool GroundCheck(float offset)
    {
        Vector3 originPosition = _raycastOrigin.position;

        originPosition.x += offset;

        RaycastHit2D hit = Physics2D.Raycast(originPosition, Vector2.down, _groundCheckDistance, _groundLayer);
        Debug.DrawRay(originPosition, Vector2.down * _groundCheckDistance, Color.red);

        if(hit.collider != null)
        {
            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer  == _groundLayer) { _currentJumps = 0; }
    }
}