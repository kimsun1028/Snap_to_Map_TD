using UnityEngine;
using SnapToMapTD.Game;

namespace SnapToMapTD.Towers
{
    public class TowerPlacer : MonoBehaviour
    {
        private GameObject selectedPrefab;
        private int selectedCost;
        private GameObject ghostInstance;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void SelectTower(TowerData data)
        {
            Cancel();

            selectedPrefab = data.prefab;
            selectedCost = data.cost;

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
            if (selectedPrefab == null) return;

            Vector3 worldPos = GetMouseWorldPos();

            if (ghostInstance != null)
                ghostInstance.transform.position = worldPos;

            if (Input.GetMouseButtonDown(0))
                TryPlace(worldPos);

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                Cancel();
        }

        private void TryPlace(Vector3 worldPos)
        {
            if (GameManager.Instance == null || !GameManager.Instance.TrySpendGold(selectedCost))
            {
                Debug.Log("[TowerPlacer] 골드 부족");
                return;
            }

            Instantiate(selectedPrefab, worldPos, Quaternion.identity);
            Cancel();
        }

        private void Cancel()
        {
            if (ghostInstance != null)
            {
                Destroy(ghostInstance);
                ghostInstance = null;
            }
            selectedPrefab = null;
            selectedCost = 0;
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mouse = Input.mousePosition;
            mouse.z = -mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(mouse);
        }
    }
}
