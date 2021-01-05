using UnityEngine;

namespace MeshCutter
{
    public class CutData
    {
        public Cuttable Parent;
        public Cuttable Child;
        public Plane Plane;
        public Vector3 CutDirection;
        public SimpleMesh SimpleMesh;
        public CutResult CutResult;
    }
}

