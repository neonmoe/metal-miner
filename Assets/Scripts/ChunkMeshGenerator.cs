using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Neonmoe.MetalMiner {
    [RequireComponent(typeof(MeshFilter))]
    public class ChunkMeshGenerator : MonoBehaviour {
        enum CubeSide {
            Top, Bottom, Right, Left, Front, Back
        }

        private MeshFilter MeshFilter;

        private void Awake() {
            MeshFilter = GetComponent<MeshFilter>();
        }

        private List<CubeSide> RenderedSides(float[] voxels, int x, int y, int z, int width, int height, int depth) {
            List<CubeSide> Sides = new List<CubeSide>();
            if (x == 0 || voxels[(x - 1) + y * width + z * width * height] == 0.0f) Sides.Add(CubeSide.Left);
            if (x == width - 1 || voxels[(x + 1) + y * width + z * width * height] == 0.0f) Sides.Add(CubeSide.Right);
            if (y == 0 || voxels[x + (y - 1) * width + z * width * height] == 0.0f) Sides.Add(CubeSide.Bottom);
            if (y == height - 1 || voxels[x + (y + 1) * width + z * width * height] == 0.0f) Sides.Add(CubeSide.Top);
            if (z == 0 || voxels[x + y * width + (z - 1) * width * height] == 0.0f) Sides.Add(CubeSide.Back);
            if (z == depth - 1 || voxels[x + y * width + (z + 1) * width * height] == 0.0f) Sides.Add(CubeSide.Front);
            return Sides;
        }

        public void Regenerate() {
            float VoxelSize = 1f / 2f;
            int Width = 16;
            int Height = 8;
            int Depth = 16;
            float[] Voxels = new float[Width * Height * Depth];
            for (int z = 0; z < Depth; z++) {
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        Voxels[x + y * Width + z * Width * Height] = 1.0f;
                        if (Random.value < 0.5f) {
                            Voxels[x + y * Width + z * Width * Height] = 0.0f;
                        }
                    }
                }
            }

            List<int> Indices = new List<int>();
            List<Vector3> Vertices = new List<Vector3>();
            List<Vector3> Normals = new List<Vector3>();
            for (int z = 0; z < Depth; z++) {
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        if (Voxels[x + y * Width + z * Width * Height] == 0.0f) continue;
                        float HalfExtent = VoxelSize / 2f;
                        Vector3 Center = new Vector3(x * VoxelSize + HalfExtent, y * VoxelSize + HalfExtent, z * VoxelSize + HalfExtent);
                        List<CubeSide> Sides = RenderedSides(Voxels, x, y, z, Width, Height, Depth);
                        foreach (CubeSide Side in Sides) {
                            int BaseIndex = Vertices.Count;

                            switch (Side) {
                            case CubeSide.Top:
                                Vertices.Add(Center + new Vector3(HalfExtent, HalfExtent, HalfExtent));
                                Vertices.Add(Center + new Vector3(HalfExtent, HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, HalfExtent, HalfExtent));
                                for (int i = 0; i < 4; i++) Normals.Add(Vector3.up);
                                break;
                            case CubeSide.Bottom:
                                Vertices.Add(Center + new Vector3(-HalfExtent, -HalfExtent, HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, -HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(HalfExtent, -HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(HalfExtent, -HalfExtent, HalfExtent));
                                for (int i = 0; i < 4; i++) Normals.Add(Vector3.down);
                                break;
                            case CubeSide.Right:
                                Vertices.Add(Center + new Vector3(HalfExtent, -HalfExtent, HalfExtent));
                                Vertices.Add(Center + new Vector3(HalfExtent, -HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(HalfExtent, HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(HalfExtent, HalfExtent, HalfExtent));
                                for (int i = 0; i < 4; i++) Normals.Add(Vector3.right);
                                break;
                            case CubeSide.Left:
                                Vertices.Add(Center + new Vector3(-HalfExtent, -HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, -HalfExtent, HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, HalfExtent, HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, HalfExtent, -HalfExtent));
                                for (int i = 0; i < 4; i++) Normals.Add(Vector3.left);
                                break;
                            case CubeSide.Front:
                                Vertices.Add(Center + new Vector3(HalfExtent, HalfExtent, HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, HalfExtent, HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, -HalfExtent, HalfExtent));
                                Vertices.Add(Center + new Vector3(HalfExtent, -HalfExtent, HalfExtent));
                                for (int i = 0; i < 4; i++) Normals.Add(Vector3.forward);
                                break;
                            case CubeSide.Back:
                                Vertices.Add(Center + new Vector3(HalfExtent, -HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, -HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(-HalfExtent, HalfExtent, -HalfExtent));
                                Vertices.Add(Center + new Vector3(HalfExtent, HalfExtent, -HalfExtent));
                                for (int i = 0; i < 4; i++) Normals.Add(Vector3.back);
                                break;
                            }

                            for (int i = 0; i < 4; i++) {
                                Indices.Add(BaseIndex + i);
                            }
                        }
                    }
                }
            }

            if (MeshFilter.mesh == null) {
                MeshFilter.mesh = new Mesh();
            }
            MeshFilter.mesh.SetVertices(Vertices);
            MeshFilter.mesh.SetNormals(Normals);
            MeshFilter.mesh.indexFormat = IndexFormat.UInt32;
            MeshFilter.mesh.SetIndices(Indices, MeshTopology.Quads, 0);
        }
    }
}
