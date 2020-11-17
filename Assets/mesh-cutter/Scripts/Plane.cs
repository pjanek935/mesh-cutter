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
        public float GetSide (Vector3 point)
        {
            return Vector3.Dot (Normal, Origin - point);
        }
    }
}

