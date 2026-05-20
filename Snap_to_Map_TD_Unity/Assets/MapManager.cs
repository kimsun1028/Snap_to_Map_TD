using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SnapToMapTD.Pathing;

// [ExecuteAlways]를 붙여서 게임을 틀지 않아도 에디터가 리스트를 실시간으로 인지하게 만듭니다.
[ExecuteAlways]
public class MapManager : MonoBehaviour
{
    [Header("Settings")]
    public string fileName = "path_data.json";
    public string backgroundFileName = "img.jpg";
    public Vector2 mapPixelSize = new Vector2(1000f, 400f);
    public float pixelsPerUnit = 100f;
    public Vector3 worldOrigin = Vector3.zero;
    public PathPlane plane = PathPlane.XY; // XY 고정
    public bool invertY = true;
    public bool includeStartPoint = true;
    public bool includeEndPoint = true;
    public bool preferJsonImageSize = true;
    public int backgroundSortingOrder = -10;
    public int lineSortingOrder = 10;
    
    [Header("Debug View")]
    public List<Vector3> worldWaypoints = new List<Vector3>();

    private LineRenderer lineRenderer;
    private SpriteRenderer backgroundRenderer;

    private void Awake()
    {
        LoadAndGeneratePath();
        LoadAndApplyBackground();
    }

    // 에디터에서 인스펙터 값을 바꿀 때마다 자동으로 다시 계산되도록 유니티 콜백 추가
    private void OnValidate()
    {
        // 씬이 켜져있을 때만 작동하도록 안전장치
        if (!Application.isPlaying)
        {
            LoadAndGeneratePath();
            LoadAndApplyBackground();
        }
    }

    public void LoadAndGeneratePath()
    {
        var loader = new MapPathLoader
        {
            fileName = fileName,
            mapPixelSize = mapPixelSize,
            pixelsPerUnit = pixelsPerUnit,
            worldOrigin = worldOrigin,
            plane = plane,
            invertY = invertY,
            includeStartPoint = includeStartPoint,
            includeEndPoint = includeEndPoint,
            preferJsonImageSize = preferJsonImageSize
        };

        worldWaypoints.Clear();

        try
        {
            worldWaypoints.AddRange(loader.LoadWorldPath());
        }
        catch (Exception exception)
        {
            Debug.LogError($"[MapManager] JSON 파싱 또는 경로 변환 실패: {exception.Message}");
            return;
        }

        // 데이터가 잘 들어왔다면 LineRenderer로 시각화 강제 연동
        UpdateLineRenderer();
    }

    private void LoadAndApplyBackground()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, backgroundFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[MapManager] 배경 이미지를 찾을 수 없습니다: {filePath}");
            return;
        }

        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(imageBytes))
        {
            Debug.LogError($"[MapManager] 배경 이미지 로드 실패: {filePath}");
            return;
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        backgroundRenderer = EnsureBackgroundRenderer();
        if (backgroundRenderer == null)
        {
            Debug.LogError("[MapManager] 배경 SpriteRenderer를 만들 수 없습니다.");
            return;
        }

        if (backgroundRenderer.sprite != null && backgroundRenderer.sprite != sprite)
        {
            if (Application.isPlaying)
            {
                Destroy(backgroundRenderer.sprite.texture);
                Destroy(backgroundRenderer.sprite);
            }
            else
            {
                DestroyImmediate(backgroundRenderer.sprite.texture);
                DestroyImmediate(backgroundRenderer.sprite);
            }
        }

        backgroundRenderer.sprite = sprite;
        backgroundRenderer.sortingOrder = backgroundSortingOrder;
        backgroundRenderer.color = Color.white;

        Transform backgroundTransform = backgroundRenderer.transform;
        backgroundTransform.position = worldOrigin;
        backgroundTransform.localScale = Vector3.one;
    }

    private SpriteRenderer EnsureBackgroundRenderer()
    {
        Transform backgroundTransform = transform.Find("MapBackground");
        if (backgroundTransform == null)
        {
            GameObject backgroundObject = new GameObject("MapBackground");
            backgroundObject.transform.SetParent(transform, false);
            backgroundTransform = backgroundObject.transform;
        }

        SpriteRenderer renderer = backgroundTransform.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = backgroundTransform.gameObject.AddComponent<SpriteRenderer>();
        }

        return renderer;
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (lineRenderer != null && worldWaypoints != null && worldWaypoints.Count > 0)
        {
            lineRenderer.sortingOrder = lineSortingOrder;
            lineRenderer.positionCount = worldWaypoints.Count;
            lineRenderer.SetPositions(worldWaypoints.ToArray());
        }
    }

    // 데이터가 너무 많으므로 Gizmos는 선(Line)만 얇게 그리도록 경량화
    private void OnDrawGizmos()
    {
        if (worldWaypoints == null || worldWaypoints.Count < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < worldWaypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(worldWaypoints[i], worldWaypoints[i + 1]);
        }
    }
}