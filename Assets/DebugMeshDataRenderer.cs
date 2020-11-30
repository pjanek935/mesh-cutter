using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMeshDataRenderer : MonoBehaviour
{
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
                    Vector3 origin = transform.position + mesh.vertices [i];
                    Debug.DrawLine (origin, origin + mesh.normals [i] * 0.2f, Color.red);
                }

                if (Input.GetKeyDown (KeyCode.R))
                {
                    Debug.Log ("Vertices count: " + mesh.vertices.Length);
                }
            }
        }
    }
}
