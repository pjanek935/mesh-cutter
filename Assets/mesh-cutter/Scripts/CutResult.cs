using UnityEngine;

namespace MeshCutter
{
    public class CutResult
    {
        public Mesh AMesh
        {
            get;
            private set;
        }

        public Mesh BMesh
        {
            get;
            private set;
        }

        public CutResult (Mesh aMesh, Mesh bMesh)
        {
            this.AMesh = aMesh;///
            this.BMesh = bMesh;
        }
    }
}

