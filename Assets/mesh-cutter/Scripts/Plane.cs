using UnityEngine;

namespace MeshCutter
{
    public struct Plane
    {
        public Vector3 Normal;
        public Vector3 Origin;

        /// <summary>
        /// Return positive number if given point is on the same side of plane that
        /// plane normal is facing. Negative number otherwise.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public PlaneSide GetSide (Vector3 point)
        {
            PlaneSide result = PlaneSide.INVALID;
            float dot = Vector3.Dot (Normal, Origin - point);

            if (dot == 0)
            {
                result = PlaneSide.ON_PLANE;
            }
            else if (dot > 0)
            {
                result = PlaneSide.POSITIVE;
            }
            else
            {
                result = PlaneSide.NEGATIVE;
            }

            return result;
        }
    }
}

