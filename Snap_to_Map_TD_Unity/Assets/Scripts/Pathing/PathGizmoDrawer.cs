using System.Collections.Generic;
using UnityEngine;

namespace SnapToMapTD.Pathing
{
    [ExecuteAlways]
    public sealed class PathGizmoDrawer : MonoBehaviour
    {
        [Header("Path Source")]
        [SerializeField] private string fileName = "path_data.json";
        [SerializeField] private Vector2 mapPixelSize = new Vector2(1000f, 400f);
        [SerializeField] private float pixelsPerUnit = 100f;
        [SerializeField] private Vector3 worldOrigin = Vector3.zero;
        [SerializeField] private PathPlane plane = PathPlane.XZ;
        [SerializeField] private bool invertY = true;
        [SerializeField] private bool includeStartPoint = true;
        [SerializeField] private bool includeEndPoint = true;

        [Header("Gizmos")]
        [SerializeField] private Color pathColor = Color.cyan;
        [SerializeField] private float pointRadius = 0.08f;

        private void OnDrawGizmos()
        {
            try
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
                    includeEndPoint = includeEndPoint
                };

                List<Vector3> points = loader.LoadWorldPath();
                if (points.Count == 0)
                {
                    return;
                }

                Gizmos.color = pathColor;
                Vector3 previous = points[0];
                Gizmos.DrawSphere(previous, pointRadius);

                for (int i = 1; i < points.Count; i++)
                {
                    Vector3 current = points[i];
                    Gizmos.DrawLine(previous, current);
                    Gizmos.DrawSphere(current, pointRadius);
                    previous = current;
                }
            }
            catch
            {
                // Ignore missing-file and editor-only preview errors.
            }
        }
    }
}
