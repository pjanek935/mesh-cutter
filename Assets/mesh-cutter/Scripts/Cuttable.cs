﻿using UnityEngine;

namespace MeshCutter
{
    [RequireComponent (typeof (MeshFilter))]
    public class Cuttable : MonoBehaviour
    {
        MeshFilter meshFilter = null;

        private void Awake ()
        {
            getMeshFilterIfNeeded ();
        }

        public MeshFilter GetMeshFilter ()
        {
            getMeshFilterIfNeeded ();

            return meshFilter;
        }

        public bool IsTouchingBlade
        {
            get;
            set;
        }

        public bool IsBusy
        {
            get;
            set;
        }

        private void OnTriggerEnter (Collider other)
        {
            if (string.Equals (other.transform.tag, "Blade"))
            {
                IsTouchingBlade = true;
            }
        }

        private void OnTriggerExit (Collider other)
        {
            if (string.Equals (other.transform.tag, "Blade"))
            {
                IsTouchingBlade = false;
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

