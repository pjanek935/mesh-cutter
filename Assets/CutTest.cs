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
        CutResult result = Cutter.Cut (meshToCut.mesh,
            new MeshCutter.Plane () { Origin = plane.transform.position, Normal = plane.transform.up });
        meshA.mesh = result.AMesh;
        meshB.mesh = result.BMesh;
    }

    protected void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            cut ();
        }
    }
}
