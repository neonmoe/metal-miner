using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour {
        public GameInput GameInput;
        public Transform HeadingTransform;
        public bool IsGrounded = false;
        private CharacterController CharacterController;

        private float CameraYaw = 0;
        private float CameraPitch = 0;
        private Vector3 GravitationalMovementVector;
        private float GroundedTime = 0;
        private bool Grounded { get { return Time.time - GroundedTime < 0.25f; } }
        private Vector3 GroundMovementVector;

        private void Awake() {
            CharacterController = GetComponent<CharacterController>();
        }

        private void Update() {
            CameraPitch = Mathf.Clamp(CameraPitch + GameInput.GetCharacterPitchDelta(), -89, 89);
            CameraYaw += GameInput.GetCharacterYawDelta();
            HeadingTransform.localEulerAngles = new Vector3(CameraPitch, CameraYaw, 0);

            Vector3 GroundMovementDirection = GameInput.GetCharacterMovementVector();
            Vector3 GroundNormal = GetGroundNormal();
            GroundMovementDirection = HeadingTransform.TransformDirection(GroundMovementDirection);
            // Ensure we point in a walkable direction
            GroundMovementDirection.y = 0;
            GroundMovementDirection.Normalize();
            // Project onto ground
            GroundMovementDirection -= GroundNormal * Vector3.Dot(GroundMovementDirection, GroundNormal);
            GroundMovementDirection.Normalize();
            // Handle friction
            float TargetSpeed = 7.0f;
            float CurrentSpeed = GroundMovementVector.magnitude;
            float Friction = Mathf.Min(CurrentSpeed, 8.0f * Mathf.Max(CurrentSpeed, 3.0f) * Time.deltaTime);
            GroundMovementVector -= Friction * GroundMovementVector.normalized;
            // Handle accel
            CurrentSpeed = Vector3.Dot(GroundMovementDirection, GroundMovementVector);
            float Accel = Mathf.Min(10.0f * TargetSpeed * Time.deltaTime, TargetSpeed - CurrentSpeed);
            GroundMovementVector += Accel * GroundMovementDirection;

            if (Grounded) {
                if (Vector3.Dot(GravitationalMovementVector, GroundNormal) < 0) {
                    GravitationalMovementVector = -0.1f * GroundNormal;
                }
                if (GameInput.IsCharacterJumping()) {
                    GravitationalMovementVector = Vector3.up * 5f;
                }
            } else {
                GravitationalMovementVector += Physics.gravity * Time.deltaTime;
            }

            CharacterController.Move(Time.deltaTime * (GravitationalMovementVector + GroundMovementVector));
            if (CharacterController.isGrounded) {
                GroundedTime = Time.time;
            }
            IsGrounded = Grounded;
        }

        private Vector3 GetGroundNormal() {
            float HalfHeightAndOffset = CharacterController.height / 2 + CharacterController.skinWidth * 0.2f - CharacterController.radius;
            Vector3 Origin = CharacterController.transform.position + CharacterController.center - HalfHeightAndOffset * Vector3.up;
            RaycastHit Hit;
            if (Physics.SphereCast(Origin, CharacterController.radius, -Vector3.up, out Hit, CharacterController.skinWidth)) {
                Debug.DrawLine(Origin, Origin + Hit.normal, Color.red);
                return Hit.normal;
            }
            return Vector3.up;
        }
    }
}
