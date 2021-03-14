﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TestHierarchicalСlustering
{
    using MDict = Dictionary<(int, int), double>;
    
    class HCOneThreadedAlgorithm
    {
        
        public HCState State;
        public DistanceMatrix<MDict> DistanceMatrix;


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
                DistanceMatrix.SetDistance(clusterPair.I, clusterPair.J, distance);

                if (distance < closest.Distance)
                {
                    (closest.I, closest.J) = clusterPair;
                    closest.Distance = distance;
                }
            }

            return closest;
        }


        public void InitState(List<HCPoint> points)
        {
            State = new();
            DistanceMatrix = new(new MDict());

            var clusters = HCCluster.ClusterPerPoint(points);
            var closest = FindClosestPair(HCCluster.AllPairs(clusters), Metric.SingleLinkage);

            State.Iterations.Add(new HCIteration(
                clusters: clusters,
                closestPair: closest
            ));
        }

        double LanceWillamsSingleLinkage(HCCluster joinedA, HCCluster joinedB, HCCluster other)
        {
            return 0.5 * DistanceMatrix.GetDistance(joinedA, other)
                   + 0.5 * DistanceMatrix.GetDistance(joinedB, other)
                   - 0.5 * Math.Abs(DistanceMatrix.GetDistance(joinedA, other) - DistanceMatrix.GetDistance(joinedB, other));
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
                distanceFunc: (joinedCluster, other) => LanceWillamsSingleLinkage(clusterI, clusterJ, other)
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
