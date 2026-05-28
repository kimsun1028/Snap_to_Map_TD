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

    [Header("Debug View")]
    public List<Vector3> worldWaypoints = new List<Vector3>();

    private LineRenderer lineRenderer;
    private SpriteRenderer backgroundRenderer;
    private SnapToMapTD.PathMarkers pathMarkers;

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
            var rawData = loader.LoadRaw();
            if (!string.IsNullOrEmpty(rawData.image_file))
                backgroundFileName = rawData.image_file;
            worldWaypoints.AddRange(loader.ConvertToWorld(rawData));
        }
        catch (Exception exception)
        {
            Debug.LogError($"[MapManager] JSON 파싱 또는 경로 변환 실패: {exception.Message}");
            return;
        }

        UpdateLineRenderer();

        if (pathMarkers == null)
            pathMarkers = GetComponentInChildren<SnapToMapTD.PathMarkers>();
        pathMarkers?.UpdateMarkers();
    }

    private void UpdateLineRenderer()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            return;

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = worldWaypoints.Count;
        lineRenderer.SetPositions(worldWaypoints.ToArray());
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
        // 중복 MapBackground 제거 후 하나만 유지
        SpriteRenderer found = null;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name != "MapBackground") continue;

            if (found == null)
            {
                found = child.GetComponent<SpriteRenderer>();
                if (found == null)
                    found = child.gameObject.AddComponent<SpriteRenderer>();
            }
            else
            {
                if (Application.isPlaying) Destroy(child.gameObject);
                else DestroyImmediate(child.gameObject);
            }
        }

        if (found != null) return found;

        GameObject go = new GameObject("MapBackground");
        go.transform.SetParent(transform, false);
        return go.AddComponent<SpriteRenderer>();
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