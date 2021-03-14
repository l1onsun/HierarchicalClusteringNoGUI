using System;
using System.Collections.Generic;
using System.Linq;

namespace TestHierarchicalСlustering
{

    class HCBaseAlgorithm<Matrix> where Matrix : IDistanceMatrix
    {

        public HCState State;
        public Matrix DistanceMatrix;

        public virtual HCClusterPair FindClosestPair(
            IEnumerable<(HCCluster I, HCCluster J)> clusterPairs,
            Func<HCCluster, HCCluster, double> distanceFunc
        )
        {
            return DistanceMatrix.FindClosestPair(clusterPairs, distanceFunc);
        }

        public HCBaseAlgorithm(Matrix distanceMatrix)
        {
            this.DistanceMatrix = distanceMatrix;
        }

        public void InitState(List<HCPoint> points)
        {
            State = new();

            var clusters = HCCluster.ClusterPerPoint(points);
            var closest = FindClosestPair(HCCluster.AllPairs(clusters), Metric.SingleLinkage);

            State.Iterations.Add(new HCIteration(
                clusters: clusters,
                closestPair: closest
            ));
        }

        public bool Step()
        {
            var prevIteration = State.Iterations.Last();
            if (prevIteration.Clusters.Count <= 1)
            {
                return false;
            }
            HCCluster clusterI = prevIteration.ClosestPair.I;
            HCCluster clusterJ = prevIteration.ClosestPair.J;
            HCCluster joinedCluster = HCCluster.Join(clusterI, clusterJ);

            List<HCCluster> newClusters = new();
            foreach (HCCluster oldCluster in prevIteration.Clusters)
            {
                if (oldCluster != clusterI && oldCluster != clusterJ)
                {
                    newClusters.Add(oldCluster);
                }
            }
            var closest = FindClosestPair(
                clusterPairs: joinedCluster.PairsWith(newClusters),
                distanceFunc: (joinedCluster, other) => Metric.LanceWillamsSingleLinkage(DistanceMatrix, clusterI, clusterJ, other)
            );
            newClusters.Add(joinedCluster);

            if (closest.Distance > prevIteration.ClosestPair.Distance)
            {
                closest = FindClosestPair(HCCluster.AllPairs(newClusters), DistanceMatrix.GetDistance);
            }

            State.Iterations.Add(new HCIteration(
                clusters: newClusters,
                closestPair: closest
            ));
            return true;
        }

        public string LastIterationInfo()
        {
            return State.Iterations.Last().ToString();
        }

    }
}
