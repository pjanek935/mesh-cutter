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

        public bool Cut (List<SimpleMesh> meshesToCut, Plane cuttingPlane)
        {
            bool result = false;

            if (meshesToCut != null && meshesToCut.Count > 0)
            {
                StartCoroutine (proceedCut (meshesToCut, cuttingPlane));
                result = true;
            }

            return result;
        }

        IEnumerator proceedCut (List<SimpleMesh> simpleMeshes, Plane cuttingPlane)
        {
            bool threadRunning = true;
            List<CutResult> cutResults = new List<CutResult> ();

            System.Threading.Thread thread = new System.Threading.Thread (new ThreadStart (
                   () =>
                   {
                       foreach (SimpleMesh simpleMesh in simpleMeshes)
                       {
                           cutResults.Add (Cutter.Cut (simpleMesh.Triangles, simpleMesh.Vertices, simpleMesh.Normals, simpleMesh.UVs, cuttingPlane));
                       }

                       threadRunning = false;
                   }
               ));

            thread.Start ();

            yield return new WaitWhile (() => threadRunning);

            OnMeshCutFinished?.Invoke (cutResults);
        }
    }
}

