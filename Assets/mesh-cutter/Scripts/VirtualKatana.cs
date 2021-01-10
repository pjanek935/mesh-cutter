using System.Collections;
using UnityEngine;

namespace MeshCutter
{
    public class VirtualKatana : MonoBehaviour
    {
        public delegate void OnCutTriggeredDelegate (Transform plane);
        public event OnCutTriggeredDelegate OnCutTriggered;

        [SerializeField] Transform cuttingPlane;
        [SerializeField] new Camera camera;
        [SerializeField] UILineRenderer slash;

        [SerializeField] float minDist = 5f;

        bool isMouseDown;
        Vector3 startMousePos = Vector3.zero;
        bool requestCut = false;

        private void FixedUpdate ()
        {
            if (requestCut)
            {
                requestCut = false;
                StartCoroutine (proceedFireEvent ());//wait to the next frame
            }
        }

        IEnumerator proceedFireEvent ()
        {
            yield return new WaitForEndOfFrame ();
            yield return new WaitForEndOfFrame ();

            OnCutTriggered?.Invoke (cuttingPlane);
        }

        private void Update ()
        {
            if (isMouseDown)
            {
                slash.End = Input.mousePosition;
            }

            if (Input.GetMouseButtonDown (0))
            {
                isMouseDown = true;
                startMousePos = Input.mousePosition;
                slash.gameObject.SetActive (true);
                slash.SetPoints (startMousePos, Input.mousePosition);
            }

            if (Input.GetMouseButtonUp (0))
            {
                slash.gameObject.SetActive (false);
                isMouseDown = false;
                float dist = Vector3.Distance (Input.mousePosition, startMousePos);

                if (dist > minDist)
                {
                    Vector3 center = (Input.mousePosition + startMousePos) / 2f;
                    Ray ray = camera.ScreenPointToRay (center);
                    Vector3 direction = Input.mousePosition - startMousePos;
                    float angle = Mathf.Atan2 (direction.y, direction.x);

                    cuttingPlane.forward = ray.direction;
                    cuttingPlane.rotation = Quaternion.Euler (cuttingPlane.eulerAngles.x, cuttingPlane.eulerAngles.y, Mathf.Rad2Deg * angle);
                    cuttingPlane.position = (camera.ScreenToWorldPoint (Input.mousePosition) + camera.ScreenToWorldPoint (startMousePos)) / 2f;
                    cuttingPlane.localScale = new Vector3 (dist / 1000f, 0.01f, 5);

                    requestCut = true; //event should be fired after all the coliders have a chance to update IsTouchingBlade value
                    //so its pushed to the FixedUpdate method
                }
            }
        }
    }
}

