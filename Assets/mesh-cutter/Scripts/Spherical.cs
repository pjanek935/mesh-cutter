using UnityEngine;

namespace MeshCutter
{
    public struct Spherical
    {
        public float R;
        public float Theta;
        public float Phi;

        public Spherical (Vector3 vector3)
        {
            R = Mathf.Sqrt (Mathf.Pow (vector3.x, 2) +
                       Mathf.Pow (vector3.z, 2) +
                       Mathf.Pow (vector3.y, 2));
            Theta = Mathf.Atan2 (vector3.z, vector3.x);
            Phi = Mathf.Acos (vector3.y / R);
        }

        public Vector3 ToVector3 ()
        {
            Vector3 result = Vector3.zero;
            result.x = R * Mathf.Sin (Phi) * Mathf.Cos (Theta);
            result.z = R * Mathf.Sin (Phi) * Mathf.Sin (Theta);
            result.y = R * Mathf.Cos (Phi);

            return result;
        }
    }
}

