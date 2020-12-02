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

                PlaneSide aSide = plane.GetSide (mesh.vertices [aVertexIndex]);
                PlaneSide bSide = plane.GetSide (mesh.vertices [bVertexIndex]);
                PlaneSide cSide = plane.GetSide (mesh.vertices [cVertexIndex]);

                if (aSide == PlaneSide.POSITIVE &&
                    bSide == PlaneSide.POSITIVE &&
                    cSide == PlaneSide.POSITIVE)
                {//all verticies on a positive side
                    aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                    aMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    aMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                }
                else if (aSide == PlaneSide.NEGATIVE &&
                         bSide == PlaneSide.NEGATIVE &&
                         cSide == PlaneSide.NEGATIVE)
                {//all verticies on a negative side
                    bMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                    bMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                    bMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                }
                else if (aSide == PlaneSide.POSITIVE && bSide == PlaneSide.NEGATIVE && cSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, mesh,
                        plane, aVertexIndex, bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide == PlaneSide.POSITIVE && cSide == PlaneSide.NEGATIVE && aSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, 
                        plane, bVertexIndex, cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide == PlaneSide.POSITIVE && aSide == PlaneSide.NEGATIVE && bSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, mesh,
                        plane, cVertexIndex, aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
                else if (aSide == PlaneSide.NEGATIVE && bSide == PlaneSide.POSITIVE && cSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, plane, bVertexIndex,
                        cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide == PlaneSide.NEGATIVE && cSide == PlaneSide.POSITIVE && aSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, plane, cVertexIndex,
                        aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide == PlaneSide.NEGATIVE && aSide == PlaneSide.POSITIVE && bSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, mesh, plane, aVertexIndex,
                        bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                }
                else if (aSide == PlaneSide.ON_PLANE)
                {
                    if (bSide == PlaneSide.POSITIVE && cSide == PlaneSide.POSITIVE)
                    {//treat as all vertecies are on a postive halfspace
                        aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                        aMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                        aMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    }
                    else if (bSide == PlaneSide.NEGATIVE && cSide == PlaneSide.NEGATIVE)
                    {//treat as all vertecies are on a negative halfspace
                        bMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                        bMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                        bMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    }
                    else if (bSide == PlaneSide.POSITIVE && cSide == PlaneSide.NEGATIVE)
                    {
                        sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, mesh, plane, aVertexIndex, bVertexIndex,
                            cVertexIndex, sliceEdgeVertecies);
                    }
                    else if (bSide == PlaneSide.NEGATIVE && cSide == PlaneSide.POSITIVE)
                    {
                        sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, mesh, plane, aVertexIndex, 
                            bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                    }

                }
                else if (bSide == PlaneSide.ON_PLANE)
                {
                    if (cSide == PlaneSide.POSITIVE && aSide == PlaneSide.POSITIVE)
                    {//treat as all vertecies are on a postive halfspace
                        aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                        aMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                        aMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    }
                    else if (cSide == PlaneSide.NEGATIVE && aSide == PlaneSide.NEGATIVE)
                    {//treat as all vertecies are on a negative halfspace
                        bMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                        bMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                        bMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    }
                    else if (cSide == PlaneSide.POSITIVE && aSide == PlaneSide.NEGATIVE)
                    {
                        sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, mesh, plane,
                            bVertexIndex, cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                    }
                    else if (cSide == PlaneSide.NEGATIVE && aSide == PlaneSide.POSITIVE)
                    {
                        sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, mesh, plane, bVertexIndex,
                           cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                    }
                }
                else if (cSide == PlaneSide.ON_PLANE)
                {
                    if (aSide == PlaneSide.POSITIVE && bSide == PlaneSide.POSITIVE)
                    {//treat as all vertecies are on a postive halfspace
                        aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                        aMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                        aMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    }
                    else if (aSide == PlaneSide.NEGATIVE && bSide == PlaneSide.NEGATIVE)
                    {//treat as all vertecies are on a negative halfspace
                        bMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], mesh.uv [aVertexIndex]);
                        bMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], mesh.uv [bVertexIndex]);
                        bMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], mesh.uv [cVertexIndex]);
                    }
                    else if (aSide == PlaneSide.POSITIVE && bSide == PlaneSide.NEGATIVE)
                    {
                        sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, mesh, plane,
                            cVertexIndex, aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                    }
                    else if (aSide == PlaneSide.NEGATIVE && bSide == PlaneSide.POSITIVE)
                    {
                        sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, mesh, plane, cVertexIndex,
                           aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                    }
                }
            }

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

        static void sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aOnPlaneIndex, int bPositiveIndex, int cNegativeIndex, List<Vector3> sliceEdgeVertecies)
        {
            float bcD; //normalized distance between B and C
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [bPositiveIndex],
                originalMesh.vertices [cNegativeIndex], out bcD);

            Vector3 bcNormal = Vector3.Lerp (originalMesh.normals [bPositiveIndex], originalMesh.normals [cNegativeIndex], bcD);
            Vector2 bcUV = Vector3.Lerp (originalMesh.normals [bPositiveIndex], originalMesh.normals [cNegativeIndex], bcD);

            sliceEdgeVertecies.Add (originalMesh.vertices [aOnPlaneIndex]);
            sliceEdgeVertecies.Add (bcIntersection);

            aMeshData.Add (originalMesh.vertices [aOnPlaneIndex],
               originalMesh.normals [aOnPlaneIndex], originalMesh.uv [aOnPlaneIndex]);
            aMeshData.Add (originalMesh.vertices [bPositiveIndex],
                originalMesh.normals [bPositiveIndex], originalMesh.uv [bPositiveIndex]);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (originalMesh.vertices [aOnPlaneIndex],
               originalMesh.normals [aOnPlaneIndex], originalMesh.uv [aOnPlaneIndex]);
            bMeshData.Add (bcIntersection, bcNormal, bcUV);
            bMeshData.Add (originalMesh.vertices [cNegativeIndex],
                originalMesh.normals [cNegativeIndex], originalMesh.uv [cNegativeIndex]);
        }

        static void sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aOnPlaneIndex, int bNegativeIndex, int cPositiveIndex, List<Vector3> sliceEdgeVertecies)
        {
            float bcD; //normalized distance between B and C
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [bNegativeIndex],
                originalMesh.vertices [cPositiveIndex], out bcD);

            Vector3 bcNormal = Vector3.Lerp (originalMesh.normals [bNegativeIndex], originalMesh.normals [cPositiveIndex], bcD);
            Vector2 bcUV = Vector3.Lerp (originalMesh.normals [bNegativeIndex], originalMesh.normals [cPositiveIndex], bcD);

            sliceEdgeVertecies.Add (bcIntersection);
            sliceEdgeVertecies.Add (originalMesh.vertices [aOnPlaneIndex]);

            aMeshData.Add (originalMesh.vertices [cPositiveIndex],
               originalMesh.normals [cPositiveIndex], originalMesh.uv [cPositiveIndex]);
            aMeshData.Add (originalMesh.vertices [aOnPlaneIndex],
                originalMesh.normals [aOnPlaneIndex], originalMesh.uv [aOnPlaneIndex]);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (originalMesh.vertices [aOnPlaneIndex],
               originalMesh.normals [aOnPlaneIndex], originalMesh.uv [aOnPlaneIndex]);
            bMeshData.Add (originalMesh.vertices [bNegativeIndex],
                originalMesh.normals [bNegativeIndex], originalMesh.uv [bNegativeIndex]);
            bMeshData.Add (bcIntersection, bcNormal, bcUV);
            
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

