using UnityEngine;

namespace Grid
{
    public class HexTile : MonoBehaviour
    {
        public HexCoordinates coordinates;
        
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color hoverColor = Color.cyan;

        private Renderer _renderer;
        private Color _originalColor;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }
        }

        public void SetCoordinates(HexCoordinates coords)
        {
            coordinates = coords;
            // 좌표 디버깅을 위한 이름 변경 (선택 사항)
            name = $"HexTile {coords.ToString()}";
        }

        // 마우스 상호작용 (Collider 필요)
        private void OnMouseEnter()
        {
            if (_renderer != null) _renderer.material.color = hoverColor;
        }

        private void OnMouseExit()
        {
            if (_renderer != null) _renderer.material.color = _originalColor;
        }

        private void OnMouseDown()
        {
            Debug.Log($"HexTile Clicked: {coordinates.ToString()}");
        }
    }
}
