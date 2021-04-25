using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    public class ChunkSpawner : MonoBehaviour {
        public GameObject ChunkPrefab;

        private Dictionary<Vector3, GameObject> Chunks = new Dictionary<Vector3, GameObject>();

        private void Update() {
            HashSet<Vector3> ChunksInRange = new HashSet<Vector3>();
            int Radius = 2;
            for (int OffZ = -Radius; OffZ <= Radius; OffZ++) {
                for (int OffY = -Radius; OffY <= Radius; OffY++) {
                    for (int OffX = -Radius; OffX <= Radius; OffX++) {
                        if ((int)Mathf.Sqrt(OffX * OffX + OffY * OffY + OffZ * OffZ) > Radius) {
                            continue;
                        }

                        Vector3 ChunkVec = GetChunkVector(transform.position +
                                                          new Vector3(OffX * Chunk.WORLD_SPACE_WIDTH,
                                                                      OffY * Chunk.WORLD_SPACE_HEIGHT,
                                                                      OffZ * Chunk.WORLD_SPACE_DEPTH));
                        ChunksInRange.Add(ChunkVec);
                        if (!Chunks.ContainsKey(ChunkVec)) {
                            GameObject NewChunk = Instantiate<GameObject>(ChunkPrefab);
                            Chunk Chunk = NewChunk.GetComponent<Chunk>();
                            ChunkMeshGenerator ChunkMeshGenerator = NewChunk.GetComponent<ChunkMeshGenerator>();
                            NewChunk.transform.position = new Vector3(ChunkVec.x * Chunk.WORLD_SPACE_WIDTH - Chunk.WORLD_SPACE_WIDTH / 2f,
                                                                      ChunkVec.y * Chunk.WORLD_SPACE_HEIGHT,
                                                                      ChunkVec.z * Chunk.WORLD_SPACE_DEPTH - Chunk.WORLD_SPACE_DEPTH / 2f);
                            Chunk.Generate((int)ChunkVec.x, (int)ChunkVec.y, (int)ChunkVec.z);
                            ChunkMeshGenerator.Regenerate();
                            Chunks.Add(ChunkVec, NewChunk);
                        } else {
                            GameObject OldChunk = Chunks[ChunkVec];
                            Chunk Chunk = OldChunk.GetComponent<Chunk>();
                            ChunkMeshGenerator ChunkMeshGenerator = OldChunk.GetComponent<ChunkMeshGenerator>();
                            if (Chunk.Dirty) {
                                ChunkMeshGenerator.Regenerate();
                                Chunk.Dirty = false;
                            }
                        }
                    }
                }
            }
            HashSet<Vector3> ChunksToRemove = new HashSet<Vector3>(Chunks.Keys);
            ChunksToRemove.ExceptWith(ChunksInRange);
            foreach (Vector3 OutOfRangePos in ChunksToRemove) {
                Destroy(Chunks[OutOfRangePos]);
                Chunks.Remove(OutOfRangePos);
            }
        }

        /// Returns a Vector with *integers* which are in *chunk coordinates*.
        private Vector3 GetChunkVector(Vector3 worldPosition) {
            int ChunkX = Mathf.FloorToInt((worldPosition.x + Chunk.WORLD_SPACE_WIDTH / 2) / Chunk.WORLD_SPACE_WIDTH);
            int ChunkY = Mathf.FloorToInt((worldPosition.y) / Chunk.WORLD_SPACE_HEIGHT);
            int ChunkZ = Mathf.FloorToInt((worldPosition.z + Chunk.WORLD_SPACE_DEPTH / 2) / Chunk.WORLD_SPACE_DEPTH);
            return new Vector3(ChunkX, ChunkY, ChunkZ);
        }

        public Chunk GetChunkAt(Vector3 worldPos) {
            Vector3 ChunkVec = GetChunkVector(worldPos);
            if (Chunks.ContainsKey(ChunkVec)) {
                return Chunks[ChunkVec].GetComponent<Chunk>();
            }
            return null;
        }
    }
}
