using UnityEngine;

namespace MeshCutter
{
    [RequireComponent (typeof (MeshFilter))]
    public class Cuttable : MonoBehaviour
    {
        MeshFilter meshFilter = null;
        const string BladeTag = "Blade";

        public bool IsBusy
        {
            get;
            set;
        }

        private void Awake ()
        {
            getMeshFilterIfNeeded ();
        }

        public MeshFilter GetMeshFilter ()
        {
            getMeshFilterIfNeeded ();

            return meshFilter;
        }

        public bool IsTouchingBlade;

        private void OnTriggerEnter (Collider other)
        {
            if (string.Equals (other.transform.tag, BladeTag))
            {
                IsTouchingBlade = true;
            }
        }

        private void OnTriggerExit (Collider other)
        {
            if (string.Equals (other.transform.tag, BladeTag))
            {
                IsTouchingBlade = false;
            }
        }

        public void SetNewMesh (Mesh mesh)
        {
            getMeshFilterIfNeeded ();
            meshFilter.mesh = mesh;

            MeshCollider meshCollider = GetComponent<MeshCollider> ();

            if (meshCollider != null)
            {
                meshCollider.sharedMesh = mesh;
            }
        }

        void getMeshFilterIfNeeded ()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter> ();
            }
        }
    }
}

