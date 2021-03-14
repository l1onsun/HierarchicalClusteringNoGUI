using System;
using System.Collections.Generic;
using System.Linq;

namespace TestHierarchicalСlustering
{
    class SyncDistanceMatrix : IDistanceMatrix
    {
        private readonly Dictionary<(int, int), double> matrix;
        public SyncDistanceMatrix()
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
            HCClusterPair closest = new(
                clusterI: null,
                clusterJ: null,
                distance: double.PositiveInfinity
            );

            foreach ((HCCluster I, HCCluster J) clusterPair in clusterPairs)
            {
                double distance = distanceFunc(clusterPair.I, clusterPair.J);
                SetDistance(clusterPair.I, clusterPair.J, distance);

                if (distance < closest.Distance)
                {
                    (closest.I, closest.J) = clusterPair;
                    closest.Distance = distance;
                }
            }

            return closest;
        }
    }

    class HCSyncAlgorithm : HCBaseAlgorithm<SyncDistanceMatrix>
    {
        public HCSyncAlgorithm() : base(new SyncDistanceMatrix()) {}
    }
}
