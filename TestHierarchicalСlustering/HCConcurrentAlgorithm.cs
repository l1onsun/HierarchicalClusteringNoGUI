using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace TestHierarchicalСlustering
{
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
        public HCClusterPair FindClosestPair(
            IEnumerable<(HCCluster I, HCCluster J)> clusterPairs,
            Func<HCCluster, HCCluster, double> distanceFunc
        )
        {
            ConcurrentQueue<HCClusterPair> cq = new();

            Parallel.ForEach(clusterPairs, clusterPair =>
            {
                double distance = distanceFunc(clusterPair.I, clusterPair.J);
                SetDistance(clusterPair.I, clusterPair.J, distance);
                cq.Enqueue(new(
                        clusterI: clusterPair.I,
                        clusterJ: clusterPair.J,
                        distance: distance
                ));
            });

            HCClusterPair closest = cq.AsParallel().Min();
            if (closest == null)
            {
                closest = new(null, null, double.PositiveInfinity);
            }
            return closest;
        }
    }

    class HCConcurrentAlgorithm : HCBaseAlgorithm<ConcurrentDistanceMatrix>
    {
        public HCConcurrentAlgorithm() : base(new ConcurrentDistanceMatrix()) {}
    }
}
