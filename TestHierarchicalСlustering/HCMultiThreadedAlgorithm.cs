using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHierarchicalСlustering
{
    class HCConcurentDistanceMatrix
    {
        public ConcurrentDictionary<(int, int), double> matrix;
        public HCConcurentDistanceMatrix()
        {
            matrix = new();
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
    class HCMultiThreadedAlgorithm
    {
        HCState State;
        HCConcurentDistanceMatrix DistanceMatrix;

        public void FindClosest(IEnumerable<(HCCluster I, HCCluster J)> clusters, Func<HCCluster, HCCluster, double> distanceFunc)
        {

        }

        public void InitState(List<HCPoint> points)
        {
            State = new();
            DistanceMatrix = new();

            var clusters = HCCluster.ClusterPerPoint(points);
            (HCCluster, HCCluster) closest = (null, null);
            double minDistance = double.PositiveInfinity;

            for (var i = 0; i < clusters.Count; i++)
            {
                for (var j = i + 1; j < clusters.Count; j++)
                {
                    double distance = Metric.SingleLinkage(clusters[i], clusters[j]);
                    DistanceMatrix.SetDistance(clusters[i], clusters[j], distance);
                    if (distance < minDistance)
                    {
                        closest = (clusters[i], clusters[j]);
                        minDistance = distance;
                    }
                }
            }

            //State.Iterations.Add(new HCIteration(
            //    clusters: clusters,
            //    minDistance: minDistance,
            //    closestClusters: closest
            //));
        }
    }
}
