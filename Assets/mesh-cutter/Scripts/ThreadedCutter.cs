using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MeshCutter
{
    public class ThreadedCutter : MonoBehaviour
    {
        public bool IsBusy
        {
            get;
            private set;
        }

        public bool Cut (List<CutData> meshesToCut)
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

        IEnumerator proceedCut (List<CutData> objectsToCut)
        {
            bool threadRunning = true;
            List<CutResult> cutResults = new List<CutResult> ();

            System.Threading.Thread thread = new System.Threading.Thread (new ThreadStart (
                   () =>
                   {
                       foreach (CutData cutData in objectsToCut)
                       {
                           SimpleMesh simpleMesh = cutData.SimpleMesh;
                           CutResult cutResult = Cutter.Cut (simpleMesh.Triangles, simpleMesh.Vertices, simpleMesh.Normals, simpleMesh.UVs, simpleMesh.CuttingPlane);
                           cutData.CutResult = cutResult;
                           cutResults.Add (cutResult);
                       }

                       threadRunning = false;
                   }
               ));

            thread.Start ();

            yield return new WaitWhile (() => threadRunning);

            IsBusy = false;
        }
    }
}

