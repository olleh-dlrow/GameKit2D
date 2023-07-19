using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Platformer.Kinematic
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] private Collider2D colliderToCast;
        public Collider2D ColliderToCast => colliderToCast;

        [SerializeField] private float castDistance = 0.03f;
        private Dictionary<Collider2D, Coroutine> collisionCoroutines = new Dictionary<Collider2D, Coroutine>();
        public RaycastHit2D[] CollisionResults;
        public bool CheckCollisionIn(Vector2 direction)
        {
            CollisionResults = new RaycastHit2D[3];
            return colliderToCast.Cast(direction, CollisionResults, castDistance) > 0;
        }
        public bool CheckCollisionIn(Vector2 direction, float distance)
        {
            CollisionResults = new RaycastHit2D[3];
            return colliderToCast.Cast(direction, CollisionResults, distance) > 0;
        }

        public bool SeperateOnCollision(Vector2 direction, float distance,
        ContactFilter2D filter2D,
        Movement sepTarget, int iteration, float offsetPerIteration)
        {
            direction = direction.normalized;
            bool hasCollsionThisDirection = false;
            CollisionResults = new RaycastHit2D[5];
            var collisionCount = colliderToCast.Cast(direction, filter2D, CollisionResults, distance);
            if (collisionCount > 0)
            {
                collisionCount = Mathf.Min(collisionCount, CollisionResults.Length);
                for (int i = 0; i < collisionCount; i++)
                {
                    var collisionResult = CollisionResults[i];
                    var otherCollider = collisionResult.collider;

                    ColliderDrawer.Instance?.AddBoxCollider2D(otherCollider as BoxCollider2D);

                    var normal = collisionResult.normal;
                    var projection = Vector2.Dot(normal, direction) * direction;
                    projection = projection.normalized;
                    if (projection == Vector2.zero || projection != -direction)
                    {
                        continue;
                    }
                    else
                    {
                        hasCollsionThisDirection = true;
                    }

                    if (!colliderToCast.IsTouching(otherCollider))
                    {
                        continue;
                    }
                    else
                    {
                        // seperate
                        if (collisionCoroutines.TryGetValue(otherCollider, out var coroutine))
                        {
                            if (coroutine != null)
                                StopCoroutine(coroutine);
                        }
                        collisionCoroutines[otherCollider] =
                        StartCoroutine(
                            SeperateOnCollisionCoroutine(otherCollider, projection,
                            sepTarget, iteration, offsetPerIteration));
                    }

                }
            }
            return hasCollsionThisDirection;
        }

        IEnumerator SeperateOnCollisionCoroutine(Collider2D otherCollider,
        Vector2 projection,
        Movement sepTarget,
        int iteration, float offsetPerIteration)
        {
            for (int j = 0; j < iteration; j++)
            {
                sepTarget.ForceMovement((Vector3)projection * offsetPerIteration);
                if (!colliderToCast.IsTouching(otherCollider))
                {
                    yield break;
                }
                else
                {
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }
}


