using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRenderer : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (KeyCode.R))
        {
            MeshFilter meshFilter = GetComponent<MeshFilter> ();
            Mesh mesh = meshFilter.mesh;

            for (int i = 0; i < mesh.normals.Length; i ++)
            {
                Vector3 origin = transform.position + mesh.vertices [i];
                Debug.DrawLine (origin, origin + mesh.normals [i] * 1, Color.green, 20);
            }

            Debug.Log ("Vertices count: " + mesh.vertices.Length);
        }
    }
}
