using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace Controllers
{
    public class CameraZoom : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minHeight = 2f;
        [SerializeField] private float maxHeight = 20f;
        
        [Header("Smooth Settings")]
        [SerializeField] private float smoothTime = 0.2f;

        private CinemachineVirtualCamera _vcam;
        private CinemachineTransposer _transposer;
        private float _targetHeight;
        private float _currentVelocity;

        private void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            if (_vcam != null)
            {
                _transposer = _vcam.GetCinemachineComponent<CinemachineTransposer>();
                if (_transposer != null)
                {
                    _targetHeight = _transposer.m_FollowOffset.y;
                }
            }
        }

        private void Update()
        {
            if (_transposer == null) return;

            HandleZoomInput();
            ApplyZoom();
        }

        private void HandleZoomInput()
        {
            // Check if Mouse is valid
            if (Mouse.current == null) return;

            float scroll = Mouse.current.scroll.y.ReadValue();
            
            // Scroll usually returns +/- 120 or similar, normalize slightly or just use speed
            if (scroll != 0)
            {
                // Scroll Up (+): Zoom In (Decrease Height)
                // Scroll Down (-): Zoom Out (Increase Height)
                _targetHeight -= scroll * zoomSpeed * 0.01f; 
                _targetHeight = Mathf.Clamp(_targetHeight, minHeight, maxHeight);
            }
        }

        private void ApplyZoom()
        {
            float newHeight = Mathf.SmoothDamp(_transposer.m_FollowOffset.y, _targetHeight, ref _currentVelocity, smoothTime);
            
            // Maintain 45-degree angle (or whatever ratio was set initially)
            // Assuming initial was (0, Y, -Y) or similar. 
            // We just update Y and Z logic: Z should typically be -Y for 45 deg top-down looking forward.
            
            _transposer.m_FollowOffset = new Vector3(
                _transposer.m_FollowOffset.x,
                newHeight,
                -newHeight
            );
        }
    }
}
