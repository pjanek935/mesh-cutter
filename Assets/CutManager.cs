using MeshCutter;
using UnityEngine;

public class CutManager : MonoBehaviour
{
    public void Cut (GameObject cutter, Cuttable objectToCut)
    {
        if (cutter == null || objectToCut == null) return;

        MeshCutter.Plane cuttingPlane = new MeshCutter.Plane (cutter.transform, objectToCut.transform);
        CutResult cutResult = Cutter.Cut (objectToCut.Mesh, cuttingPlane);

        GameObject aObject = instantiateNewGameObject ();
        GameObject bObject = instantiateNewGameObject ();

        aObject.GetComponent<MeshFilter> ().mesh = cutResult.AMesh;
        bObject.GetComponent<MeshFilter> ().mesh = cutResult.BMesh;

        aObject.transform.SetParent (objectToCut.transform.parent);
        bObject.transform.SetParent (objectToCut.transform.parent);
    }

    GameObject instantiateNewGameObject ()
    {
        GameObject gameObject = new GameObject ();
        gameObject.AddComponent<MeshFilter> ();

        return gameObject;
    }
}
