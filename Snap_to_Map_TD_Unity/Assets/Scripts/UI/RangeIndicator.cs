using UnityEngine;

namespace SnapToMapTD.UI
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RangeIndicator : MonoBehaviour
    {
        [SerializeField] private int segments = 60;
        [SerializeField] private Color fillColor = new Color(0.7f, 0.7f, 0.7f, 0.25f);

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = fillColor;
            meshRenderer.material = mat;
            meshRenderer.sortingOrder = 5;

            gameObject.SetActive(false);
        }

        public void Show(Vector3 worldPos, float radius)
        {
            transform.position = worldPos;
            meshFilter.mesh = BuildCircleMesh(radius);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private Mesh BuildCircleMesh(float radius)
        {
            var vertices = new Vector3[segments + 1];
            var triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * 2f * Mathf.PI / segments;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            }

            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3]     = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % segments + 1;
            }

            var mesh = new Mesh();
            mesh.vertices  = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
