using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.RandomDataPlugin
{
    internal static class Generator
    {
        internal static SlamPoint[] GeneratePoints(int minId, int amount, float scale)
        {
            var rand = new System.Random();
            var result = new SlamPoint[amount];
            for (int i = 0; i < amount; i++)
            {
                result[i] = new SlamPoint
                {
                    Id = minId + i,
                    Position = new Vector3((float) rand.NextDouble() * scale,
                                           (float) rand.NextDouble() * scale,
                                           (float) rand.NextDouble() * scale),
                    Color = new Color((float) rand.NextDouble(), (float) rand.NextDouble(),
                                      (float) rand.NextDouble()),
                    Message = "",
                };
            }

            return result;
        }

        internal static SlamPoint[] UpdatePoints(IList<SlamPoint> points, int amount, float scale)
        {
            var rand = new System.Random();
            var result = new SlamPoint[amount];
            for (int i = 0; i < amount; i++)
            {
                var slamPoint = points[rand.Next(points.Count)];
                slamPoint.Position = new Vector3((float) rand.NextDouble() * 10,
                                                 (float) rand.NextDouble() * 10,
                                                 (float) rand.NextDouble() * 10);
                result[i] = slamPoint;
            }

            return result;
        }

        internal static int[] RemovePoints(IList<SlamPoint> points, int amount)
        {
            var rand = new System.Random();
            var removingIds = new HashSet<int>();
            for (int i = 0; i < amount; i++)
            {
                removingIds.Add(points[rand.Next(points.Count)].Id);
            }

            return removingIds.ToArray();
        }

        internal static SlamLine[] GenerateConnections(IList<SlamPoint> points, int amount)
        {
            var rand = new System.Random();
            var result = new SlamLine[amount];
            for (int i = 0; i < amount; i++)
            {
                result[i] = new SlamLine(points[rand.Next(points.Count)], points[rand.Next(points.Count)]);
            }

            return result;
        }
    }
}