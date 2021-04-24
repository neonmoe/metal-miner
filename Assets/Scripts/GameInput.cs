using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    public class GameInput : MonoBehaviour {
        public bool InvertMouseY;
        private bool MouseLocked = false;

        private void Update() {
            if (Input.GetButtonDown("Fire1")) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                MouseLocked = true;
            }
            if (Input.GetKeyDown("escape")) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                MouseLocked = false;
            }
        }

        public Vector3 GetCharacterMovementVector() {
            return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        public float GetCharacterPitchDelta() {
            return MouseLocked ? (Input.GetAxis("Mouse Y") * 1.5f * (InvertMouseY ? 1 : -1)) : 0;
        }

        public float GetCharacterYawDelta() {
            return MouseLocked ? (Input.GetAxis("Mouse X") * 1.5f) : 0;
        }
    }
}
