using System.Collections.Generic;
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

        public List<Vector3> EdgeVertices = new List<Vector3> ();

        public CutResult (Mesh aMesh, Mesh bMesh, List <Vector3> edgeVertices)
        {
            this.AMesh = aMesh;
            this.BMesh = bMesh;
            this.EdgeVertices = edgeVertices;
        }
    }
}

