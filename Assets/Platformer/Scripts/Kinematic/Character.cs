using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Platformer.Kinematic
{
    public class Character : MonoBehaviour
    {
        [Header("Movement")]
        public float HorizontalInput;
        [SerializeField] private Movement movement;
        [SerializeField] private float speed = 5;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private bool isFacingRight;
        [SerializeField] private float jumpPower = 6.5f;
        [SerializeField] private float jumpCount;
        [SerializeField] private bool isLanding;
        public bool WantToJump;

        [Header("Collision")]
        [SerializeField] private Collider2D bodyCollider;
        [SerializeField] private CollisionDetector downDetector;
        [SerializeField] private CollisionDetector leftDetector;
        [SerializeField] private CollisionDetector rightDetector;
        [SerializeField] private CollisionDetector upDetector;
        [SerializeField] private float safeDistance = 0.1f;
        [SerializeField] private int separateIteration = 5;
        [SerializeField] private float separateOffsetPerIteration = 0.02f;
        private ContactFilter2D filter2D;
        [SerializeField] private LayerMask filterMask;
        [SerializeField] private bool isGrounded;
        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public bool IsFacingRight
        {
            get => isFacingRight;
            set
            {
                isFacingRight = value;
                // TODO flip sprite
                if (isFacingRight)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
            }
        }

        private void OnEnable() {
        }

        private void Start()
        {
            filter2D = new ContactFilter2D();
            filter2D.useLayerMask = true;
            filter2D.layerMask = filterMask;
        }

        private void Update()
        {
            HandleFacing();
        }

        private void FixedUpdate()
        {
            var lastGrounded = isGrounded;

            isGrounded = downDetector.SeperateOnCollision(
                Vector2.down, safeDistance,
                filter2D,
                movement, separateIteration, separateOffsetPerIteration
            );

            if (isGrounded && !lastGrounded)
            {
                HandleOnLand();
            }

            if (isGrounded)
            {
                movement.Velocity = new Vector2(movement.Velocity.x, 0);
            }

            // move or stop with friction
            if (Mathf.Abs(HorizontalInput) > 0.01f)
            {
                MoveHorizontally();
                if (isGrounded && !isLanding)
                    animator.Play("Move");
            }
            else
            {
                var currentSpeed = movement.Velocity.x;
                var newSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
                movement.Velocity = new Vector2(newSpeed, movement.Velocity.y);
            }

            // to idle
            if (isGrounded && Mathf.Abs(movement.Velocity.x) < 0.01f && !isLanding)
            {
                animator.Play("Idle");
            }

            // jump and air
            HandleJumpAndAir();

            // separate from walls
            SeperateFromWalls();

            movement.PerformMovement();
        }

        void HandleOnLand()
        {
            animator.Play("Land");
            isLanding = true;
            StartCoroutine(LandingCheck());
        }

        void HandleJumpAndAir()
        {
            if (isGrounded)
            {
                jumpCount = 0;
            }
            else
            {
                movement.ApplyGravity();
            }

            if (jumpCount < 2 && WantToJump)
            {
                Jump();
                jumpCount++;
                WantToJump = false;
            }
        }

        void SeperateFromWalls()
        {
            if (movement.Velocity.x < 0)
            {
                if (leftDetector.SeperateOnCollision(
                    Vector2.left, Mathf.Abs(movement.Velocity.x) * Time.fixedDeltaTime + safeDistance,
                    filter2D,
                    movement, separateIteration, separateOffsetPerIteration
                ))
                {
                    movement.Velocity = new Vector2(0, movement.Velocity.y);
                }
            }
            else if (movement.Velocity.x > 0)
            {
                if (rightDetector.SeperateOnCollision(
                    Vector2.right, movement.Velocity.x * Time.fixedDeltaTime + safeDistance,
                    filter2D,
                    movement, separateIteration, separateOffsetPerIteration
                ))
                {
                    movement.Velocity = new Vector2(0, movement.Velocity.y);
                }
            }

            if (movement.Velocity.y > 0 && upDetector.SeperateOnCollision(
                Vector2.up, movement.Velocity.y * Time.fixedDeltaTime + safeDistance,
                filter2D,
                movement, separateIteration, separateOffsetPerIteration
            ))
            {
                movement.Velocity = new Vector2(movement.Velocity.x, 0);
            }
        }

        void Jump()
        {
            animator.Play("Jump");
            movement.Velocity = new Vector2(movement.Velocity.x, jumpPower);
        }

        IEnumerator LandingCheck()
        {
            yield return null;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (stateInfo.IsName("Land") && stateInfo.normalizedTime < 0.95f)
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
            isLanding = false;
        }


        void MoveHorizontally()
        {
            var targetSpeed = HorizontalInput * speed;
            var newSpeed = Mathf.MoveTowards(movement.Velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            movement.Velocity = new Vector2(newSpeed, movement.Velocity.y);
        }

        void HandleFacing()
        {
            if (HorizontalInput > 0)
            {
                IsFacingRight = true;
            }
            else if (HorizontalInput < 0)
            {
                IsFacingRight = false;
            }
        }
    }
}

