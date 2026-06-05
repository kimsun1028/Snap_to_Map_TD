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

        private GameObject selectedPrefab;
        private int selectedCost;
        private GameObject ghostInstance;
        private Camera mainCamera;

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

            selectedPrefab = data.prefab;
            selectedCost = data.cost;
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
            if (IsOnRoad(worldPos))
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
            Cancel();
        }

        [SerializeField] private float placementCheckRadius = 0.3f;

        private bool IsOnRoad(Vector3 worldPos)
        {
            foreach (var col in Physics2D.OverlapCircleAll(worldPos, placementCheckRadius))
                if (col.GetComponent<MapRoadGenerator>() != null)
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
