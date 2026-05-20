using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SnapToMapTD.Pathing
{
    public enum PathPlane
    {
        XY,
        XZ
    }

    public sealed class MapPathLoader
    {
        public string fileName = "path_data.json";
        public Vector2 mapPixelSize = new Vector2(1000f, 400f);
        public float pixelsPerUnit = 100f;
        public Vector3 worldOrigin = Vector3.zero;
        public PathPlane plane = PathPlane.XZ;
        public bool invertY = true;
        public bool includeStartPoint = true;
        public bool includeEndPoint = true;
        public bool preferJsonImageSize = true;

        public MapPathData LoadRaw()
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Could not find path JSON at {fullPath}", fullPath);
            }

            string json = File.ReadAllText(fullPath);
            return MapPathJsonParser.Parse(json);
        }

        public List<Vector3> LoadWorldPath()
        {
            return ConvertToWorld(LoadRaw());
        }

        public List<Vector3> ConvertToWorld(MapPathData data)
        {
            Vector2 sourcePixelSize = ResolveSourcePixelSize(data);
            var points = new List<Vector3>();

            if (includeStartPoint)
            {
                points.Add(ConvertPoint(data.start_point, sourcePixelSize));
            }

            foreach (PathPoint point in data.path_points)
            {
                points.Add(ConvertPoint(point, sourcePixelSize));
            }

            if (includeEndPoint)
            {
                points.Add(ConvertPoint(data.end_point, sourcePixelSize));
            }

            return points;
        }

        public Vector3 ConvertPoint(PathPoint point)
        {
            return ConvertPoint(point, mapPixelSize);
        }

        public Vector3 ConvertPoint(PathPoint point, Vector2 sourcePixelSize)
        {
            float halfWidth = sourcePixelSize.x * 0.5f;
            float halfHeight = sourcePixelSize.y * 0.5f;

            float centeredX = (point.x - halfWidth) / pixelsPerUnit;
            float centeredY = invertY
                ? (halfHeight - point.y) / pixelsPerUnit
                : (point.y - halfHeight) / pixelsPerUnit;

            return plane == PathPlane.XY
                ? worldOrigin + new Vector3(centeredX, centeredY, 0f)
                : worldOrigin + new Vector3(centeredX, 0f, centeredY);
        }

        private Vector2 ResolveSourcePixelSize(MapPathData data)
        {
            if (preferJsonImageSize && data.image_size.x > 0 && data.image_size.y > 0)
            {
                return new Vector2(data.image_size.x, data.image_size.y);
            }

            return mapPixelSize;
        }
    }
}
