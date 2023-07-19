using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Kinematic
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2d;

        [SerializeField] private Vector2 velocity;

        public Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        private void Start()
        {
            Debug.Assert(rb2d != null, "Rigidbody can't be null!", gameObject);
            rb2d.isKinematic = true;
        }

        public void ApplyGravity()
        {
            //gravity has NEGATIVE y value
            velocity += Physics2D.gravity * Time.fixedDeltaTime;
        }

        public void PerformMovement()
        {
            rb2d.MovePosition(rb2d.position + velocity * Time.fixedDeltaTime);
        }

        public void ForceMovement(Vector2 offset)
        {
            rb2d.position = rb2d.position + offset;
        }
    }
}
