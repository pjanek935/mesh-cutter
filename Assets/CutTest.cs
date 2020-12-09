using MeshCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutTest : MonoBehaviour
{
    [SerializeField] MeshFilter meshToCut;
    [SerializeField] MeshFilter meshA;
    [SerializeField] MeshFilter meshB;
    [SerializeField] GameObject plane;

    void cut ()
    {
        MeshCutter.Plane planeCutter = new MeshCutter.Plane (plane.transform, meshToCut.transform);
        CutResult result = Cutter.Cut (meshToCut.mesh, planeCutter);
        meshA.mesh = result.AMesh;
        meshB.mesh = result.BMesh;

        meshA.transform.localScale = meshToCut.transform.localScale;
        meshB.transform.localScale = meshToCut.transform.localScale;
        meshA.transform.rotation = meshToCut.transform.rotation;
        meshB.transform.rotation = meshToCut.transform.rotation;
    }

    protected void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            cut ();
        }
    }
}
