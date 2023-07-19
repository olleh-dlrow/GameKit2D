using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Kinematic
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Character character;
        private Coroutine wantToJumpTimeoutCoroutine;

        private void Update() {
            character.HorizontalInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                character.WantToJump = true;
                if(wantToJumpTimeoutCoroutine != null)
                {
                    StopCoroutine(wantToJumpTimeoutCoroutine);
                }
                wantToJumpTimeoutCoroutine = StartCoroutine(WantToJumpTimeout());
            }
        }

        private void FixedUpdate() {

        }

        IEnumerator WantToJumpTimeout()
        {
            yield return new WaitForSeconds(0.1f);
            character.WantToJump = false;
        }
    }
}




