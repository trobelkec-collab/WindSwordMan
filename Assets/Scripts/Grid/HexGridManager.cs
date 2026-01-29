using UnityEngine;
using System.Collections.Generic;

namespace Grid
{
    public class HexGridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private HexTile tilePrefab;
        [SerializeField] private float outerRadius = 1f;

        private Dictionary<string, HexTile> _tiles;

        private void Awake()
        {
            _tiles = new Dictionary<string, HexTile>();
        }

        private void Start()
        {
            GenerateGrid();
        }

        [ContextMenu("Generate Grid")] // 에디터에서도 테스트 가능하도록
        public void GenerateGrid()
        {
            ClearGrid();

            // Pointy topped hexagons
            float innerRadius = outerRadius * 0.866025404f;

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    CreateCell(x, z, innerRadius);
                }
            }
        }

        private void CreateCell(int x, int z, float innerRadius)
        {
            if (tilePrefab == null)
            {
                Debug.LogError("HexGridManager: Tile Prefab is missing!");
                return;
            }

            // Calculate Position
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (innerRadius * 2f);
            position.y = 0f;
            position.z = z * (outerRadius * 1.5f);

            // Instantiate
            HexTile tile = Instantiate(tilePrefab);
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = position;

            // Set Coordinates
            HexCoordinates coords = HexCoordinates.FromOffsetCoordinates(x, z);
            tile.SetCoordinates(coords);

            // Register
            if (_tiles == null) _tiles = new Dictionary<string, HexTile>();
            _tiles[coords.ToString()] = tile;
        }

        public void ClearGrid()
        {
            if (_tiles != null) _tiles.Clear();
            
            // 자식 오브젝트 모두 삭제
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}
