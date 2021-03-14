using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TestHierarchicalСlustering
{
    class Metric
    {
        public static double Euclid(HCPoint a, HCPoint b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
        public static double SingleLinkage(HCCluster a, HCCluster b)
        {
            double minDistance = double.PositiveInfinity;
            for (var i = 0; i < a.Points.Count; i++)
            {
                for (var j = 0; j < b.Points.Count; j++)
                {
                    minDistance = Math.Min(minDistance, Metric.Euclid(a.Points[i], b.Points[j]));
                }
            }
            return minDistance;
        }

        public static double LanceWillamsSingleLinkage(IDistanceMatrix dm, HCCluster joinedA, HCCluster joinedB, HCCluster other)
        {
            return 0.5 * dm.GetDistance(joinedA, other)
                   + 0.5 * dm.GetDistance(joinedB, other)
                   - 0.5 * Math.Abs(dm.GetDistance(joinedA, other) - dm.GetDistance(joinedB, other));
        }
    }

    interface IDistanceMatrix
    {
        static public (int, int) Ordered(HCCluster a, HCCluster b)
        {
            int i = Math.Min(a.Order, b.Order);
            int j = Math.Max(a.Order, b.Order);
            return (i, j);
        }
        public double GetDistance(HCCluster a, HCCluster b);
        public void SetDistance(HCCluster a, HCCluster b, double distance);
    }

    class DistanceMatrix : IDistanceMatrix
    {
        private readonly Dictionary<(int, int), double> matrix;
        public DistanceMatrix()
        {
            this.matrix = new();
        }
        public double GetDistance(HCCluster a, HCCluster b)
        {
            return matrix[IDistanceMatrix.Ordered(a, b)];
        }
        public void SetDistance(HCCluster a, HCCluster b, double distance)
        {
            matrix[IDistanceMatrix.Ordered(a, b)] = distance;
        }
    }

    class ConcurrentDistanceMatrix : IDistanceMatrix
    {
        private readonly ConcurrentDictionary<(int, int), double> matrix;
        public ConcurrentDistanceMatrix()
        {
            this.matrix = new();
        }
        public double GetDistance(HCCluster a, HCCluster b)
        {
            return matrix[IDistanceMatrix.Ordered(a, b)];
        }
        public void SetDistance(HCCluster a, HCCluster b, double distance)
        {
            matrix[IDistanceMatrix.Ordered(a, b)] = distance;
        }
    }
}
