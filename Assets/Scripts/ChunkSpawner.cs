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

                        Vector3 Offset = new Vector3(OffX * Chunk.WORLD_SPACE_WIDTH,
                                                     OffY * Chunk.WORLD_SPACE_HEIGHT,
                                                     OffZ * Chunk.WORLD_SPACE_DEPTH);
                        Vector3 ChunkVec = GetChunkVector(transform.position + Offset);
                        Vector3 ChunkVecWorldSpace = GetChunkVector(transform.position + Offset, false);
                        Vector3 ChunkPosition = new Vector3(ChunkVecWorldSpace.x * Chunk.WORLD_SPACE_WIDTH - Chunk.WORLD_SPACE_WIDTH / 2f,
                                                            ChunkVecWorldSpace.y * Chunk.WORLD_SPACE_HEIGHT,
                                                            ChunkVecWorldSpace.z * Chunk.WORLD_SPACE_DEPTH - Chunk.WORLD_SPACE_DEPTH / 2f);
                        ChunksInRange.Add(ChunkVec);
                        if (!Chunks.ContainsKey(ChunkVec)) {
                            GameObject NewChunk = Instantiate<GameObject>(ChunkPrefab);
                            Chunk Chunk = NewChunk.GetComponent<Chunk>();
                            ChunkMeshGenerator ChunkMeshGenerator = NewChunk.GetComponent<ChunkMeshGenerator>();
                            NewChunk.transform.position = ChunkPosition;
                            Chunk.Generate((int)ChunkVec.x, (int)ChunkVec.y, (int)ChunkVec.z);
                            ChunkMeshGenerator.Regenerate();
                            Chunks.Add(ChunkVec, NewChunk);
                        } else {
                            GameObject OldChunk = Chunks[ChunkVec];
                            OldChunk.SetActive(true);
                            OldChunk.transform.position = ChunkPosition;
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
                GameObject ChunkObj = Chunks[OutOfRangePos];
                if (ChunkObj.GetComponent<Chunk>().Untouched) {
                    Destroy(ChunkObj);
                    Chunks.Remove(OutOfRangePos);
                } else {
                    ChunkObj.SetActive(false);
                }
            }
        }

        /// Returns a Vector with *integers* which are in *chunk coordinates*.
        private Vector3 GetChunkVector(Vector3 worldPosition, bool wrap = true) {
            int ChunkX = Mathf.FloorToInt((worldPosition.x + Chunk.WORLD_SPACE_WIDTH / 2) / Chunk.WORLD_SPACE_WIDTH);
            int ChunkY = Mathf.FloorToInt((worldPosition.y) / Chunk.WORLD_SPACE_HEIGHT);
            int ChunkZ = Mathf.FloorToInt((worldPosition.z + Chunk.WORLD_SPACE_DEPTH / 2) / Chunk.WORLD_SPACE_DEPTH);
            if (wrap) {
                int WrapAt = 5;
                ChunkX = ((ChunkX % WrapAt) + WrapAt) % WrapAt;
                ChunkZ = ((ChunkZ % WrapAt) + WrapAt) % WrapAt;
            }
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
