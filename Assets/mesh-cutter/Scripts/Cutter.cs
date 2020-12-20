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

                if (areAllVerteciesOnTheSameSide (aSide, bSide, cSide, PlaneSide.POSITIVE))
                {
                    bool addUV = mesh.uv.Length > 0;

                    aMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], addUV ? mesh.uv [aVertexIndex] : Vector2.zero);
                    aMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], addUV ? mesh.uv [bVertexIndex] : Vector2.zero);
                    aMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], addUV ? mesh.uv [cVertexIndex] : Vector2.zero);

                    if (aSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (mesh.vertices [aVertexIndex]);
                    }

                    if (bSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (mesh.vertices [bVertexIndex]);
                    }

                    if (cSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (mesh.vertices [cVertexIndex]);
                    }
                }
                else if (areAllVerteciesOnTheSameSide (aSide, bSide, cSide, PlaneSide.NEGATIVE))
                {
                    bool addUV = mesh.uv.Length > 0;

                    bMeshData.Add (mesh.vertices [aVertexIndex], mesh.normals [aVertexIndex], addUV ? mesh.uv [aVertexIndex] : Vector2.zero);
                    bMeshData.Add (mesh.vertices [bVertexIndex], mesh.normals [bVertexIndex], addUV ? mesh.uv [bVertexIndex] : Vector2.zero);
                    bMeshData.Add (mesh.vertices [cVertexIndex], mesh.normals [cVertexIndex], addUV ? mesh.uv [cVertexIndex] : Vector2.zero);

                    if (aSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (mesh.vertices [aVertexIndex]);
                    }

                    if (bSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (mesh.vertices [bVertexIndex]);
                    }

                    if (cSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (mesh.vertices [cVertexIndex]);
                    }
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
                else if (aSide == PlaneSide.ON_PLANE && bSide == PlaneSide.POSITIVE && cSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, mesh, plane, aVertexIndex, bVertexIndex,
                            cVertexIndex, sliceEdgeVertecies);
                }
                else if (aSide == PlaneSide.ON_PLANE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, mesh, plane, aVertexIndex,
                            bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide == PlaneSide.ON_PLANE && cSide == PlaneSide.POSITIVE && aSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, mesh, plane,
                            bVertexIndex, cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide == PlaneSide.ON_PLANE && cSide == PlaneSide.NEGATIVE && aSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, mesh, plane, bVertexIndex,
                                               cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide == PlaneSide.ON_PLANE && aSide == PlaneSide.POSITIVE && bSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, mesh, plane,
                            cVertexIndex, aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide == PlaneSide.ON_PLANE && aSide == PlaneSide.NEGATIVE && bSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, mesh, plane, cVertexIndex,
                            aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
            }

            return new CutResult (aMeshData.ToMesh (sliceEdgeVertecies, 1, plane),
                bMeshData.ToMesh (sliceEdgeVertecies , - 1, plane));
        }

        static bool areAllVerteciesOnTheSameSide (PlaneSide aSide, PlaneSide bSide, PlaneSide cSide, PlaneSide side)
        {
            bool result = false;

            if (aSide == side &&
                bSide == side &&
                cSide == side)
            {
                result = true;
            }
            else if (aSide == PlaneSide.ON_PLANE &&
                bSide == side &&
                cSide == side)
            {
                result = true;
            }
            else if (bSide == PlaneSide.ON_PLANE &&
                cSide == side &&
                aSide == side)
            {
                result = true;
            }
            else if (cSide == PlaneSide.ON_PLANE &&
                aSide == side &&
                bSide == side)
            {
                result = true;
            }
            else if (aSide == PlaneSide.ON_PLANE &&
                bSide == PlaneSide.ON_PLANE &&
                cSide == side)
            {
                result = true;
            }
            else if (bSide == PlaneSide.ON_PLANE &&
                cSide == PlaneSide.ON_PLANE &&
                aSide == side)
            {
                result = true;
            }
            else if (cSide == PlaneSide.ON_PLANE
                && aSide == PlaneSide.ON_PLANE
                && bSide == side)
            {
                result = true;
            }

            return result;
        }

        static void sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aPositiveIndex, int bPositiveIndex, int cNegativeIndex, List <Vector3> sliceEdgeVertices)
        {
            bool addUV = originalMesh.uv.Length > 0;

            float acD; //normalized distance between A and C
            Vector3 acIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [aPositiveIndex],
                originalMesh.vertices [cNegativeIndex], out acD);

            float bcD;
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [bPositiveIndex],
                originalMesh.vertices [cNegativeIndex], out bcD);

            Vector3 acNormal = Vector3.Lerp (originalMesh.normals [aPositiveIndex], originalMesh.normals [cNegativeIndex], acD);
            Vector2 acUV = Vector2.zero;

            if (addUV)
            {
                acUV = Vector2.Lerp (originalMesh.uv [aPositiveIndex], originalMesh.uv [cNegativeIndex], acD * 4.54f);
            }

            Vector3 bcNormal = Vector3.Lerp (originalMesh.normals [bPositiveIndex], originalMesh.normals [cNegativeIndex], bcD);
            Vector2 bcUV = Vector2.zero;

            if (addUV)
            {
                bcUV = Vector3.Lerp (originalMesh.uv [bPositiveIndex], originalMesh.uv [cNegativeIndex], bcD * 4.54f);
            }
           
            sliceEdgeVertices.Add (acIntersection);
            sliceEdgeVertices.Add (bcIntersection);

            aMeshData.Add (originalMesh.vertices [aPositiveIndex],
                originalMesh.normals [aPositiveIndex], addUV ? originalMesh.uv [aPositiveIndex] : Vector2.zero);
            aMeshData.Add (originalMesh.vertices [bPositiveIndex],
               originalMesh.normals [bPositiveIndex], addUV ? originalMesh.uv [bPositiveIndex] : Vector2.zero);
            aMeshData.Add (acIntersection, acNormal, acUV);

            aMeshData.Add (acIntersection, acNormal, acUV);
            aMeshData.Add (originalMesh.vertices [bPositiveIndex],
              originalMesh.normals [bPositiveIndex], addUV ? originalMesh.uv [bPositiveIndex] : Vector2.zero);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (bcIntersection, bcNormal, bcUV);
            bMeshData.Add (originalMesh.vertices [cNegativeIndex],
             originalMesh.normals [cNegativeIndex], addUV ? originalMesh.uv [cNegativeIndex] : Vector2.zero);
        }

        static void sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aPositiveIndex, int bNegativeIndex, int cNegativeIndex, List <Vector3> sliceEdgeVertecies)
        {
            bool addUV = originalMesh.uv.Length > 0;

            float abD; //normalized distance between A and B
            Vector3 abIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [aPositiveIndex],
                originalMesh.vertices [bNegativeIndex], out abD);

            float acD;
            Vector3 acIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [aPositiveIndex],
                originalMesh.vertices [cNegativeIndex], out acD);

            Vector3 abNormal = Vector3.Lerp (originalMesh.normals [aPositiveIndex], originalMesh.normals [bNegativeIndex], abD);
            Vector2 abUV = Vector2.zero;

            if (addUV)
            {
                abUV = Vector3.Lerp (originalMesh.uv [aPositiveIndex], originalMesh.uv [bNegativeIndex], abD * 4.54f) ;
            }

            Vector3 acNormal = Vector3.Lerp (originalMesh.normals [aPositiveIndex], originalMesh.normals [cNegativeIndex], acD);
            Vector2 acUV = Vector2.zero;
            
            if (addUV)
            {
                acUV = Vector3.Lerp (originalMesh.uv [aPositiveIndex], originalMesh.uv [cNegativeIndex], acD * 4.54f);
            }

            sliceEdgeVertecies.Add (acIntersection);
            sliceEdgeVertecies.Add (abIntersection);

            aMeshData.Add (originalMesh.vertices [aPositiveIndex], 
                originalMesh.normals [aPositiveIndex], addUV ? originalMesh.uv [aPositiveIndex] : Vector2.zero);
            aMeshData.Add (abIntersection, abNormal, abUV);
            aMeshData.Add (acIntersection, acNormal, acUV);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (abIntersection, abNormal, abUV);
            bMeshData.Add (originalMesh.vertices [bNegativeIndex],
                originalMesh.normals [bNegativeIndex], addUV ? originalMesh.uv [bNegativeIndex] : Vector2.zero);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (originalMesh.vertices [bNegativeIndex], originalMesh.normals [bNegativeIndex],
                addUV ? originalMesh.uv [bNegativeIndex] : Vector2.zero);
            bMeshData.Add (originalMesh.vertices [cNegativeIndex], originalMesh.normals [cNegativeIndex],
                addUV ? originalMesh.uv [cNegativeIndex] : Vector2.zero);
        }

        static void sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aOnPlaneIndex, int bPositiveIndex, int cNegativeIndex, List<Vector3> sliceEdgeVertecies)
        {
            bool addUV = originalMesh.uv.Length > 0;

            float bcD; //normalized distance between B and C
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [bPositiveIndex],
                originalMesh.vertices [cNegativeIndex], out bcD);

            Vector3 bcNormal = Vector3.Lerp (originalMesh.normals [bPositiveIndex], originalMesh.normals [cNegativeIndex], bcD);
            Vector2 bcUV = Vector2.zero;

            if (addUV)
            {
                bcUV = Vector3.Lerp (originalMesh.uv [bPositiveIndex], originalMesh.uv [cNegativeIndex], bcD);
            }

            sliceEdgeVertecies.Add (originalMesh.vertices [aOnPlaneIndex]);
            sliceEdgeVertecies.Add (bcIntersection);

            aMeshData.Add (originalMesh.vertices [aOnPlaneIndex],
               originalMesh.normals [aOnPlaneIndex], addUV ? originalMesh.uv [aOnPlaneIndex] : Vector2.zero);
            aMeshData.Add (originalMesh.vertices [bPositiveIndex],
                originalMesh.normals [bPositiveIndex], addUV ? originalMesh.uv [bPositiveIndex] : Vector2.zero);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (originalMesh.vertices [aOnPlaneIndex],
               originalMesh.normals [aOnPlaneIndex], addUV ? originalMesh.uv [aOnPlaneIndex] : Vector2.zero);
            bMeshData.Add (bcIntersection, bcNormal, bcUV);
            bMeshData.Add (originalMesh.vertices [cNegativeIndex],
                originalMesh.normals [cNegativeIndex], addUV ? originalMesh.uv [cNegativeIndex] : Vector2.zero);
        }

        static void sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (MeshData aMeshData, MeshData bMeshData, Mesh originalMesh, Plane plane,
            int aOnPlaneIndex, int bNegativeIndex, int cPositiveIndex, List<Vector3> sliceEdgeVertecies)
        {
            bool addUV = originalMesh.uv.Length > 0;

            float bcD; //normalized distance between B and C
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMesh.vertices [bNegativeIndex],
                originalMesh.vertices [cPositiveIndex], out bcD);

            Vector3 bcNormal = Vector3.Lerp (originalMesh.normals [bNegativeIndex], originalMesh.normals [cPositiveIndex], bcD);
            Vector2 bcUV = Vector2.zero;

            if (addUV)
            {
                bcUV = Vector3.Lerp (originalMesh.uv [bNegativeIndex], originalMesh.uv [cPositiveIndex], bcD);
            }

            sliceEdgeVertecies.Add (bcIntersection);
            sliceEdgeVertecies.Add (originalMesh.vertices [aOnPlaneIndex]);

            aMeshData.Add (originalMesh.vertices [cPositiveIndex],
               originalMesh.normals [cPositiveIndex], addUV ? originalMesh.uv [cPositiveIndex] : Vector2.zero);
            aMeshData.Add (originalMesh.vertices [aOnPlaneIndex],
                originalMesh.normals [aOnPlaneIndex], addUV ? originalMesh.uv [aOnPlaneIndex] : Vector2.zero);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (originalMesh.vertices [aOnPlaneIndex],
               originalMesh.normals [aOnPlaneIndex], addUV ? originalMesh.uv [aOnPlaneIndex] : Vector2.zero);
            bMeshData.Add (originalMesh.vertices [bNegativeIndex],
                originalMesh.normals [bNegativeIndex], addUV ? originalMesh.uv [bNegativeIndex] : Vector2.zero);
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

