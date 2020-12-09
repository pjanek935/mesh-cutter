using UnityEngine;

[RequireComponent (typeof (MeshFilter))]
public class Cuttable : MonoBehaviour
{
    MeshFilter meshFilter;

    public Mesh Mesh
    {
        get { return meshFilter.mesh; }
    }

    private void Awake ()
    {
        meshFilter = GetComponent<MeshFilter> ();
    }
}
