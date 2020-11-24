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
        List<int> edgeVertecies = new List<int> ();

        public MeshData ()
        {

        }

        public int Add2 (Vector3 vertex, Vector3 normal, Vector2 uv)
        {
            vertices.Add (vertex);
            normals.Add (normal);
            UVs.Add (uv);
            triangles.Add (vertices.Count - 1);

            return vertices.Count - 1;
        }

        public int Add (Vector3 vertex, Vector3 normal, Vector2 uv)
        {
            int index = vertices.IndexOf (vertex);
            bool addNew = true;

            if (index >= 0)
            {
                if (isNearlyEqual (normal, normals [index]))
                {
                    addNew = false;
                }
            }

            if (addNew)
            {
                vertices.Add (vertex);
                normals.Add (normal);
                UVs.Add (uv);
                triangles.Add (vertices.Count - 1);
                index = vertices.Count - 1;
            }
            else
            {
                triangles.Add (index);
            }

            return index;
        }

        public void AddEdgeVertex (int vertexIndex)
        {
            if (edgeVertecies.IndexOf (vertexIndex) == -1)
            {
                edgeVertecies.Add (vertexIndex);
            }
        }

        public void FillMeshWithEdgeVerteciesData ()
        {
            if (edgeVertecies.Count == 0 || vertices.Count == 0) return;
            //edgeVertecies.Sort ();
            //TODO
            Vector3 center = Vector3.zero;
            edgeVertecies.ForEach (x => center += vertices [x]);
            center /= edgeVertecies.Count;

            vertices.Add (center);
            normals.Add (Vector3.up);
            UVs.Add (Vector3.zero);
            int centerIndex = vertices.Count - 1;

            for (int i = 0; i < edgeVertecies.Count - 1; i++)
            {
                triangles.Add (edgeVertecies [i]);
                triangles.Add (edgeVertecies [i + 1]);
                triangles.Add (centerIndex);
            }

            edgeVertecies.Clear ();
        }

        static bool isNearlyEqual (float f1, float f2)
        {
            bool result = false;
            const float minFloat = 0.0001f;

            if (f1 == f2)
            {
                result = true;
            }
            else
            {
                result = Mathf.Abs (f1 - f2) < minFloat;
            }

            return result;
        }

        static bool isNearlyEqual (Vector3 v1, Vector3 v2)
        {
            return isNearlyEqual (v1.x, v2.x) &&
                isNearlyEqual (v1.y, v2.y) && isNearlyEqual (v1.z, v2.z);
        }

        public Mesh ToMesh ()
        {
            FillMeshWithEdgeVerteciesData ();

            Mesh mesh = new Mesh ();
            mesh.vertices = vertices.ToArray ();
            mesh.normals = normals.ToArray ();
            mesh.uv = UVs.ToArray ();
            mesh.triangles = triangles.ToArray ();

            return mesh;
        }

        public int GetVerticesCount ()
        {
            return vertices.Count;
        }
    }
}

