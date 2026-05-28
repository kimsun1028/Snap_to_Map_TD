using UnityEngine;

namespace SnapToMapTD
{
    [ExecuteAlways]
    public class PathMarkers : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapManager mapManager;

        [Header("Spawn Marker")]
        [SerializeField] private Sprite spawnSprite;
        [SerializeField] private Color spawnColor = new Color(1f, 0.3f, 0.3f, 1f);
        [SerializeField] private float spawnScale = 0.5f;

        [Header("Destination Marker")]
        [SerializeField] private Sprite destinationSprite;
        [SerializeField] private Color destinationColor = new Color(0.3f, 0.7f, 1f, 1f);
        [SerializeField] private float destinationScale = 0.5f;

        [Header("Sorting")]
        [SerializeField] private int sortingOrder = 20;

        private SpriteRenderer spawnRenderer;
        private SpriteRenderer destinationRenderer;
        private static Sprite fallbackCircle;

        private void Awake()  => EnsureMarkers();
        private void Start()  => UpdateMarkers();
        private void OnValidate()
        {
            EnsureMarkers();
            UpdateMarkers();
        }

        private void EnsureMarkers()
        {
            spawnRenderer       = EnsureMarker("SpawnMarker",       ref spawnRenderer);
            destinationRenderer = EnsureMarker("DestinationMarker", ref destinationRenderer);
        }

        private SpriteRenderer EnsureMarker(string childName, ref SpriteRenderer cached)
        {
            if (cached != null) return cached;

            Transform t = transform.Find(childName);
            if (t == null)
            {
                var go = new GameObject(childName);
                go.transform.SetParent(transform, false);
                t = go.transform;
            }

            var sr = t.GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = t.gameObject.AddComponent<SpriteRenderer>();

            return sr;
        }

        public void UpdateMarkers()
        {
            if (mapManager == null) return;
            var pts = mapManager.worldWaypoints;
            if (pts == null || pts.Count < 2) return;

            Sprite spawnFinal  = spawnSprite       != null ? spawnSprite       : GetFallbackCircle();
            Sprite destFinal   = destinationSprite != null ? destinationSprite : GetFallbackCircle();

            ApplyMarker(spawnRenderer,       pts[0],             spawnFinal, spawnColor,       spawnScale);
            ApplyMarker(destinationRenderer, pts[pts.Count - 1], destFinal,  destinationColor, destinationScale);
        }

        private void ApplyMarker(SpriteRenderer sr, Vector3 pos, Sprite sprite, Color color, float scale)
        {
            if (sr == null) return;

            sr.transform.position   = pos;
            sr.transform.localScale = Vector3.one * scale;
            sr.sprite               = sprite;
            sr.color                = color;
            sr.sortingOrder         = sortingOrder;
        }

        // 스프라이트 미지정 시 사용할 원형 텍스처를 런타임에 생성
        private static Sprite GetFallbackCircle()
        {
            if (fallbackCircle != null) return fallbackCircle;

            const int size = 64;
            float radius = size * 0.5f;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - radius;
                    float dy = y + 0.5f - radius;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    // 안쪽은 흰색, 테두리 1px 안티앨리어싱
                    float alpha = Mathf.Clamp01(radius - dist);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            tex.Apply();

            fallbackCircle = Sprite.Create(
                tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size
            );
            return fallbackCircle;
        }
    }
}
