using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
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
        [SerializeField] ParticleManager particleManager;
        [SerializeField] ThreadedCutter threadedCutter;

        [SerializeField] float torqueAfterCutRange = 0.1f;
        [SerializeField] Vector2 forceAfterCut = new Vector2 (2, 7);

        List<Cuttable> activatedChildren = new List<Cuttable> ();
        Queue<Cuttable> deactivatedChildrenPool = new Queue<Cuttable> ();
        System.Random random = new System.Random ();

        const int childrenCount = 50;
        const string originalTatamiTag = "OriginalTatami";

        Vector3 defaultPosition;
        Quaternion defaultRotation;

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
            defaultPosition = originalObjectToCut.transform.position;
            defaultRotation = originalObjectToCut.transform.rotation;
            initChildren ();
            activatedChildren.Add (originalObjectToCut);
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

            originalObjectToCut.gameObject.SetActive (true);
            originalObjectToCut.GetComponent<MeshFilter> ().mesh = originalTatamiMesh;
            originalObjectToCut.GetComponent<MeshCollider> ().sharedMesh = originalTatamiMesh;
            activatedChildren.Add (originalObjectToCut);

            originalObjectToCut.transform.rotation = defaultRotation;
            originalObjectToCut.transform.position = defaultPosition;
        }

        private void Update ()
        {
            if (Input.GetKeyDown (KeyCode.Space))
            {
                Reset ();
            }
        }

        void onCutTriggered (Transform planeTransform)
        {
            if (threadedCutter.IsBusy)
                return;

            List<CutData> cutData = new List<CutData> ();
            int activadedChildrenCount = activatedChildren.Count;

            for (int i = 0; i < activadedChildrenCount; i++)
            {
                if (activatedChildren [i].IsTouchingBlade && deactivatedChildrenPool.Count > 0)
                {
                    Plane cuttingPlane = new Plane (planeTransform, activatedChildren [i].transform);
                    CutData data = new CutData ();
                    SimpleMesh simpleMesh = new SimpleMesh ();

                    simpleMesh.Create (activatedChildren [i].GetMeshFilter ().mesh, cuttingPlane);
                    data.Parent = activatedChildren [i];
                    data.Child = deactivatedChildrenPool.Dequeue ();
                    data.Plane = cuttingPlane;
                    data.SimpleMesh = simpleMesh;
                    data.CutDirection = planeTransform.right;

                    cutData.Add (data);

                    data.Parent.IsBusy = true;
                    data.Child.IsBusy = true;
                }
            }

            if (cutData.Count > 0)
            {
                StartCoroutine (cutInThreadAndWaitToEnd (cutData));
            }
        }

        IEnumerator cutInThreadAndWaitToEnd (List<CutData> cutData)
        {
            threadedCutter.Cut (cutData);

            yield return new WaitWhile (() => threadedCutter.IsBusy);

            processCutResult (cutData);
        }

        void processCutResult (List<CutData> cutData)
        {
            foreach (CutData data in cutData)
            {
                proceedCutResult (data);
            }
        }

        void proceedCutResult (CutData cutData)
        {
            if (cutData == null || cutData.Parent == null || cutData.Child == null || cutData.CutResult == null)
            {
                return;
            } 

            Cuttable parent = cutData.Parent;
            Cuttable child = cutData.Child;
            Plane cuttingPlane = cutData.Plane;
            Vector3 cutDirection = cutData.CutDirection;

            Mesh mesh = parent.GetMeshFilter ().mesh;
            CutResult cutResult = cutData.CutResult;
            Mesh aMesh = cutResult.CreateMeshA ();
            Mesh bMesh = cutResult.CreateMeshB ();
            Mesh newParentMesh = aMesh;
            Mesh newChildMesh = bMesh;

            if (newParentMesh.vertexCount > 0 && newChildMesh.vertexCount > 0)
            {
                //original object base should stay in place so I switch meshes position in case
                //bMesh is closer to an origin point
                flipMeshesPositionIfNeeded (ref newParentMesh, ref newChildMesh, parent.transform);

                parent.SetNewMesh (newParentMesh);
                child.SetNewMesh (newChildMesh);

                //set mesh and position for child
                child.transform.position = parent.transform.position;
                child.transform.rotation = parent.transform.rotation;
                child.transform.localScale = parent.transform.localScale;
                child.GetMeshFilter ().mesh = newChildMesh;
                child.gameObject.SetActive (true);
                activatedChildren.Add (child);

                //add torque and force to child object
                Rigidbody rb = child.GetComponent<Rigidbody> ();
                addForceAndTorqueToAChildObject (rb, cutDirection);

                //shoot particles
                if (cutResult.EdgeVertices.Count > 0)
                {
                    Vector3 pos = cutResult.EdgeVertices [0];
                    pos = parent.transform.TransformPoint (pos);
                    particleManager.ShootParticles (pos, cutDirection);
                }
            }
            else
            {
                child.gameObject.SetActive (false);
                deactivatedChildrenPool.Enqueue (child);
            }

            parent.IsBusy = false;
            child.IsBusy = false;
        }

        void addForceAndTorqueToAChildObject (Rigidbody rb, Vector3 cutDirection)
        {
            if (rb == null)
                return;

            Vector3 forceVector = Vector3.up * forceAfterCut.y + cutDirection.normalized * forceAfterCut.x;
            rb.AddForce (forceVector, ForceMode.Impulse);
            rb.AddTorque (new Vector3 ((float) random.NextDouble () * torqueAfterCutRange / 2f,
                (float) random.NextDouble () * torqueAfterCutRange / 2f,
                ((float) random.NextDouble () * torqueAfterCutRange / 2f) * 0.5f), ForceMode.Impulse);
        }

        /// <summary>
        /// Flips given aMesh and bMesh based on distance from an origin point.
        /// </summary>
        /// <param name="aMesh"></param>
        /// <param name="bMesh"></param>
        /// <param name="parentTransform"></param>
        void flipMeshesPositionIfNeeded (ref Mesh aMesh, ref Mesh bMesh, Transform parentTransform)
        {
            bool isOriginalTatami = string.Equals (parentTransform.tag, originalTatamiTag);

            if (isOriginalTatami)
            {
                Vector3 aPos = parentTransform.TransformPoint (aMesh.vertices [0]);
                Vector3 bPos = parentTransform.TransformPoint (bMesh.vertices [0]);
                float aD = Vector3.Distance (origin.position, aPos);
                float bD = Vector3.Distance (origin.position, bPos);

                if (bD < aD)
                {
                    Mesh tmp = aMesh;
                    aMesh = bMesh;
                    bMesh = tmp;
                }
            }
        }
    }
}



