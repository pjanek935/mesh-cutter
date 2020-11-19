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
                else if (aSide > 0 && bSide < 0 && cSide < 0)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, mesh,
                        plane, aVertexIndex, bVertexIndex, cVertexIndex);
                }
                else if (bSide > 0 && cSide < 0 && aSide < 0)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, 
                        plane, bVertexIndex, cVertexIndex, aVertexIndex);
                }
                else if (cSide > 0 && aSide < 0 && bSide < 0)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, mesh,
                        plane, cVertexIndex, aVertexIndex, bVertexIndex);
                }
                else if (aSide < 0 && bSide > 0 && cSide > 0)
                {

                }
                //TODO...
            }

            Debug.Log ("Vertices count: " + (aMeshData.GetVerticesCount () + bMeshData.GetVerticesCount ()));

            return new CutResult (aMeshData.ToMesh (), bMeshData.ToMesh ());
        }

        static void sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aPositiveIndex, int bNegativeIndex, int cNegativeIndex)
        {
            Vector3 abIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [aPositiveIndex],
                originalMesh.vertices [bNegativeIndex]);
            Vector3 acIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [aPositiveIndex],
                originalMesh.vertices [cNegativeIndex]);

            aMeshData.Add (originalMesh.vertices [aPositiveIndex], 
                originalMesh.normals [aPositiveIndex], originalMesh.uv [aPositiveIndex]);
            aMeshData.Add (abIntersection, originalMesh.normals [aPositiveIndex],
                originalMesh.uv [aPositiveIndex]);
            aMeshData.Add (acIntersection, originalMesh.normals [aPositiveIndex],
                originalMesh.uv [aPositiveIndex]);

            bMeshData.Add (acIntersection, originalMesh.normals [cNegativeIndex],
                originalMesh.uv [cNegativeIndex]);
            bMeshData.Add (abIntersection, originalMesh.normals [bNegativeIndex],
                originalMesh.uv [bNegativeIndex]);
            bMeshData.Add (originalMesh.vertices [bNegativeIndex],
                originalMesh.normals [bNegativeIndex], originalMesh.uv [bNegativeIndex]);

            bMeshData.Add (acIntersection, originalMesh.normals [cNegativeIndex],
                originalMesh.uv [cNegativeIndex]);
            bMeshData.Add (originalMesh.vertices [bNegativeIndex], originalMesh.normals [bNegativeIndex],
                originalMesh.uv [bNegativeIndex]);
            bMeshData.Add (originalMesh.vertices [cNegativeIndex], originalMesh.normals [cNegativeIndex],
                originalMesh.uv [cNegativeIndex]);
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

