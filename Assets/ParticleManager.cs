using System.Collections.Generic;
using UnityEngine;

namespace MeshCutter
{
    public class ParticleManager : MonoBehaviour
    {
        [SerializeField] GameObject particleEmiterPrefab;

        const int maxParticleEmiters = 4;

        Queue<ParticleSystem> particleEmiters = new Queue<ParticleSystem> ();

        private void Awake ()
        {
            initParticleEmiters ();
        }

        void initParticleEmiters ()
        {
            for (int i = 0; i < maxParticleEmiters; i++)
            {
                GameObject newGameObject = Instantiate (particleEmiterPrefab);
                newGameObject.SetActive (false);
                newGameObject.transform.SetParent (this.transform);
                particleEmiters.Enqueue (newGameObject.GetComponent<ParticleSystem> ());
            }
        }

        public void ShootParticles (Vector3 pos, Vector3 forward)
        {
            ParticleSystem particleSystem = particleEmiters.Dequeue ();
            particleSystem.gameObject.SetActive (true);
            particleSystem.transform.position = pos;
            particleSystem.transform.forward = forward;
            particleSystem.Play ();

            particleEmiters.Enqueue (particleSystem);
        }

    }
}

