using UnityEngine;

namespace Managers
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 10, -8);
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private float rotationAngleX = 60f; // Top-down angle

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    Debug.LogWarning("CameraFollow: Player not found!");
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Follow Position
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // Fix Rotation (Look at target or fixed angle)
            // Option 1: Look at target
            // transform.LookAt(target);
            
            // Option 2: Fixed Angle (Better for static Top-Down)
            transform.rotation = Quaternion.Euler(rotationAngleX, 0, 0);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
