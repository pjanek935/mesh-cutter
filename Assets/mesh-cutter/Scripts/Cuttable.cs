using UnityEngine;

namespace MeshCutter
{
    public class Cuttable : MonoBehaviour
    {
        public bool IsTouchingBlade
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
    }
}

