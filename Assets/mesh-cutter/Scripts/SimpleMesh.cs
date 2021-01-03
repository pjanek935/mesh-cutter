using UnityEngine;

namespace MeshCutter
{
    public struct SimpleMesh
    {
        public int [] Triangles;
        public Vector3 [] Vertices;
        public Vector3 [] Normals;
        public Vector2 [] UVs;

        public void Create (Mesh mesh)
        {
            if (mesh != null)
            {
                Triangles = mesh.triangles;
                Vertices = mesh.vertices;
                Normals = mesh.normals;
                UVs = mesh.uv;
            }
        }
    }
}

