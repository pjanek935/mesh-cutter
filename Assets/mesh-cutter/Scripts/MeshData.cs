using System.Collections.Generic;
using UnityEngine;

namespace MeshCutter
{
    public class MeshData
    {
        List<Vector3> vertices = new List<Vector3> ();
        List<Vector3> normals = new List<Vector3> ();
        List<Vector2> UVs = new List<Vector2> ();
        List<int> triangles = new List<int> ();

        public MeshData ()
        {

        }

        public void Add (Vector3 vertex, Vector3 normal, Vector2 uv)
        {
            vertices.Add (vertex);
            normals.Add (normal);
            UVs.Add (uv);
            triangles.Add (triangles.Count);
        }

        public Mesh ToMesh ()
        {
            Mesh mesh = new Mesh ();
            mesh.vertices = vertices.ToArray ();
            mesh.normals = normals.ToArray ();
            mesh.uv = UVs.ToArray ();
            mesh.triangles = triangles.ToArray ();

            return mesh;
        }
    }
}

