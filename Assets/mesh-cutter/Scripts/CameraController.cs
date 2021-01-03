using UnityEngine;

namespace MeshCutter
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform objectToRotateAround;

        [SerializeField] float rotationSpeed;
        [SerializeField] float zoomSpeed;

        [SerializeField] float maxYAngle;
        [SerializeField] float maxZoom = 10;
        [SerializeField] float minZoom = 5;
        [SerializeField] float deaccelerationFactor = 10f;

        Vector3 prevMousePos;
        bool isMouseDown = false;
        Vector3 deltaRotation;
        bool rotateOrZoomThisFrame = false;
        const float minScrollMagnitude = 0.01f;

        private void Update ()
        {
            if (objectToRotateAround == null)
                return;

            rotateOrZoomThisFrame = false;

            if (isMouseDown)
            {
                deltaRotation = (Input.mousePosition - prevMousePos) * Time.deltaTime * rotationSpeed;
                deltaRotation.x *= -1f;
                prevMousePos = Input.mousePosition;
                rotateOrZoomThisFrame = true;
            }

            if (Input.GetMouseButtonDown (1))
            {
                prevMousePos = Input.mousePosition;
                isMouseDown = true;
            }

            if (Input.GetMouseButtonUp (1))
            {
                isMouseDown = false;
            }

            if (Input.mouseScrollDelta.sqrMagnitude > minScrollMagnitude)
            {
                rotateOrZoomThisFrame = true;
            }

            if (rotateOrZoomThisFrame || deltaRotation.sqrMagnitude < minScrollMagnitude)
            {
                rotateAndZoom (deltaRotation, -Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed);
            }

            deltaRotation = Vector3.Lerp (deltaRotation, Vector3.zero, Time.deltaTime * deaccelerationFactor);
        }

        void rotateAndZoom (Vector2 deltaRotation, float zoomDelta)
        {
            Vector3 direction = this.transform.position - objectToRotateAround.position;
            Spherical spherical = new Spherical (direction);

            spherical.Theta += deltaRotation.x;
            spherical.Phi += deltaRotation.y;
            spherical.R += zoomDelta;

            float maxYAngleRad = maxYAngle * Mathf.Deg2Rad;
            spherical.Phi = Mathf.Clamp (spherical.Phi, maxYAngleRad, 3.14f - maxYAngleRad);
            spherical.R = Mathf.Clamp (spherical.R, minZoom, maxZoom);

            this.transform.position = spherical.ToVector3 () + objectToRotateAround.position;
            this.transform.LookAt (objectToRotateAround.transform);
        }
    }
}

