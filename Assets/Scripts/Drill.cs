using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    public class Drill : MonoBehaviour {
        public GameInput GameInput;
        public ChunkSpawner ChunkSpawner;
        public Transform TargetingTransform;
        public Transform DrillBitTransform;
        public Transform DrillBitRootTransform;
        public AudioSource DrillAudio;
        public AudioSource ActiveDrillAudio;
        public Animator Animator;
        public ParticleSystem DrillingParticleSystem;
        [Header("Drill size")]
        public int TilePickingRadius = 1;
        public float TileDistanceCutoff = 1.5f;
        [Header("Mineral deposit mechanic")]
        public GameObject MineralDepositNote;
        public GameObject MineralPrefab;
        public Transform MineralLauncherTransform;
        public AudioSource BopAudio;

        private Vector3 LerpedPosition;
        private Vector3 TargetPosition;

        private void Start() {
            LerpedPosition = DrillBitTransform.position;
        }

        private void Update() {
            float DrillDistance = 3.5f;
            Vector3 DrillPoint = TargetingTransform.position + TargetingTransform.forward * (DrillDistance - 0.7f);
            bool CanDrill = false;
            MineralDepositNote.SetActive(false);

            RaycastHit Hit;
            if (Physics.Raycast(TargetingTransform.position, TargetingTransform.forward, out Hit, DrillDistance)) {
                CanDrill = true;
                DrillPoint = Hit.point - TargetingTransform.forward * 0.2f;

                if (Hit.collider.name == "MineralShop" && Chunk.MINERALS > 0) {
                    MineralDepositNote.SetActive(true);
                    if (GameInput.IsCharacterDepositing()) {
                        GameObject SpawnedMineral = Instantiate<GameObject>(MineralPrefab);
                        SpawnedMineral.transform.position = MineralLauncherTransform.position;
                        Rigidbody MineralBody = SpawnedMineral.GetComponent<Rigidbody>();
                        MineralBody.AddForce(MineralLauncherTransform.forward * 20f, ForceMode.VelocityChange);
                        BopAudio.Play();
                        Chunk.MINERALS--;
                    }
                }
            }

            Vector3 DrillAngles = DrillBitRootTransform.localEulerAngles;
            Vector3 DrillScale = DrillBitRootTransform.localScale;
            if (CanDrill && GameInput.IsCharacterDrilling()) {
                DrillAudio.volume = Mathf.Lerp(DrillAudio.volume, 1.0f, 15f * Time.deltaTime);
                ActiveDrillAudio.volume = Mathf.Lerp(ActiveDrillAudio.volume, 1.0f, 15f * Time.deltaTime);
                Animator.SetBool("Drilling", true);
                TargetPosition = DrillPoint;
                DrillAngles.z += 1440f * Time.deltaTime;
                DrillScale = Vector3.Lerp(DrillScale, Vector3.one * 2f, 8f * Time.deltaTime);
            } else {
                DrillAudio.volume = Mathf.Lerp(DrillAudio.volume, 0.3f, 5f * Time.deltaTime);
                ActiveDrillAudio.volume = Mathf.Lerp(ActiveDrillAudio.volume, 0.0f, 5f * Time.deltaTime);
                Animator.SetBool("Drilling", false);
                LerpedPosition = DrillBitTransform.position;
                TargetPosition = DrillBitRootTransform.position;
                DrillAngles.z += 720f * Time.deltaTime * (0.5f * ((int)(Time.time * 30.0f) % 4));
                DrillScale = Vector3.Lerp(DrillScale, Vector3.one, 15f * Time.deltaTime);
            }
            DrillBitRootTransform.localEulerAngles = DrillAngles;
            DrillBitRootTransform.localScale = DrillScale;
            LerpedPosition = Vector3.Lerp(LerpedPosition, TargetPosition, 10f * Time.deltaTime);
            DrillBitTransform.position = LerpedPosition;
            DrillingParticleSystem.transform.position = DrillBitTransform.position;

            bool AtDrillingPosition = (TargetPosition - DrillPoint).magnitude < 0.1f;
            if (CanDrill && AtDrillingPosition && !DrillingParticleSystem.isPlaying) {
                DrillingParticleSystem.Play();
            } else if (DrillingParticleSystem.isPlaying && (!CanDrill || !AtDrillingPosition)) {
                DrillingParticleSystem.Stop();
            }

            if (CanDrill) {
                float TileSize = 1f / 2f;
                int Radius = TilePickingRadius;
                Vector3 EffectiveDrillingPoint = DrillPoint + TargetingTransform.forward * 0.3f;
                Chunk Chunk = ChunkSpawner.GetChunkAt(EffectiveDrillingPoint);
                for (int OffZ = -Radius; OffZ <= Radius; OffZ++) {
                    for (int OffY = -Radius; OffY <= Radius; OffY++) {
                        for (int OffX = -Radius; OffX <= Radius; OffX++) {
                            float Distance = Mathf.Sqrt(OffX * OffX + OffY * OffY + OffZ * OffZ);
                            if (Distance > TileDistanceCutoff) continue;

                            float TileX = Mathf.Floor(EffectiveDrillingPoint.x / TileSize) * TileSize + OffX * TileSize;
                            float TileY = Mathf.Floor(EffectiveDrillingPoint.y / TileSize) * TileSize + OffY * TileSize;
                            float TileZ = Mathf.Floor(EffectiveDrillingPoint.z / TileSize) * TileSize + OffZ * TileSize;
                            Vector3[] Points = new Vector3[] {
                                new Vector3(TileX + TileSize, TileY + TileSize, TileZ + TileSize),
                                new Vector3(TileX + TileSize, TileY + TileSize, TileZ),
                                new Vector3(TileX, TileY + TileSize, TileZ),
                                new Vector3(TileX, TileY + TileSize, TileZ + TileSize),
                                new Vector3(TileX + TileSize, TileY, TileZ + TileSize),
                                new Vector3(TileX + TileSize, TileY, TileZ),
                                new Vector3(TileX, TileY, TileZ),
                                new Vector3(TileX, TileY, TileZ + TileSize),
                            };
                            for (int i = 0; i < Points.Length; i++) {
                                for (int j = 0; j < Points.Length; j++) {
                                    Debug.DrawLine(Points[i], Points[j], Color.red);
                                }
                            }
                            Debug.DrawLine(TargetingTransform.position, DrillPoint, Color.red);
                            Debug.DrawLine(TargetingTransform.position, DrillBitTransform.position, Color.green);

                            if (AtDrillingPosition && GameInput.IsCharacterDrilling()) {
                                float Damage = Time.deltaTime * 1.2f;
                                Chunk.DamageTile(new Vector3(TileX + TileSize / 2f, TileY + TileSize / 2f, TileZ + TileSize / 2f), Damage);
                            }
                        }
                    }
                }
            }
        }
    }
}
