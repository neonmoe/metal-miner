using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour {
        public GameInput GameInput;
        public Transform HeadingTransform;
        private CharacterController CharacterController;

        private float CameraYaw = 0;
        private float CameraPitch = 0;
        private Vector3 GravitationalMovementVector;

        private void Awake() {
            CharacterController = GetComponent<CharacterController>();
        }

        private void Update() {
            CameraPitch = Mathf.Clamp(CameraPitch + GameInput.GetCharacterPitchDelta(), -90, 90);
            CameraYaw += GameInput.GetCharacterYawDelta();
            HeadingTransform.localEulerAngles = new Vector3(CameraPitch, CameraYaw, 0);

            Vector3 GroundMovementVector = GameInput.GetCharacterMovementVector();
            Vector3 GroundNormal = GetGroundNormal();
            GroundMovementVector = HeadingTransform.TransformDirection(GroundMovementVector);
            GroundMovementVector -= GroundNormal * Vector3.Dot(GroundMovementVector, GroundNormal);
            float MoveSpeed = 5.0f;

            CharacterController.Move(Time.deltaTime * GroundMovementVector * MoveSpeed);

            if (CharacterController.isGrounded) {
                GravitationalMovementVector = Vector3.zero;
            } else {
                GravitationalMovementVector += Physics.gravity * Time.deltaTime;
            }

            CharacterController.Move(Time.deltaTime * GravitationalMovementVector);
        }

        private Vector3 GetGroundNormal() {
            RaycastHit Hit;
            if (Physics.Raycast(CharacterController.center - CharacterController.height * Vector3.up, -Vector3.up, out Hit, CharacterController.skinWidth)) {
                return Hit.normal;
            }
            return Vector3.up;
        }
    }
}
