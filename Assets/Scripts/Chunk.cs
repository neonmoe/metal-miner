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
        public bool Untouched = true;

        private bool EntirelyEmpty = false;
        private float[] Tiles = new float[WIDTH * HEIGHT * DEPTH];
        private int[] TileTypes = new int[WIDTH * HEIGHT * DEPTH];

        public void Generate(int x, int y, int z) {
            if (y > 0) {
                EntirelyEmpty = true;
            } else if (y == 0) {
                EntirelyEmpty = false;
                for (int i = 0; i < WIDTH * HEIGHT * DEPTH; i++) {
                    int TileX = (i % WIDTH) + x * WIDTH;
                    int TileY = ((i / WIDTH) % HEIGHT) + y * HEIGHT;
                    int TileZ = (i / WIDTH / HEIGHT) + z * DEPTH;
                    Tiles[i] = Mathf.PerlinNoise(TileX * 0.1f, TileZ * 0.1f) * 2 > TileY ? 1f : 0f;
                    TileTypes[i] = 0;
                }
            } else {
                EntirelyEmpty = false;
                for (int i = 0; i < WIDTH * HEIGHT * DEPTH; i++) {
                    int TileX = (i % WIDTH) + x * WIDTH;
                    int TileY = ((i / WIDTH) % HEIGHT) + y * HEIGHT;
                    int TileZ = (i / WIDTH / HEIGHT) + z * DEPTH;
                    Tiles[i] = 1f;
                    if (TileY >= -10) {
                        TileTypes[i] = Mathf.PerlinNoise(TileX * 0.1f, TileZ * 0.1f) * 4 < TileY + 10 ? 1 : 2;
                    } else {
                        TileTypes[i] = 2;
                    }
                }
                while (Random.value < 0.5f + 0.3f * -y / 10f) {
                    int VeinWidth = (int)(Random.value * 4) + 2;
                    int VeinHeight = (int)(Random.value * 4) + 2;
                    int VeinDepth = (int)(Random.value * 4) + 2;
                    int BaseX = (int)(Random.value * (WIDTH - VeinWidth));
                    int BaseY = (int)(Random.value * (HEIGHT - VeinHeight));
                    int BaseZ = (int)(Random.value * (DEPTH - VeinDepth));
                    for (int OffZ = BaseZ; OffZ < BaseZ + VeinDepth; OffZ++) {
                        for (int OffY = BaseY; OffY < BaseY + VeinHeight; OffY++) {
                            for (int OffX = BaseX; OffX < BaseX + VeinWidth; OffX++) {
                                TileTypes[OffX + OffY * WIDTH + OffZ * WIDTH * HEIGHT] = 3;
                            }
                        }
                    }
                }
            }
        }

        public bool IsEmpty() {
            return EntirelyEmpty;
        }

        public bool IsSolid(int x, int y, int z) {
            return !EntirelyEmpty && Tiles[x + y * WIDTH + z * WIDTH * HEIGHT] > 0.0f;
        }

        public int GetTileType(int x, int y, int z) {
            return TileTypes[x + y * WIDTH + z * WIDTH * HEIGHT];
        }

        public void DamageTile(Vector3 worldPos, float damage) {
            worldPos += new Vector3(WORLD_SPACE_WIDTH / 2, 0, WORLD_SPACE_DEPTH / 2);
            int TileX = (Mathf.FloorToInt(worldPos.x * WIDTH / WORLD_SPACE_WIDTH) % WIDTH + WIDTH) % WIDTH;
            int TileY = (Mathf.FloorToInt(worldPos.y * HEIGHT / WORLD_SPACE_HEIGHT) % HEIGHT + HEIGHT) % HEIGHT;
            int TileZ = (Mathf.FloorToInt(worldPos.z * DEPTH / WORLD_SPACE_DEPTH) % DEPTH + DEPTH) % DEPTH;
            int Index = TileX + TileY * WIDTH + TileZ * WIDTH * HEIGHT;
            EntirelyEmpty = false;
            Tiles[Index] -= damage;
            if (Tiles[Index] <= 0.0f) {
                Dirty = true;
                Untouched = false;
            }
        }
    }
}
