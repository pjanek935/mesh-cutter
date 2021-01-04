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
        List<CutData> cutData = new List<CutData> ();

        const int childrenCount = 100;
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
            activatedChildren.Add (originalObjectToCut);
        }

        private void OnEnable ()
        {
            virtualKatana.OnCutTriggered += onCutTriggered;
            threadedCutter.OnMeshCutFinished += onMeshCutFinished;
        }

        private void OnDisable ()
        {
            virtualKatana.OnCutTriggered -= onCutTriggered;
            threadedCutter.OnMeshCutFinished -= onMeshCutFinished;
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
        }

        private void Update ()
        {
            if (Input.GetKeyDown (KeyCode.Space))
            {
                Reset ();
            }
        }

        bool cut (Cuttable parent, Cuttable newChild, Plane cuttingPlane, Vector3 cutDirection)
        {
            bool result = false;

            if (parent == null || newChild == null)
                return false;

            Mesh mesh = parent.GetMeshFilter ().mesh;
            CutResult cutResult = Cutter.Cut (mesh.triangles, mesh.vertices, mesh.normals, mesh.uv, cuttingPlane);
            Mesh aMesh = cutResult.CreateMeshA ();
            Mesh bMesh = cutResult.CreateMeshB ();
            Mesh newOriginalMesh = aMesh;
            Mesh childMesh = bMesh;

            if (newOriginalMesh.vertexCount > 0 && childMesh.vertexCount > 0)
            {
                bool isOriginalTatami = string.Equals (parent.tag, originalTatamiTag);

                if (isOriginalTatami)
                {
                    Vector3 aPos = parent.transform.TransformPoint (aMesh.vertices [0]);
                    Vector3 bPos = parent.transform.TransformPoint (bMesh.vertices [0]);
                    float aD = Vector3.Distance (origin.position, aPos);
                    float bD = Vector3.Distance (origin.position, bPos);

                    if (bD < aD)
                    {
                        newOriginalMesh = bMesh;
                        childMesh = aMesh;
                    }
                }

                parent.GetMeshFilter ().mesh = newOriginalMesh;
                MeshCollider meshCollider = parent.GetComponent<MeshCollider> ();
                meshCollider.sharedMesh = newOriginalMesh;

                newChild.transform.position = parent.transform.position;
                newChild.transform.rotation = parent.transform.rotation;
                newChild.transform.localScale = parent.transform.localScale;
                newChild.GetMeshFilter ().mesh = childMesh;
                meshCollider = newChild.GetComponent<MeshCollider> ();
                meshCollider.convex = true;
                meshCollider.sharedMesh = childMesh;
                newChild.gameObject.SetActive (true);
                Rigidbody rb = newChild.GetComponent<Rigidbody> ();

                if (rb != null)
                {
                    Vector3 forceVector = Vector3.up * forceAfterCut.y + cutDirection.normalized * forceAfterCut.x;
                    rb.AddForce (forceVector, ForceMode.Impulse);
                    rb.AddTorque (new Vector3 ((float) random.NextDouble () * torqueAfterCutRange / 2f,
                        (float) random.NextDouble () * torqueAfterCutRange / 2f,
                        ((float) random.NextDouble () * torqueAfterCutRange / 2f) * 0.5f), ForceMode.Impulse);
                }
                
                if (cutResult.EdgeVertices.Count > 0)
                {
                    Vector3 pos = cutResult.EdgeVertices [0];
                    pos = parent.transform.TransformPoint (pos);
                    particleManager.ShootParticles (pos, cutDirection);
                }

                result = true;
            }

            return result;
        }

        void onCutTriggered (Transform planeTransform)
        {
            int activadedChildrenCount = activatedChildren.Count;

            for (int i = 0; i < activadedChildrenCount; i ++)
            {
                if (activatedChildren [i].IsTouchingBlade)
                {
                    Plane cuttingPlane = new Plane (planeTransform, activatedChildren [i].transform);
                   
                    if (deactivatedChildrenPool.Count > 0)
                    {
                        Cuttable newChild = deactivatedChildrenPool.Dequeue ();

                        if (cut (activatedChildren [i], newChild, cuttingPlane, planeTransform.right))
                        {
                            activatedChildren.Add (newChild);
                        }
                        else
                        {
                            deactivatedChildrenPool.Enqueue (newChild);
                        }
                    }
                }
            }
        }

        void onCutTriggered2 (Transform planeTransform)
        {
            if (threadedCutter.IsBusy)
                return;

            List<CutData> data = new List<CutData> ();
            int activadedChildrenCount = activatedChildren.Count;

            for (int i = 0; i < activadedChildrenCount; i++)
            {
                if (activatedChildren [i].IsTouchingBlade)
                {
                    Plane cuttingPlane = new Plane (planeTransform, activatedChildren [i].transform);

                    if (deactivatedChildrenPool.Count > 0)
                    {
                        
                        //List<SimpleMesh> meshData = new List<SimpleMesh> ();
                        SimpleMesh simpleMesh = new SimpleMesh ();
                        simpleMesh.Create (activatedChildren [i].GetMeshFilter ().mesh, cuttingPlane);
                        //meshData.Add (simpleMesh);
                        //threadedCutter.Cut (meshData, cuttingPlane);

                        CutData cutData = new CutData ();
                        cutData.Parent = activatedChildren [i];
                        cutData.Child = deactivatedChildrenPool.Dequeue ();
                        cutData.Plane = cuttingPlane;
                        cutData.SimpleMesh = simpleMesh;
                        data.Add (cutData);
                    }
                }
            }

            if (data.Count > 0)
            {
                cutData = data;
                List<SimpleMesh> meshData = new List<SimpleMesh> ();

                for (int i = 0; i < data.Count; i++)
                {
                    meshData.Add (data [i].SimpleMesh);
                }

                threadedCutter.Cut (meshData);
            }
        }


        void onMeshCutFinished (List<CutResult> results)
        {
            
        }
    }

    public class CutData
    {
        public Cuttable Parent;
        public Cuttable Child;
        public Plane Plane;
        public SimpleMesh SimpleMesh;

    }
}



