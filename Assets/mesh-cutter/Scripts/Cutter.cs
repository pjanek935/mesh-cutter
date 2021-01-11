using System.Collections.Generic;
using UnityEngine;

namespace MeshCutter
{
    public class Cutter
    {
        public static CutResult Cut (int [] meshTriangles, Vector3 [] meshVertives, Vector3 [] meshNormals, Vector2 [] meshUVs, Plane plane)
        {
            MeshData aMeshData = new MeshData ();
            MeshData bMeshData = new MeshData ();
            List<Vector3> sliceEdgeVertecies = new List<Vector3> ();

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                int aVertexIndex = meshTriangles [i];
                int bVertexIndex = meshTriangles [i + 1];
                int cVertexIndex = meshTriangles [i + 2];

                PlaneSide aSide = plane.GetSide (meshVertives [aVertexIndex]);
                PlaneSide bSide = plane.GetSide (meshVertives [bVertexIndex]);
                PlaneSide cSide = plane.GetSide (meshVertives [cVertexIndex]);

                if (areAllVerteciesOnTheSameSide (aSide, bSide, cSide, PlaneSide.POSITIVE))
                {
                    bool addUV = meshUVs.Length > 0;

                    aMeshData.Add (meshVertives [aVertexIndex], meshNormals [aVertexIndex], addUV ? meshUVs [aVertexIndex] : Vector2.zero);
                    aMeshData.Add (meshVertives [bVertexIndex], meshNormals [bVertexIndex], addUV ? meshUVs [bVertexIndex] : Vector2.zero);
                    aMeshData.Add (meshVertives [cVertexIndex], meshNormals [cVertexIndex], addUV ? meshUVs [cVertexIndex] : Vector2.zero);

                    if (aSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (meshVertives [aVertexIndex]);
                    }

                    if (bSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (meshVertives [bVertexIndex]);
                    }

                    if (cSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (meshVertives [cVertexIndex]);
                    }
                }
                else if (areAllVerteciesOnTheSameSide (aSide, bSide, cSide, PlaneSide.NEGATIVE))
                {
                    bool addUV = meshUVs.Length > 0;

                    bMeshData.Add (meshVertives [aVertexIndex], meshNormals [aVertexIndex], addUV ? meshUVs [aVertexIndex] : Vector2.zero);
                    bMeshData.Add (meshVertives [bVertexIndex], meshNormals [bVertexIndex], addUV ? meshUVs [bVertexIndex] : Vector2.zero);
                    bMeshData.Add (meshVertives [cVertexIndex], meshNormals [cVertexIndex], addUV ? meshUVs [cVertexIndex] : Vector2.zero);

                    if (aSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (meshVertives [aVertexIndex]);
                    }

                    if (bSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (meshVertives [bVertexIndex]);
                    }

                    if (cSide == PlaneSide.ON_PLANE)
                    {
                        sliceEdgeVertecies.Add (meshVertives [cVertexIndex]);
                    }
                }
                else if (aSide == PlaneSide.POSITIVE && bSide == PlaneSide.NEGATIVE && cSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs,
                        plane, aVertexIndex, bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide == PlaneSide.POSITIVE && cSide == PlaneSide.NEGATIVE && aSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs,
                        plane, bVertexIndex, cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide == PlaneSide.POSITIVE && aSide == PlaneSide.NEGATIVE && bSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs,
                        plane, cVertexIndex, aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
                else if (aSide == PlaneSide.NEGATIVE && bSide == PlaneSide.POSITIVE && cSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane, bVertexIndex,
                        cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide == PlaneSide.NEGATIVE && cSide == PlaneSide.POSITIVE && aSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane, cVertexIndex,
                        aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide == PlaneSide.NEGATIVE && aSide == PlaneSide.POSITIVE && bSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane, aVertexIndex,
                        bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                }
                else if (aSide == PlaneSide.ON_PLANE && bSide == PlaneSide.POSITIVE && cSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane, aVertexIndex, bVertexIndex,
                            cVertexIndex, sliceEdgeVertecies);
                }
                else if (aSide == PlaneSide.ON_PLANE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane, aVertexIndex,
                            bVertexIndex, cVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide == PlaneSide.ON_PLANE && cSide == PlaneSide.POSITIVE && aSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane,
                            bVertexIndex, cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (bSide == PlaneSide.ON_PLANE && cSide == PlaneSide.NEGATIVE && aSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane, bVertexIndex,
                                               cVertexIndex, aVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide == PlaneSide.ON_PLANE && aSide == PlaneSide.POSITIVE && bSide == PlaneSide.NEGATIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane,
                            cVertexIndex, aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
                else if (cSide == PlaneSide.ON_PLANE && aSide == PlaneSide.NEGATIVE && bSide == PlaneSide.POSITIVE)
                {
                    sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (aMeshData, bMeshData, meshVertives, meshNormals, meshUVs, plane, cVertexIndex,
                            aVertexIndex, bVertexIndex, sliceEdgeVertecies);
                }
            }

            return new CutResult (aMeshData, bMeshData, sliceEdgeVertecies, plane);
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

        static void sliceTriangleWhenTwoVerticesAreOnAPositiveHalfSpace (MeshData aMeshData, MeshData bMeshData, 
            Vector3 [] originalMeshVertices, Vector3 [] originalMeshNormals, Vector2 [] originalMeshUVs, 
            Plane plane, int aPositiveIndex, int bPositiveIndex, int cNegativeIndex, List <Vector3> sliceEdgeVertices)
        {
            bool addUV = originalMeshUVs.Length > 0;

            float acD; //normalized distance between A and C
            Vector3 acIntersection = getPlaneIntersectionPoint (plane, originalMeshVertices [aPositiveIndex],
                originalMeshVertices [cNegativeIndex], out acD);

            float bcD;
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMeshVertices [bPositiveIndex],
                originalMeshVertices [cNegativeIndex], out bcD);

            Vector3 acNormal = Vector3.Lerp (originalMeshNormals [aPositiveIndex], originalMeshNormals [cNegativeIndex], acD);
            Vector2 acUV = Vector2.zero;

            if (addUV)
            {
                acUV = Vector2.Lerp (originalMeshUVs [aPositiveIndex], originalMeshUVs [cNegativeIndex], acD);
            }

            Vector3 bcNormal = Vector3.Lerp (originalMeshNormals [bPositiveIndex], originalMeshNormals [cNegativeIndex], bcD);
            Vector2 bcUV = Vector2.zero;

            if (addUV)
            {
                bcUV = Vector2.Lerp (originalMeshUVs [bPositiveIndex], originalMeshUVs [cNegativeIndex], bcD);
            }
           
            sliceEdgeVertices.Add (acIntersection);
            sliceEdgeVertices.Add (bcIntersection);

            aMeshData.Add (originalMeshVertices [aPositiveIndex],
                originalMeshNormals [aPositiveIndex], addUV ? originalMeshUVs [aPositiveIndex] : Vector2.zero);
            aMeshData.Add (originalMeshVertices [bPositiveIndex],
               originalMeshNormals [bPositiveIndex], addUV ? originalMeshUVs [bPositiveIndex] : Vector2.zero);
            aMeshData.Add (acIntersection, acNormal, acUV);

            aMeshData.Add (acIntersection, acNormal, acUV);
            aMeshData.Add (originalMeshVertices [bPositiveIndex],
              originalMeshNormals [bPositiveIndex], addUV ? originalMeshUVs [bPositiveIndex] : Vector2.zero);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (bcIntersection, bcNormal, bcUV);
            bMeshData.Add (originalMeshVertices [cNegativeIndex],
             originalMeshNormals [cNegativeIndex], addUV ? originalMeshUVs [cNegativeIndex] : Vector2.zero);
        }

        static void sliceTriangleWhenOneVertexIsOnAPositiveHalfSpace (MeshData aMeshData, MeshData bMeshData, 
            Vector3 [] originalMeshVertices, Vector3 [] originalMeshNormals, Vector2 [] originalMeshUVs, 
            Plane plane, int aPositiveIndex, int bNegativeIndex, int cNegativeIndex, List <Vector3> sliceEdgeVertecies)
        {
            bool addUV = originalMeshUVs.Length > 0;

            float abD; //normalized distance between A and B
            Vector3 abIntersection = getPlaneIntersectionPoint (plane, originalMeshVertices [aPositiveIndex],
                originalMeshVertices [bNegativeIndex], out abD);

            float acD;
            Vector3 acIntersection = getPlaneIntersectionPoint (plane, originalMeshVertices [aPositiveIndex],
                originalMeshVertices [cNegativeIndex], out acD);

            Vector3 abNormal = Vector3.Lerp (originalMeshNormals [aPositiveIndex], originalMeshNormals [bNegativeIndex], abD);
            Vector2 abUV = Vector2.zero;

            if (addUV)
            {
                abUV = Vector2.Lerp (originalMeshUVs [aPositiveIndex], originalMeshUVs [bNegativeIndex], abD);
            }

            Vector3 acNormal = Vector3.Lerp (originalMeshNormals [aPositiveIndex], originalMeshNormals [cNegativeIndex], acD);
            Vector2 acUV = Vector2.zero;
            
            if (addUV)
            {
                acUV = Vector2.Lerp (originalMeshUVs [aPositiveIndex], originalMeshUVs [cNegativeIndex], acD);
            }

            sliceEdgeVertecies.Add (acIntersection);
            sliceEdgeVertecies.Add (abIntersection);

            aMeshData.Add (originalMeshVertices [aPositiveIndex],
                originalMeshNormals [aPositiveIndex], addUV ? originalMeshUVs [aPositiveIndex] : Vector2.zero);
            aMeshData.Add (abIntersection, abNormal, abUV);
            aMeshData.Add (acIntersection, acNormal, acUV);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (abIntersection, abNormal, abUV);
            bMeshData.Add (originalMeshVertices [bNegativeIndex],
                originalMeshNormals [bNegativeIndex], addUV ? originalMeshUVs [bNegativeIndex] : Vector2.zero);

            bMeshData.Add (acIntersection, acNormal, acUV);
            bMeshData.Add (originalMeshVertices [bNegativeIndex], originalMeshNormals [bNegativeIndex],
                addUV ? originalMeshUVs [bNegativeIndex] : Vector2.zero);
            bMeshData.Add (originalMeshVertices [cNegativeIndex], originalMeshNormals [cNegativeIndex],
                addUV ? originalMeshUVs [cNegativeIndex] : Vector2.zero);
        }

        static void sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnAPositiveHalfspace (MeshData aMeshData, MeshData bMeshData,
            Vector3 [] originalMeshVertices, Vector3 [] originalMeshNormals, Vector2 [] originalMeshUVs,
            Plane plane, int aOnPlaneIndex, int bPositiveIndex, int cNegativeIndex, List<Vector3> sliceEdgeVertecies)
        {
            bool addUV = originalMeshUVs.Length > 0;

            float bcD; //normalized distance between B and C
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMeshVertices [bPositiveIndex],
                originalMeshVertices [cNegativeIndex], out bcD);

            Vector3 bcNormal = Vector3.Lerp (originalMeshNormals [bPositiveIndex], originalMeshNormals [cNegativeIndex], bcD);
            Vector2 bcUV = Vector2.zero;

            if (addUV)
            {
                bcUV = Vector2.Lerp (originalMeshUVs [bPositiveIndex], originalMeshUVs [cNegativeIndex], bcD);
            }

            sliceEdgeVertecies.Add (originalMeshVertices [aOnPlaneIndex]);
            sliceEdgeVertecies.Add (bcIntersection);

            aMeshData.Add (originalMeshVertices [aOnPlaneIndex],
               originalMeshNormals [aOnPlaneIndex], addUV ? originalMeshUVs [aOnPlaneIndex] : Vector2.zero);
            aMeshData.Add (originalMeshVertices [bPositiveIndex],
                originalMeshNormals [bPositiveIndex], addUV ? originalMeshUVs [bPositiveIndex] : Vector2.zero);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (originalMeshVertices [aOnPlaneIndex],
               originalMeshNormals [aOnPlaneIndex], addUV ? originalMeshUVs [aOnPlaneIndex] : Vector2.zero);
            bMeshData.Add (bcIntersection, bcNormal, bcUV);
            bMeshData.Add (originalMeshVertices [cNegativeIndex],
                originalMeshNormals [cNegativeIndex], addUV ? originalMeshUVs [cNegativeIndex] : Vector2.zero);
        }

        static void sliceTriangleWhenOneVertexIsOnAPlaneAndNextIsOnANegativeHalfspace (MeshData aMeshData, MeshData bMeshData,
            Vector3 [] originalMeshVertices, Vector3 [] originalMeshNormals, Vector2 [] orignalMeshUVs, 
            Plane plane, int aOnPlaneIndex, int bNegativeIndex, int cPositiveIndex, List<Vector3> sliceEdgeVertecies)
        {
            bool addUV = orignalMeshUVs.Length > 0;

            float bcD; //normalized distance between B and C
            Vector3 bcIntersection = getPlaneIntersectionPoint (plane, originalMeshVertices [bNegativeIndex],
                originalMeshVertices [cPositiveIndex], out bcD);

            Vector3 bcNormal = Vector3.Lerp (originalMeshNormals [bNegativeIndex], originalMeshNormals [cPositiveIndex], bcD);
            Vector2 bcUV = Vector2.zero;

            if (addUV)
            {
                bcUV = Vector2.Lerp (orignalMeshUVs [bNegativeIndex], orignalMeshUVs [cPositiveIndex], bcD);
            }

            sliceEdgeVertecies.Add (bcIntersection);
            sliceEdgeVertecies.Add (originalMeshVertices [aOnPlaneIndex]);

            aMeshData.Add (originalMeshVertices [cPositiveIndex],
               originalMeshNormals [cPositiveIndex], addUV ? orignalMeshUVs [cPositiveIndex] : Vector2.zero);
            aMeshData.Add (originalMeshVertices [aOnPlaneIndex],
                originalMeshNormals [aOnPlaneIndex], addUV ? orignalMeshUVs [aOnPlaneIndex] : Vector2.zero);
            aMeshData.Add (bcIntersection, bcNormal, bcUV);

            bMeshData.Add (originalMeshVertices [aOnPlaneIndex],
               originalMeshNormals [aOnPlaneIndex], addUV ? orignalMeshUVs [aOnPlaneIndex] : Vector2.zero);
            bMeshData.Add (originalMeshVertices [bNegativeIndex],
                originalMeshNormals [bNegativeIndex], addUV ? orignalMeshUVs [bNegativeIndex] : Vector2.zero);
            bMeshData.Add (bcIntersection, bcNormal, bcUV);
            
        }

        /// <summary>
        /// D is a normalized distance between lineStart and intersection point
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
            Vector3 result = lineStart + lineDirection * d;
            d /= Vector3.Distance (lineStart, lineEnd); //normalizing distance

            return result;
        }
    }
}

