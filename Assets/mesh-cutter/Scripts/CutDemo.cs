using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCutter
{
    public class CutDemo : MonoBehaviour
    {
        [SerializeField] GameObject childPrefab;
        [SerializeField] VirtualKatana virtualKatana;
        [SerializeField] Cuttable origianlObject;
        [SerializeField] Transform origin;

        List<Cuttable> activatedChildren = new List<Cuttable> ();
        Queue<Cuttable> deactivatedChildrenPool = new Queue<Cuttable> ();
        const int childrenCount = 25;

        System.Random random = new System.Random ();
        float torqueRange = 0.1f;

        void initChildren ()
        {
            for (int i = 0; i < childrenCount; i ++)
            {
                GameObject newGameObject = Instantiate (childPrefab);
                newGameObject.SetActive (false);
                deactivatedChildrenPool.Enqueue (newGameObject.GetComponent <Cuttable> ());
            }
        }

        private void Awake ()
        {
            initChildren ();
        }

        private void OnEnable ()
        {
            virtualKatana.OnCutTriggered += onCutTriggered;
        }

        private void OnDisable ()
        {
            virtualKatana.OnCutTriggered -= onCutTriggered;
        }

        void cut (GameObject parent, Plane cuttingPlane, Vector3 cutDirection)
        {
            MeshFilter parentMeshFilter = parent.GetComponent<MeshFilter> ();
            CutResult cutResult = Cutter.Cut (parentMeshFilter.mesh, cuttingPlane);
            Mesh newOriginalMesh = cutResult.AMesh;
            Mesh childMesh = cutResult.BMesh;

            if (newOriginalMesh.vertexCount == 0 || childMesh.vertexCount == 0)
            {
                return;
            }

            bool isOriginalTatami = string.Equals (parent.tag, "OriginalTatami");

            if (isOriginalTatami)
            {
                Vector3 aPos = parent.transform.TransformPoint (cutResult.AMesh.vertices [0]);
                Vector3 bPos = parent.transform.TransformPoint (cutResult.BMesh.vertices [0]);
                float aD = Vector3.Distance (origin.position, aPos);
                float bD = Vector3.Distance (origin.position, bPos);

                if (bD < aD)
                {
                    newOriginalMesh = cutResult.BMesh;
                    childMesh = cutResult.AMesh;
                }
            }

            parentMeshFilter.mesh = newOriginalMesh;
            MeshCollider meshCollider = parent.GetComponent<MeshCollider> ();
            meshCollider.sharedMesh = newOriginalMesh;

            if (deactivatedChildrenPool.Count > 0)
            {
                GameObject gameObject = deactivatedChildrenPool.Dequeue ().gameObject;
                gameObject.transform.position = parent.transform.position;
                gameObject.transform.rotation = parent.transform.rotation;
                gameObject.transform.localScale = parent.transform.localScale;
                activatedChildren.Add (gameObject.GetComponent <Cuttable> ());
                parentMeshFilter = gameObject.GetComponent<MeshFilter> ();
                parentMeshFilter.mesh = childMesh;
                meshCollider = gameObject.GetComponent<MeshCollider> ();
                meshCollider.convex = true;
                meshCollider.sharedMesh = childMesh;
                gameObject.SetActive (true);
                Rigidbody rb = gameObject.GetComponent<Rigidbody> ();

                Vector3 forceVector = Vector3.up * 2 + cutDirection * 7;
                forceVector.Normalize ();
                forceVector *= 1.5f;
                rb.AddForce (forceVector, ForceMode.Impulse);
                rb.AddTorque (new Vector3 ((float) random.NextDouble () * torqueRange / 2f,
                    (float) random.NextDouble () * torqueRange / 2f,
                    ((float) random.NextDouble () * torqueRange / 2f) * 0.5f), ForceMode.Impulse);
            }
        }

        void onCutTriggered (Transform planeTransform)
        {
            if (origianlObject.IsTouchingBlade)
            {
                MeshCutter.Plane cuttingPlane = new MeshCutter.Plane (planeTransform, origianlObject.transform);
                cut (origianlObject.gameObject, cuttingPlane, planeTransform.right);
            }

            int activadedChildrenCount = activatedChildren.Count;

            for (int i = 0; i < activadedChildrenCount; i ++)
            {
                if (activatedChildren [i].IsTouchingBlade)
                {
                    MeshCutter.Plane cuttingPlane = new MeshCutter.Plane (planeTransform, activatedChildren [i].transform);
                    cut (activatedChildren [i].gameObject, cuttingPlane, planeTransform.right);
                }
            }
        }
    }
}

