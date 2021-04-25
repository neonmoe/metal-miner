using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Neonmoe.MetalMiner {
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(Chunk))]
    public class ChunkMeshGenerator : MonoBehaviour {
        enum CubeSide {
            Top, Bottom, Right, Left, Front, Back
        }

        private MeshFilter MeshFilter;
        private Chunk Chunk;

        private void Awake() {
            MeshFilter = GetComponent<MeshFilter>();
            Chunk = GetComponent<Chunk>();
        }

        private List<CubeSide> RenderedSides(int x, int y, int z) {
            List<CubeSide> Sides = new List<CubeSide>();
            if (x == 0 || !Chunk.IsSolid(x - 1, y, z)) Sides.Add(CubeSide.Left);
            if (x == Chunk.WIDTH - 1 || !Chunk.IsSolid(x + 1, y, z)) Sides.Add(CubeSide.Right);
            if (y == 0 || !Chunk.IsSolid(x, y - 1, z)) Sides.Add(CubeSide.Bottom);
            if (y == Chunk.HEIGHT - 1 || !Chunk.IsSolid(x, y + 1, z)) Sides.Add(CubeSide.Top);
            if (z == 0 || !Chunk.IsSolid(x, y, z - 1)) Sides.Add(CubeSide.Back);
            if (z == Chunk.DEPTH - 1 || !Chunk.IsSolid(x, y, z + 1)) Sides.Add(CubeSide.Front);
            return Sides;
        }

        public void Regenerate() {
            if (Chunk.IsEmpty()) return;

            float VoxelSize = 1f / 2f;
            List<int> Indices = new List<int>();
            List<Vector3> Vertices = new List<Vector3>();
            List<Vector3> Normals = new List<Vector3>();
            for (int z = 0; z < Chunk.DEPTH; z++) {
                for (int y = 0; y < Chunk.HEIGHT; y++) {
                    for (int x = 0; x < Chunk.WIDTH; x++) {
                        if (!Chunk.IsSolid(x, y, z)) continue;
                        float HalfExtent = VoxelSize / 2f;
                        Vector3 Center = new Vector3(x * VoxelSize + HalfExtent, y * VoxelSize + HalfExtent, z * VoxelSize + HalfExtent);
                        List<CubeSide> Sides = RenderedSides(x, y, z);

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
            MeshFilter.mesh.Clear();
            MeshFilter.mesh.SetVertices(Vertices);
            MeshFilter.mesh.SetNormals(Normals);
            MeshFilter.mesh.indexFormat = IndexFormat.UInt32;
            MeshFilter.mesh.SetIndices(Indices, MeshTopology.Quads, 0);
            MeshFilter.mesh.Optimize();
            MeshCollider Collider = GetComponent<MeshCollider>();
            Collider.sharedMesh = MeshFilter.mesh;
        }
    }
}
