using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neonmoe.MetalMiner {
    public class Chunk : MonoBehaviour {
        public const int WIDTH = 16;
        public const int HEIGHT = 8;
        public const int DEPTH = 16;
        public const float WORLD_SPACE_WIDTH = 8;
        public const float WORLD_SPACE_HEIGHT = 4;
        public const float WORLD_SPACE_DEPTH = 8;

        public bool Dirty = false;

        private bool EntirelyEmpty = false;
        private bool[] Tiles = new bool[WIDTH * HEIGHT * DEPTH];

        public void Generate(int x, int y, int z) {
            if (y > 0) {
                EntirelyEmpty = true;
            } else if (y == 0) {
                EntirelyEmpty = false;
                for (int i = 0; i < WIDTH * HEIGHT * DEPTH; i++) {
                    int TileX = (i % WIDTH) + x * WIDTH;
                    int TileY = ((i / WIDTH) % HEIGHT) + y * HEIGHT;
                    int TileZ = (i / WIDTH / HEIGHT) + z * DEPTH;
                    Tiles[i] = Mathf.PerlinNoise(TileX * 0.1f, TileZ * 0.1f) * 2 > TileY;
                }
            } else {
                EntirelyEmpty = false;
                for (int i = 0; i < WIDTH * HEIGHT * DEPTH; i++) {
                    Tiles[i] = true;
                }
            }
        }

        public bool IsEmpty() {
            return EntirelyEmpty;
        }

        public bool IsSolid(int x, int y, int z) {
            return !EntirelyEmpty && Tiles[x + y * WIDTH + z * WIDTH * HEIGHT];
        }

        public void RemoveTile(Vector3 worldPos) {
            worldPos += new Vector3(WORLD_SPACE_WIDTH / 2, 0, WORLD_SPACE_DEPTH / 2);
            int TileX = (Mathf.FloorToInt(worldPos.x * WIDTH / WORLD_SPACE_WIDTH) % WIDTH + WIDTH) % WIDTH;
            int TileY = (Mathf.FloorToInt(worldPos.y * HEIGHT / WORLD_SPACE_HEIGHT) % HEIGHT + HEIGHT) % HEIGHT;
            int TileZ = (Mathf.FloorToInt(worldPos.z * DEPTH / WORLD_SPACE_DEPTH) % DEPTH + DEPTH) % DEPTH;
            EntirelyEmpty = false;
            Tiles[TileX + TileY * WIDTH + TileZ * WIDTH * HEIGHT] = false;
            Dirty = true;
        }
    }
}
