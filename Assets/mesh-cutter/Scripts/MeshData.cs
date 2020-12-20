using System;
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

        public int Add2 (Vector3 vertex, Vector3 normal, Vector2 uv)
        {
            vertices.Add (vertex);
            normals.Add (normal);
            UVs.Add (uv);
            triangles.Add (vertices.Count - 1);

            return vertices.Count - 1;
        }

        public void Add (Vector3 vertex, Vector3 normal, Vector2 uv)
        {
            int index = vertices.IndexOf (vertex);
            bool addNew = true;

            if (index >= 0)
            {
                if (isNearlyEqual (normal, normals [index]) && isNearlyEqual (uv, UVs [index]))
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
        }

        public void FillMeshWithEdgeVerteciesData (List <Vector3> edgeVertecies, float side, Plane plane)
        {
            if (edgeVertecies.Count == 0 || vertices.Count == 0) return;
            
            Vector3 center = Vector3.zero;
            edgeVertecies.ForEach (x => center += x);
            center /= edgeVertecies.Count;

            Vector3 normal = side > 0 ? plane.Normal : -plane.Normal;

            vertices.Add (center);
            normals.Add (normal);
            UVs.Add (Vector3.zero);
            int centerIndex = vertices.Count - 1;

            for (int i = 0; i < edgeVertecies.Count; i += 2)
            {
                vertices.Add (edgeVertecies [i]);
                normals.Add (normal);
                UVs.Add (Vector3.zero);

                vertices.Add (edgeVertecies [i + 1]);
                normals.Add (normal);
                UVs.Add (Vector3.zero);

                if (side > 0)
                {
                    triangles.Add (centerIndex);
                    triangles.Add (vertices.Count - 2);
                    triangles.Add (vertices.Count - 1);
                }
                else
                {
                    triangles.Add (centerIndex);
                    triangles.Add (vertices.Count - 1);
                    triangles.Add (vertices.Count - 2);
                }
            }
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

        public Mesh ToMesh (List <Vector3> edgeVertecies, float side, Plane plane)
        {
            //FillMeshWithEdgeVerteciesData (edgeVertecies, side, plane);

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

