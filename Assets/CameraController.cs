using UnityEngine;

public class CameraController : MonoBehaviour
{
    struct Spherical
    {
        public float R;
        public float Theta;
        public float Phi;

        public Spherical (Vector3 vector3)
        {
            R = Mathf.Sqrt (Mathf.Pow (vector3.x, 2) +
                       Mathf.Pow (vector3.z, 2) +
                       Mathf.Pow (vector3.y, 2));
            Theta = Mathf.Atan2 (vector3.z, vector3.x);
            Phi = Mathf.Acos (vector3.y / R);
        }

        public Vector3 ToVector3 ()
        {
            Vector3 result = Vector3.zero;
            result.x = R * Mathf.Sin (Phi) * Mathf.Cos (Theta);
            result.z = R * Mathf.Sin (Phi) * Mathf.Sin (Theta);
            result.y = R * Mathf.Cos (Phi);

            return result;
        }
    }

    [SerializeField] Transform objectToRotateAround;

    [SerializeField] float rotationSpeed;
    [SerializeField] float zoomSpeed;

    [SerializeField] float maxYAngle;
    [SerializeField] float maxZoom = 10;
    [SerializeField] float minZoom = 5;

    Vector3 prevMousePos;
    bool isMouseDown = false;
    Vector3 deltaRotation;
    bool rotateOrZoomThisFrame = false;

    private void Update ()
    {
        rotateOrZoomThisFrame = false;

        if (isMouseDown)
        {
            deltaRotation = (Input.mousePosition - prevMousePos) * Time.deltaTime * rotationSpeed;
            deltaRotation.x *= -1f;
            prevMousePos = Input.mousePosition;
            rotateOrZoomThisFrame = true;
        }
        else
        {
            deltaRotation = Vector3.zero;
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

        if (Input.mouseScrollDelta.sqrMagnitude > 0.1 )
        {
            rotateOrZoomThisFrame = true;
        }

        if (rotateOrZoomThisFrame)
        {
            rotateAndZoom (deltaRotation, -Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed);
        }
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
