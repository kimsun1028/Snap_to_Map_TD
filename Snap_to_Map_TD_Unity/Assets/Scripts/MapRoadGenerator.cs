using System.Collections.Generic;
using UnityEngine;

namespace SnapToMapTD
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MapRoadGenerator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapManager mapManager;

        [Header("Road Settings")]
        [SerializeField] private float roadWidth = 0.4f;
        [SerializeField] [Range(0.001f, 0.2f)] private float simplifyTolerance = 0.01f;

        [Header("Visibility")]
        [SerializeField] private bool showMesh = true;
        [SerializeField] private Color roadColor = new Color(0.9f, 0.75f, 0.3f, 0.55f);
        [SerializeField] private int sortingOrder = 5;

        [Header("Collision")]
        [SerializeField] private bool generateCollider = true;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private PolygonCollider2D roadCollider;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            // 런타임 시작 시 MapManager의 최종 worldWaypoints로 재생성
            Generate();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                Generate();
        }

        [ContextMenu("Generate Road Mesh")]
        public void Generate()
        {
            if (mapManager == null) return;
            if (mapManager.worldWaypoints == null || mapManager.worldWaypoints.Count < 2) return;

            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            List<Vector3> raw = new List<Vector3>(mapManager.worldWaypoints);
            List<Vector3> simplified = new List<Vector3>();
            LineUtility.Simplify(raw, simplifyTolerance, simplified);

            if (simplified.Count < 2)
                simplified = raw;

            // 메시는 단순화된 포인트(성능), 콜라이더는 원본(LineRenderer와 일치)
            meshFilter.sharedMesh = BuildRoadMesh(simplified);
            Debug.Log($"[MapRoadGenerator] 원본 {raw.Count}pt → 단순화 {simplified.Count}pt");

            ApplyMaterial();

            if (generateCollider)
                BuildCollider(raw);
        }

        private Mesh BuildRoadMesh(List<Vector3> points)
        {
            int n = points.Count;
            var vertices = new Vector3[n * 2];
            var triangles = new int[(n - 1) * 6];
            var uvs = new Vector2[n * 2];

            float half = roadWidth * 0.5f;
            float dist = 0f;

            for (int i = 0; i < n; i++)
            {
                Vector3 perp = GetPerpendicular(points, i);
                vertices[i * 2]     = transform.InverseTransformPoint(points[i] + perp * half);
                vertices[i * 2 + 1] = transform.InverseTransformPoint(points[i] - perp * half);

                if (i > 0)
                    dist += Vector3.Distance(points[i], points[i - 1]);

                uvs[i * 2]     = new Vector2(0f, dist);
                uvs[i * 2 + 1] = new Vector2(1f, dist);
            }

            for (int i = 0; i < n - 1; i++)
            {
                int ti = i * 6;
                int vi = i * 2;
                triangles[ti]     = vi;
                triangles[ti + 1] = vi + 2;
                triangles[ti + 2] = vi + 1;
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + 2;
                triangles[ti + 5] = vi + 3;
            }

            var mesh = new Mesh { name = "RoadMesh" };
            mesh.vertices  = vertices;
            mesh.triangles = triangles;
            mesh.uv        = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private Vector3 GetPerpendicular(List<Vector3> pts, int i)
        {
            if (i == 0)
            {
                Vector3 d = (pts[1] - pts[0]).normalized;
                return new Vector3(-d.y, d.x, 0f);
            }
            if (i == pts.Count - 1)
            {
                Vector3 d = (pts[i] - pts[i - 1]).normalized;
                return new Vector3(-d.y, d.x, 0f);
            }

            // Miter joint: 앞뒤 세그먼트 법선의 평균 방향으로 오프셋
            // 일정한 도로 폭을 유지하기 위해 cos(각도)로 보정하되
            // 지나치게 날카로운 꺾임은 2배로 제한(miter limit)
            Vector3 d1 = (pts[i] - pts[i - 1]).normalized;
            Vector3 d2 = (pts[i + 1] - pts[i]).normalized;
            Vector3 n1 = new Vector3(-d1.y, d1.x, 0f);
            Vector3 n2 = new Vector3(-d2.y, d2.x, 0f);

            Vector3 miter = (n1 + n2).normalized;
            float cosA = Vector3.Dot(miter, n1);
            float scale = Mathf.Min(1f / Mathf.Max(cosA, 0.5f), 2f);
            return miter * scale;
        }

        private void ApplyMaterial()
        {
            if (meshRenderer == null) return;

            meshRenderer.enabled = showMesh;

            if (meshRenderer.sharedMaterial == null)
            {
                var mat = new Material(Shader.Find("Sprites/Default"));
                mat.name = "RoadMaterial";
                meshRenderer.sharedMaterial = mat;
            }

            meshRenderer.sharedMaterial.color = roadColor;
            meshRenderer.sortingOrder = sortingOrder;
        }

        private void BuildCollider(List<Vector3> points)
        {
            float half = roadWidth * 0.5f;
            int n = points.Count;
            var path = new Vector2[n * 2];

            for (int i = 0; i < n; i++)
            {
                Vector3 perp = GetPerpendicular(points, i);
                path[i] = (Vector2)transform.InverseTransformPoint(points[i] + perp * half);
            }
            for (int i = 0; i < n; i++)
            {
                Vector3 perp = GetPerpendicular(points, i);
                path[n * 2 - 1 - i] = (Vector2)transform.InverseTransformPoint(points[i] - perp * half);
            }

            if (roadCollider == null)
                roadCollider = GetComponent<PolygonCollider2D>();
            if (roadCollider == null)
                roadCollider = gameObject.AddComponent<PolygonCollider2D>();

            roadCollider.pathCount = 1;
            roadCollider.SetPath(0, path);
            roadCollider.isTrigger = true;
        }


    }
}
