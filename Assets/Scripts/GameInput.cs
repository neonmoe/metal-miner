using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    public class GameInput : MonoBehaviour {
        public GameObject PauseMenu;
        public GameObject PauseMenuNote;
        public bool InvertMouseY { get; set; }
        public float MouseSensitivityX { get; set; }
        public float MouseSensitivityY { get; set; }
        private bool MouseLocked = false;

        private void Start() {
            InvertMouseY = false;
            MouseSensitivityX = 1;
            MouseSensitivityY = 1;
        }

        private void Update() {
            if (Input.GetButtonDown("Fire1") && Input.mousePosition.x > 500) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                MouseLocked = true;
                PauseMenu.SetActive(false);
                PauseMenuNote.SetActive(true);
            }
            if (Input.GetKeyDown("escape")) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                MouseLocked = false;
                PauseMenu.SetActive(true);
                PauseMenuNote.SetActive(false);
            }
        }

        public Vector3 GetCharacterMovementVector() {
            if (!MouseLocked) {
                return Vector3.zero;
            }
            return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        public float GetCharacterPitchDelta() {
            return MouseLocked ? (Input.GetAxis("Mouse Y") * MouseSensitivityY * (InvertMouseY ? 1 : -1)) : 0;
        }

        public float GetCharacterYawDelta() {
            return MouseLocked ? (Input.GetAxis("Mouse X") * MouseSensitivityX) : 0;
        }

        public bool IsCharacterJumping() {
            return MouseLocked && Input.GetButtonDown("Jump");
        }

        public bool IsCharacterDrilling() {
            return MouseLocked && Input.GetButton("Fire1");
        }
    }
}
