using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Platformer
{
    public class ColliderDrawer : MonoBehaviour
    {
        public List<BoxCollider2D> boxCollider2Ds = new List<BoxCollider2D>();
        private static ColliderDrawer instance;
        public static ColliderDrawer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ColliderDrawer>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void AddBoxCollider2D(BoxCollider2D boxCollider2D)
        {
            boxCollider2Ds.Add(boxCollider2D);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach(BoxCollider2D boxCollider2D in boxCollider2Ds)
            {
                Handles.Label(boxCollider2D.bounds.center, LayerMask.LayerToName(boxCollider2D.gameObject.layer),
                new GUIStyle()
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState()
                    {
                        textColor = Color.red
                    }
                });
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(boxCollider2D.bounds.center, boxCollider2D.bounds.size);
            }

            boxCollider2Ds.Clear();
        }
    }
#endif
}