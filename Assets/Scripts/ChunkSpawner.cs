using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    public class ChunkSpawner : MonoBehaviour {
        public GameObject ChunkPrefab;

        private void Start() {
            GameObject NewChunk = Instantiate<GameObject>(ChunkPrefab);
            ChunkMeshGenerator ChunkMeshGenerator = NewChunk.GetComponent<ChunkMeshGenerator>();
            ChunkMeshGenerator.Regenerate();
        }
    }
}
