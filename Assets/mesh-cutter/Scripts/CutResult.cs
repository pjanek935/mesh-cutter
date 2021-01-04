using System.Collections.Generic;
using UnityEngine;

namespace MeshCutter
{
    public class CutResult
    {
        public MeshData AMeshData
        {
            get;
            private set;
        }

        public MeshData BMeshData
        {
            get;
            private set;
        }

        public Plane Plane
        {
            get;
            set;
        }

        public List<Vector3> EdgeVertices = new List<Vector3> ();

        public CutResult (MeshData aMeshData, MeshData bMeshData, List <Vector3> edgeVertices, Plane plane)
        {
            this.AMeshData = aMeshData;
            this.BMeshData = bMeshData;
            this.EdgeVertices = edgeVertices;
            this.Plane = plane;
        }

        public Mesh CreateMeshA ()
        {
            return AMeshData.ToMesh (EdgeVertices, 1, Plane);
        }

        public Mesh CreateMeshB ()
        {
            return BMeshData.ToMesh (EdgeVertices, -1, Plane);
        }
    }
}

