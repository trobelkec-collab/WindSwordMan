using UnityEngine;

namespace Combat
{
    public class BloodAssistant : MonoBehaviour
    {
        public static BloodAssistant Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private GameObject bloodParticlePrefab;
        [SerializeField] private GameObject groundDecalPrefab;
        [SerializeField] private float decalChance = 0.5f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public void SpawnBlood(Vector3 position, Vector3 direction)
        {
            // 1. Spawn Particle (The Spray)
            if (bloodParticlePrefab != null)
            {
                // Random rotation for variety
                Quaternion rot = Quaternion.LookRotation(direction);
                GameObject blood = Instantiate(bloodParticlePrefab, position, rot);
                Destroy(blood, 2f); // Cleanup
            }

            // 2. Spawn Decal (The Puddle)
            if (Random.value < decalChance && groundDecalPrefab != null)
            {
                // Raycast down to find ground
                if (Physics.Raycast(position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
                {
                    GameObject decal = Instantiate(groundDecalPrefab, hit.point + Vector3.up * 0.01f, Quaternion.identity);
                    decal.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    // Random rotation around up axis
                    decal.transform.Rotate(Vector3.up, Random.Range(0, 360));
                    
                    // Simple logic to prevent Z-fighting or accumulation could go here
                }
            }
        }
    }
}
