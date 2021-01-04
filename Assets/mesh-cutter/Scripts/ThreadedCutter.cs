using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MeshCutter
{
    public class ThreadedCutter : MonoBehaviour
    {
        public delegate void OnMeshCutFinishedDelegate (List <CutResult> results);
        public event OnMeshCutFinishedDelegate OnMeshCutFinished;

        public bool IsBusy
        {
            get;
            private set;
        }

        public bool Cut (List<SimpleMesh> meshesToCut)
        {
            bool result = false;

            if (!IsBusy && meshesToCut != null && meshesToCut.Count > 0)
            {
                IsBusy = true;
                StartCoroutine (proceedCut (meshesToCut));
                result = true;
            }

            return result;
        }

        IEnumerator proceedCut (List<SimpleMesh> simpleMeshes)
        {
            bool threadRunning = true;
            List<CutResult> cutResults = new List<CutResult> ();

            System.Threading.Thread thread = new System.Threading.Thread (new ThreadStart (
                   () =>
                   {
                       foreach (SimpleMesh simpleMesh in simpleMeshes)
                       {
                           cutResults.Add (Cutter.Cut (simpleMesh.Triangles, simpleMesh.Vertices, simpleMesh.Normals, simpleMesh.UVs, simpleMesh.CuttingPlane));
                       }

                       threadRunning = false;
                   }
               ));

            thread.Start ();

            yield return new WaitWhile (() => threadRunning);

            OnMeshCutFinished?.Invoke (cutResults);

            IsBusy = false;
        }
    }
}

