using System.Collections.Generic;
using UnityEngine;

namespace MeshCutter
{
    public class Cutter
    {
        public static CutResult Cut (Mesh mesh, Plane plane)
        {
            MeshData aMeshData = new MeshData ();
            MeshData bMeshData = new MeshData ();

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
                    aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                    aMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    aMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                }
                else if (aSide < 0 &&
                         bSide < 0 &&
                         cSide < 0)
                {//all verticies on a negative side
                    bMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                    bMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    bMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                }
                else if (aSide > 0 && bSide > 0 && cSide < 0)
                {
                    Vector3 bcIntersection = getPlaneIntersectionPoint (plane, mesh.vertices [bVertexIndex], mesh.vertices [cVertexIndex]);
                    Vector3 acIntersection = getPlaneIntersectionPoint (plane, mesh.vertices [aVertexIndex], mesh.vertices [cVertexIndex]);

                    aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                    aMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    aMeshData.Add (bcIntersection, mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);

                    aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                    aMeshData.Add (bcIntersection, mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    aMeshData.Add (acIntersection, mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);

                    bMeshData.Add (bcIntersection, mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    bMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    bMeshData.Add (acIntersection, mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                }
                else if (aSide > 0 && bSide < 0 && cSide < 0)
                {
                    Vector3 abIntersection = getPlaneIntersectionPoint (plane, mesh.vertices [aVertexIndex], mesh.vertices [bVertexIndex]);
                    Vector3 acIntersection = getPlaneIntersectionPoint (plane, mesh.vertices [aVertexIndex], mesh.vertices [cVertexIndex]);

                    aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                    aMeshData.Add (abIntersection, mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                    aMeshData.Add (acIntersection, mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);

                    bMeshData.Add (acIntersection, mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    bMeshData.Add (abIntersection, mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    bMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);

                    bMeshData.Add (acIntersection, mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    bMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    bMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                }
                //TODO...
            }

            return new CutResult (aMeshData.ToMesh (), bMeshData.ToMesh ());
        }

        static Vector3 getPlaneIntersectionPoint (Plane plane, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 lineDirection = lineEnd - lineStart;
            lineDirection.Normalize ();
            float d = Vector3.Dot (plane.Origin - lineStart, plane.Normal) / Vector3.Dot (lineDirection, plane.Normal);

            return lineStart + lineDirection * d;
        }
    }
}

