using System;
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
    }

    class DistanceMatrix<Matrix> where Matrix : IDictionary<(int, int), double>
    {
        private readonly Matrix matrix;
        public DistanceMatrix(Matrix matrix)
        {
            this.matrix = matrix;
        }
        static private (int, int) Ordered(HCCluster a, HCCluster b)
        {
            int i = Math.Min(a.Order, b.Order);
            int j = Math.Max(a.Order, b.Order);
            return (i, j);
        }
        public double GetDistance(HCCluster a, HCCluster b)
        {
            return matrix[Ordered(a, b)];
        }
        public void SetDistance(HCCluster a, HCCluster b, double distance)
        {
            matrix[Ordered(a, b)] = distance;
        }
    }
}
