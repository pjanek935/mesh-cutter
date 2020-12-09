using UnityEngine;

namespace MeshCutter
{
    public struct Plane
    {
        public Vector3 Normal;
        public Vector3 Origin;

        public Plane (Vector3 origin, Vector3 normal)
        {
            this.Origin = origin;
            this.Normal = normal;
        }

        public Plane (Transform planeTransform, Transform objectToCutTransform)
        {
            if (planeTransform != null && objectToCutTransform != null)
            {
                this.Origin = objectToCutTransform.InverseTransformPoint (planeTransform.position);
                this.Normal = objectToCutTransform.InverseTransformVector (planeTransform.up);
            }
            else
            {
                this.Origin = Vector3.zero;
                this.Normal = Vector3.zero;
            }
        }

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

        public void Transform (Transform transform)
        {
            this.Origin = transform.InverseTransformPoint (this.Origin);
            this.Normal = transform.InverseTransformVector (this.Normal);
        }
    }
}

