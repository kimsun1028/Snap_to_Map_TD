using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnapToMapTD.Pathing
{
    public sealed class PathFollower : MonoBehaviour
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

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float waypointRadius = 0.05f;
        [SerializeField] private bool snapToPathOnStart = true;
        [SerializeField] private bool loopPath = false;

        private readonly List<Vector3> waypoints = new List<Vector3>();
        private MapPathLoader loader;
        private int waypointIndex;

        private void Awake()
        {
            ReloadPath();
        }

        private void Start()
        {
            if (snapToPathOnStart && waypoints.Count > 0)
            {
                transform.position = waypoints[0];
                waypointIndex = waypoints.Count > 1 ? 1 : 0;
            }
        }

        private void Update()
        {
            if (waypoints.Count == 0 || waypointIndex >= waypoints.Count)
            {
                return;
            }

            Vector3 target = waypoints[waypointIndex];
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            if ((transform.position - target).sqrMagnitude <= waypointRadius * waypointRadius)
            {
                waypointIndex++;
                if (waypointIndex >= waypoints.Count && loopPath && waypoints.Count > 1)
                {
                    waypointIndex = snapToPathOnStart ? 1 : 0;
                    transform.position = waypoints[0];
                }
            }
        }

        public bool ReloadPath()
        {
            try
            {
                loader = new MapPathLoader
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

                waypoints.Clear();
                waypoints.AddRange(loader.LoadWorldPath());
                waypointIndex = 0;
                return waypoints.Count > 0;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load path JSON: {exception.Message}", this);
                waypoints.Clear();
                waypointIndex = 0;
                return false;
            }
        }

        public IReadOnlyList<Vector3> GetWaypoints()
        {
            return waypoints;
        }
    }
}
