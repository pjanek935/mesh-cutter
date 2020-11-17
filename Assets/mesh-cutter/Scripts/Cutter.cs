using System.Collections.Generic;
using UnityEngine;

namespace MeshCutter
{
    public class Cutter
    {
        public static CutResult Cut (Mesh mesh, Plane plane)
        {
            Mesh aMesh = new Mesh ();
            Mesh bMesh = new Mesh ();
            List<Vector3> aVerticies = new List<Vector3> ();
            List<Vector3> bVerticies = new List<Vector3> ();
            List<Vector3> aNormals = new List<Vector3> ();
            List<Vector3> bNormals = new List<Vector3> ();
            List<Vector2> aUVs = new List<Vector2> ();
            List<Vector2> bUVs = new List<Vector2> ();
            List<int> aTriangles = new List<int> ();
            List<int> bTriangles = new List<int> ();

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                int aVertexIndex = mesh.triangles [i];
                int bVertexIndex = mesh.triangles [i + 1];
                int cVertexIndex = mesh.triangles [i + 2];

                float aSide = plane.GetSide (mesh.vertices [aVertexIndex]);
                float bSide = plane.GetSide (mesh.vertices [bVertexIndex]);
                float cSide = plane.GetSide (mesh.vertices [cVertexIndex]);

                if (aSide > 0 &&
                    bSide > 0 &&
                    cSide > 0)
                {//all verticies on a positive side
                    aVerticies.Add (mesh.vertices [aVertexIndex]);
                    aVerticies.Add (mesh.vertices [bVertexIndex]);
                    aVerticies.Add (mesh.vertices [cVertexIndex]);

                    aUVs.Add (mesh.uv [aVertexIndex]);
                    aUVs.Add (mesh.uv [bVertexIndex]);
                    aUVs.Add (mesh.uv [cVertexIndex]);

                    aNormals.Add (mesh.normals [aVertexIndex]);
                    aNormals.Add (mesh.normals [bVertexIndex]);
                    aNormals.Add (mesh.normals [cVertexIndex]);

                    aTriangles.Add (aTriangles.Count);
                    aTriangles.Add (aTriangles.Count);
                    aTriangles.Add (aTriangles.Count);
                }
                else if (aSide < 0 &&
                         bSide < 0 &&
                         cSide < 0)
                {//all verticies on a negative side
                    bVerticies.Add (mesh.vertices [aVertexIndex]);
                    bVerticies.Add (mesh.vertices [bVertexIndex]);
                    bVerticies.Add (mesh.vertices [cVertexIndex]);

                    bUVs.Add (mesh.uv [aVertexIndex]);
                    bUVs.Add (mesh.uv [bVertexIndex]);
                    bUVs.Add (mesh.uv [cVertexIndex]);

                    bNormals.Add (mesh.normals [aVertexIndex]);
                    bNormals.Add (mesh.normals [bVertexIndex]);
                    bNormals.Add (mesh.normals [cVertexIndex]);

                    bTriangles.Add (bTriangles.Count);
                    bTriangles.Add (bTriangles.Count);
                    bTriangles.Add (bTriangles.Count);
                }
                //TODO...
            }

            aMesh.vertices = aVerticies.ToArray ();
            bMesh.vertices = bVerticies.ToArray ();
            aMesh.normals = aNormals.ToArray ();
            bMesh.normals = bNormals.ToArray ();
            aMesh.uv = aUVs.ToArray ();
            bMesh.uv = bUVs.ToArray ();
            aMesh.triangles = aTriangles.ToArray ();
            bMesh.triangles = bTriangles.ToArray ();

            return new CutResult (aMesh, bMesh);
        }
    }
}

