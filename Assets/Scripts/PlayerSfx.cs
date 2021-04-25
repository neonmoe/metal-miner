using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerSfx : MonoBehaviour {
        public Transform HeadTransform;
        public AudioSource WindOutsideSource;
        public AudioSource WindInsideSource;
        private CharacterController CharacterController;

        private void Awake() {
            CharacterController = GetComponent<CharacterController>();
        }

        private void Update() {
            float Openness = 0f;
            Openness += GetRaycastLength(Vector3.up, 2.0f);
            Openness += GetRaycastLength(Vector3.forward, 1.0f);
            Openness += GetRaycastLength(Vector3.back, 1.0f);
            Openness += GetRaycastLength(Vector3.right, 1.0f);
            Openness += GetRaycastLength(Vector3.left, 1.0f);
            Openness = HeadTransform.position.y > 0 ? Mathf.Pow(Openness / 6.0f, 4.0f) : 0.0f;
            float WindVolume = Mathf.Clamp(Mathf.Pow(HeadTransform.position.y / 5.0f, 3.0f), -0.5f, 0.5f) + 0.5f;
            WindOutsideSource.volume = Mathf.Lerp(WindOutsideSource.volume,
                                                  Openness * WindVolume,
                                                  10f * Time.deltaTime);
            WindInsideSource.volume = Mathf.Lerp(WindInsideSource.volume,
                                                  (1.0f - Openness) * WindVolume,
                                                  10f * Time.deltaTime);
        }

        private float GetRaycastLength(Vector3 direction, float length) {
            RaycastHit Hit;
            if (Physics.Raycast(HeadTransform.position, direction, out Hit, length)) {
                return Hit.distance;
            }
            return length;
        }
    }
}
