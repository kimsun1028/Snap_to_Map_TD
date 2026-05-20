using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SnapToMapTD.Pathing
{
    [Serializable]
    public struct PathPoint
    {
        public int x;
        public int y;

        public PathPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }

    [Serializable]
    public sealed class MapPathData
    {
        public PathPoint start_point;
        public PathPoint end_point;
        public PathPoint image_size;
        public List<PathPoint> path_points = new List<PathPoint>();
    }

    public static class MapPathJsonParser
    {
        private static readonly Regex StartRegex = new Regex("\"start_point\"\\s*:\\s*\\[\\s*(-?\\d+)\\s*,\\s*(-?\\d+)\\s*\\]", RegexOptions.Compiled);
        private static readonly Regex EndRegex = new Regex("\"end_point\"\\s*:\\s*\\[\\s*(-?\\d+)\\s*,\\s*(-?\\d+)\\s*\\]", RegexOptions.Compiled);
        private static readonly Regex ImageSizeRegex = new Regex("\"image_size\"\\s*:\\s*\\[\\s*(-?\\d+)\\s*,\\s*(-?\\d+)\\s*\\]", RegexOptions.Compiled);
        private static readonly Regex PathPointsRegex = new Regex("\"path_points\"\\s*:\\s*\\[(?<block>.*?)\\]\\s*,\\s*\"contours\"", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex PointRegex = new Regex("\\[\\s*(-?\\d+)\\s*,\\s*(-?\\d+)\\s*\\]", RegexOptions.Compiled);

        public static MapPathData Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON text is empty.", nameof(json));
            }

            Match startMatch = StartRegex.Match(json);
            Match endMatch = EndRegex.Match(json);
            if (!startMatch.Success || !endMatch.Success)
            {
                throw new FormatException("Could not find start_point or end_point in the JSON text.");
            }

            var data = new MapPathData
            {
                start_point = new PathPoint(ParseInt(startMatch.Groups[1]), ParseInt(startMatch.Groups[2])),
                end_point = new PathPoint(ParseInt(endMatch.Groups[1]), ParseInt(endMatch.Groups[2]))
            };

            Match imageMatch = ImageSizeRegex.Match(json);
            if (imageMatch.Success)
            {
                data.image_size = new PathPoint(ParseInt(imageMatch.Groups[1]), ParseInt(imageMatch.Groups[2]));
            }

            Match pathBlockMatch = PathPointsRegex.Match(json);
            if (!pathBlockMatch.Success)
            {
                throw new FormatException("Could not find path_points array in the JSON text.");
            }

            string pathBlock = pathBlockMatch.Groups["block"].Value;
            MatchCollection pointMatches = PointRegex.Matches(pathBlock);
            data.path_points = new List<PathPoint>(pointMatches.Count);

            foreach (Match pointMatch in pointMatches)
            {
                data.path_points.Add(new PathPoint(ParseInt(pointMatch.Groups[1]), ParseInt(pointMatch.Groups[2])));
            }

            return data;
        }

        private static int ParseInt(Group group)
        {
            return int.Parse(group.Value, CultureInfo.InvariantCulture);
        }
    }
}
