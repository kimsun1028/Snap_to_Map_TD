using UnityEngine;
using UnityEngine.UI;

namespace SnapToMapTD.UI
{
    public class BoundaryLines : MonoBehaviour
    {
        [SerializeField] private Color lineColor = Color.yellow;
        [SerializeField] private float lineWidth = 2f;
        [SerializeField] private MapManager mapManager;

        private void Start()
        {
            if (mapManager == null)
                mapManager = FindObjectOfType<MapManager>();

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null || mapManager == null) return;

            // 카메라 기준으로 맵 경계를 스크린 좌표로 변환
            Camera cam = Camera.main;
            float halfWorldWidth = mapManager.mapPixelSize.x / mapManager.pixelsPerUnit / 2f;

            Vector3 leftWorld = new Vector3(-halfWorldWidth, 0f, 0f);
            Vector3 rightWorld = new Vector3(halfWorldWidth, 0f, 0f);

            float leftScreenX = cam.WorldToScreenPoint(leftWorld).x;
            float rightScreenX = cam.WorldToScreenPoint(rightWorld).x;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            float scaleX = canvasRect.rect.width / Screen.width;

            float leftCanvasX = leftScreenX * scaleX - canvasRect.rect.width / 2f;
            float rightCanvasX = rightScreenX * scaleX - canvasRect.rect.width / 2f;

            CreateLine("LeftBoundary", leftCanvasX);
            CreateLine("RightBoundary", rightCanvasX);
        }

        private void CreateLine(string lineName, float canvasX)
        {
            GameObject go = new GameObject(lineName);
            go.transform.SetParent(transform, false);

            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(lineWidth, 0f);
            rect.anchoredPosition = new Vector2(canvasX, 0f);

            Image img = go.AddComponent<Image>();
            img.color = lineColor;
        }
    }
}
