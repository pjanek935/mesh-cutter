using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMeshDataRenderer : MonoBehaviour
{
    const float lineLength = 0.2f;
    // Update is called once per frame
    void Update()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter> ();

        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;

            if (mesh != null)
            {
                for (int i = 0; i < mesh.normals.Length; i++)
                {
                    Vector3 origin = mesh.vertices [i];
                    origin = transform.TransformPoint (origin);
                    Vector3 normal = transform.TransformVector (mesh.normals [i]);
                    Debug.DrawLine (origin, origin + normal * lineLength, Color.red);
                }

                if (Input.GetKeyDown (KeyCode.R))
                {
                    Debug.Log ("Vertices count: " + mesh.vertices.Length);
                }
            }
        }
    }
}
