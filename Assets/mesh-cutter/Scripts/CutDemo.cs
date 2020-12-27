using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCutter
{
    public class CutDemo : MonoBehaviour
    {
        [SerializeField] GameObject childPrefab;
        [SerializeField] VirtualKatana virtualKatana;
        [SerializeField] Cuttable originalObjectToCut;
        [SerializeField] Transform origin;
        [SerializeField] Mesh originalTatamiMesh;

        [SerializeField] float torqueAfterCutRange = 0.1f;
        [SerializeField] Vector2 forceAfterCut = new Vector2 (2, 7);

        List<Cuttable> activatedChildren = new List<Cuttable> ();
        Queue<Cuttable> deactivatedChildrenPool = new Queue<Cuttable> ();
        System.Random random = new System.Random ();

        const int childrenCount = 25;
        const string originalTatamiTag = "OriginalTatami";

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

        public void Reset ()
        {
            for (int i = 0; i < activatedChildren.Count; i ++)
            {
                activatedChildren [i].gameObject.SetActive (false);
                deactivatedChildrenPool.Enqueue (activatedChildren [i]);
            }

            activatedChildren.Clear ();
            originalObjectToCut.GetComponent<MeshFilter> ().mesh = originalTatamiMesh;
            originalObjectToCut.GetComponent<MeshCollider> ().sharedMesh = originalTatamiMesh;
        }

        private void Update ()
        {
            if (Input.GetKeyDown (KeyCode.Space))
            {
                Reset ();
            }
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

            bool isOriginalTatami = string.Equals (parent.tag, originalTatamiTag);

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
                activatedChildren.Add (gameObject.GetComponent<Cuttable> ());

                gameObject.transform.position = parent.transform.position;
                gameObject.transform.rotation = parent.transform.rotation;
                gameObject.transform.localScale = parent.transform.localScale;
                parentMeshFilter = gameObject.GetComponent<MeshFilter> ();
                parentMeshFilter.mesh = childMesh;
                meshCollider = gameObject.GetComponent<MeshCollider> ();
                meshCollider.convex = true;
                meshCollider.sharedMesh = childMesh;
                gameObject.SetActive (true);
                Rigidbody rb = gameObject.GetComponent<Rigidbody> ();

                Vector3 forceVector = Vector3.up * forceAfterCut.y + cutDirection.normalized * forceAfterCut.x;
                rb.AddForce (forceVector, ForceMode.Impulse);
                rb.AddTorque (new Vector3 ((float) random.NextDouble () * torqueAfterCutRange / 2f,
                    (float) random.NextDouble () * torqueAfterCutRange / 2f,
                    ((float) random.NextDouble () * torqueAfterCutRange / 2f) * 0.5f), ForceMode.Impulse);
            }
        }

        void onCutTriggered (Transform planeTransform)
        {
            if (originalObjectToCut.IsTouchingBlade)
            {
                MeshCutter.Plane cuttingPlane = new MeshCutter.Plane (planeTransform, originalObjectToCut.transform);
                cut (originalObjectToCut.gameObject, cuttingPlane, planeTransform.right);
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

