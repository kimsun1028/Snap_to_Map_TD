using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SnapToMapTD.Game;
using SnapToMapTD.UI;
using SnapToMapTD;

namespace SnapToMapTD.Towers
{
    public class TowerPlacer : MonoBehaviour
    {
        [SerializeField] private GameObject cancelButton;

        private const int CostIncreasePerTower = 25;
        private readonly Dictionary<TowerData, int> placedCounts = new();

        private TowerData selectedData;
        private GameObject selectedPrefab;
        private int selectedCost;
        private GameObject ghostInstance;
        private Camera mainCamera;

        public event System.Action onTowerPlaced;

        public int GetCurrentCost(TowerData data)
        {
            int count = placedCounts.TryGetValue(data, out int n) ? n : 0;
            return data.cost + count * CostIncreasePerTower;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
            if (cancelButton != null)
            {
                cancelButton.GetComponent<Button>()?.onClick.AddListener(Cancel);
                cancelButton.SetActive(false);
            }
        }

        public void SelectTower(TowerData data)
        {
            TowerInfoPanel.Instance?.Hide();
            Cancel();

            selectedData = data;
            selectedPrefab = data.prefab;
            selectedCost = GetCurrentCost(data);
            cancelButton?.SetActive(true);

            ghostInstance = Instantiate(selectedPrefab);
            ghostInstance.name = "Ghost";

            var sr = ghostInstance.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = new Color(1f, 1f, 1f, 0.5f);

            var tower = ghostInstance.GetComponent<Tower>();
            if (tower != null) tower.enabled = false;

            var col = ghostInstance.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }

        private void Update()
        {
            if (selectedPrefab == null)
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    var hit = Physics2D.OverlapPoint(GetMouseWorldPos());
                    if (hit != null && hit.GetComponent<Tower>() != null)
                        TowerInfoPanel.Instance?.Show(hit.GetComponent<Tower>());
                    else
                        TowerInfoPanel.Instance?.Hide();
                }
                return;
            }

            Vector3 worldPos = GetMouseWorldPos();

            if (ghostInstance != null)
                ghostInstance.transform.position = worldPos;

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                TryPlace(worldPos);

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                Cancel();
        }

        private void TryPlace(Vector3 worldPos)
        {
            if (IsOnRoad(worldPos) || IsOnTower(worldPos))
            {
                Cancel();
                return;
            }

            if (GameManager.Instance == null || !GameManager.Instance.TrySpendGold(selectedCost))
            {
                Debug.Log("[TowerPlacer] 골드 부족");
                return;
            }

            Instantiate(selectedPrefab, worldPos, Quaternion.identity);

            if (selectedData != null)
                placedCounts[selectedData] = placedCounts.TryGetValue(selectedData, out int n) ? n + 1 : 1;

            onTowerPlaced?.Invoke();
            Cancel();
        }

        [SerializeField] private float roadCheckRadius = 0.05f;
        [SerializeField] private float towerCheckRadius = 0.1f;

        private bool IsOnRoad(Vector3 worldPos)
        {
            foreach (var col in Physics2D.OverlapCircleAll(worldPos, roadCheckRadius))
                if (col.GetComponent<MapRoadGenerator>() != null)
                    return true;
            return false;
        }

        private bool IsOnTower(Vector3 worldPos)
        {
            foreach (var col in Physics2D.OverlapCircleAll(worldPos, towerCheckRadius))
                if (col.GetComponent<Tower>() != null)
                    return true;
            return false;
        }

        public void Cancel()
        {
            if (ghostInstance != null)
            {
                Destroy(ghostInstance);
                ghostInstance = null;
            }
            selectedData = null;
            selectedPrefab = null;
            selectedCost = 0;
            cancelButton?.SetActive(false);
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mouse = Input.mousePosition;
            mouse.z = -mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(mouse);
        }
    }
}
