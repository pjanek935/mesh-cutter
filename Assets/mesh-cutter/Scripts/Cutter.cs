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
            List<Vector3> sliceEdgeVertecies = new List<Vector3> ();

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
                        plane, aVertexIndex, bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide > 0 && cSide < 0 && aSide < 0)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, 
                        plane, bVertexIndex, cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide > 0 && aSide < 0 && bSide < 0)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, mesh,
                        plane, cVertexIndex, aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
                else if (aSide < 0 && bSide > 0 && cSide > 0)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, plane, bVertexIndex,
                        cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide < 0 && cSide > 0 && aSide > 0)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, plane, cVertexIndex,
                        aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide < 0 && aSide > 0 && bSide > 0)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, plane, aVertexIndex,
                        bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                }
                //TODO...
            }

            Debug.Log ("Vertices count: " + (aMeshData.GetVerticesCount () + bMeshData.GetVerticesCount ()));

            return new CutResult (aMeshData.ToMesh (sliceEdgeVertecies, 1, plane),
                bMeshData.ToMesh (sliceEdgeVertecies , - 1, plane));
        }

        static void sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aPositiveIndex, int bPositiveIndex, int cNegativeIndex, List <Vector3> sliceEdgeVertices)
        {
            float acD; //normalized distance between A and C
            Vector3 acIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [aPositiveIndex],
                originalMesh.vertices [cNegativeIndex], out acD);

            float bcD;
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [bPositiveIndex],
                originalMesh.vertices [cNegativeIndex], out bcD);

            Vector3 acNormal = Vector3.Lerp (originalMesh.normals [aPositiveIndex], originalMesh.normals [cNegativeIndex], acD);
            Vector2 acUV = Vector2.Lerp (originalMesh.uv [aPositiveIndex], originalMesh.uv [cNegativeIndex], acD);

            Vector3 bcNormal = Vector3.Lerp (originalMesh.normals [bPositiveIndex], originalMesh.normals [cNegativeIndex], bcD);
            Vector2 bcUV = Vector3.Lerp (originalMesh.uv [bPositiveIndex], originalMesh.uv [cNegativeIndex], bcD);

            sliceEdgeVertices.Add (acIntersection);
            sliceEdgeVertices.Add (bcIntersection);

            aMeshData.Add (originalMesh.vertices [aPositiveIndex],
                originalMesh.normals [aPositiveIndex], originalMesh.uv [aPositiveIndex]);
            aMeshData.Add (originalMesh.vertices [bPositiveIndex],
               originalMesh.normals [bPositiveIndex], originalMesh.uv [bPositiveIndex]);
            aMeshData.Add (acIntersection, acNormal, acUV);

            aMeshData.Add (acIntersection, acNormal, acUV);
            aMeshData.Add (originalMesh.vertices [bPositiveIndex],
              originalMesh.normals [bPositiveIndex], originalMesh.uv [bPositiveIndex]);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (bcIntersection, bcNormal, bcUV);
            bMeshData.Add (originalMesh.vertices [cNegativeIndex],
             originalMesh.normals [cNegativeIndex], originalMesh.uv [cNegativeIndex]);
        }

        static void sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aPositiveIndex, int bNegativeIndex, int cNegativeIndex, List <Vector3> sliceEdgeVertecies)
        {
            float abD; //normalized distance between A and B
            Vector3 abIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [aPositiveIndex],
                originalMesh.vertices [bNegativeIndex], out abD);

            float acD;
            Vector3 acIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [aPositiveIndex],
                originalMesh.vertices [cNegativeIndex], out acD);

            Vector3 abNormal = Vector3.Lerp (originalMesh.normals [aPositiveIndex], originalMesh.normals [bNegativeIndex], abD);
            Vector2 abUV = Vector3.Lerp (originalMesh.normals [aPositiveIndex], originalMesh.normals [bNegativeIndex], abD);

            Vector3 acNormal = Vector3.Lerp (originalMesh.normals [aPositiveIndex], originalMesh.normals [cNegativeIndex], acD);
            Vector2 acUV = Vector3.Lerp (originalMesh.uv [aPositiveIndex], originalMesh.uv [cNegativeIndex], acD);

            sliceEdgeVertecies.Add (acIntersection);
            sliceEdgeVertecies.Add (abIntersection);

            aMeshData.Add (originalMesh.vertices [aPositiveIndex], 
                originalMesh.normals [aPositiveIndex], originalMesh.uv [aPositiveIndex]);
            aMeshData.Add (abIntersection, abNormal, abUV);
            aMeshData.Add (acIntersection, acNormal, acUV);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (abIntersection, abNormal, abUV);
            bMeshData.Add (originalMesh.vertices [bNegativeIndex],
                originalMesh.normals [bNegativeIndex], originalMesh.uv [bNegativeIndex]);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (originalMesh.vertices [bNegativeIndex], originalMesh.normals [bNegativeIndex],
                originalMesh.uv [bNegativeIndex]);
            bMeshData.Add (originalMesh.vertices [cNegativeIndex], originalMesh.normals [cNegativeIndex],
                originalMesh.uv [cNegativeIndex]);
        }

        /// <summary>
        /// D is a normalized distance between lineStart and lineEnd
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        static Vector3 getPlaneIntersectionPoint (Plane plane, Vector3 lineStart, Vector3 lineEnd, out float d)
        {
            Vector3 lineDirection = lineEnd - lineStart;
            lineDirection.Normalize ();
            d = Vector3.Dot (plane.Origin - lineStart, plane.Normal) / Vector3.Dot (lineDirection, plane.Normal);

            return lineStart + lineDirection * d;
        }
    }
}

