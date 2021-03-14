using System;
using System.Collections.Generic;
using System.Linq;

namespace TestHierarchicalСlustering
{
    class HCDistanceMatrix
    {
        public Dictionary<(int, int), double> matrix;
        public HCDistanceMatrix()
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

    class HCOneThreadedAlgorithm
    {
        public HCState State;
        public HCDistanceMatrix DistanceMatrix;

        //public (HCCluster clusterI, HCCluster clusterJ, double minDistance) FindClosest(List<HCCluster> clusters, Func<HCCluster, HCCluster, double> distanceFunc)
        //{
        //    double minDistance = double.PositiveInfinity;
        //    (HCCluster I, HCCluster J) closest = (null, null);

        //    for (var i = 0; i < clusters.Count; i++)
        //    {
        //        for (var j = i + 1; j < clusters.Count; j++)
        //        {
        //            double distance = distanceFunc(clusters[i], clusters[j]);
        //            State.DistanceMatrix.SetDistance(clusters[i], clusters[j], distance);
        //            if (distance < minDistance)
        //            {
        //                closest = (clusters[i], clusters[j]);
        //                minDistance = distance;
        //            }
        //        }
        //    }
        //    return (closest.I, closest.J, minDistance)
        //}

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

            State.Iterations.Add(new HCIteration(
                clusters: clusters,
                minDistance: minDistance,
                closestClusters: closest
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
            HCCluster clusterI = prevIteration.ClosestClusters.I;
            HCCluster clusterJ = prevIteration.ClosestClusters.J;
            HCCluster joinedCluster = HCCluster.Join(clusterI, clusterJ);

            List<HCCluster> newClusters = new();
            newClusters.Add(joinedCluster);

            var newMinDistance = double.PositiveInfinity;
            (HCCluster, HCCluster) closest = (null, null);
            foreach (HCCluster oldCluster in prevIteration.Clusters)
            {
                if (oldCluster != clusterI && oldCluster != clusterJ)
                {
                    newClusters.Add(oldCluster);

                    double distance = LanceWillamsSingleLinkage(clusterI, clusterJ, oldCluster);
                    if (distance < newMinDistance)
                    {
                        newMinDistance = distance;
                        closest = (joinedCluster, oldCluster);
                    }
                    DistanceMatrix.SetDistance(oldCluster, joinedCluster, distance);
                }
            }

            if (newMinDistance > prevIteration.MinDistance)
            {
                newMinDistance = double.PositiveInfinity;
                closest = (null, null);
                for (var i = 0; i < newClusters.Count; i++)
                {
                    for (var j = i + 1; j < newClusters.Count; j++)
                    {

                        double distance = DistanceMatrix.GetDistance(newClusters[i], newClusters[j]);
                        if (distance < newMinDistance)
                        {
                            newMinDistance = distance;
                            closest = (newClusters[i], newClusters[j]);
                        }
                    }
                }
            }

            State.Iterations.Add(new HCIteration(
                clusters: newClusters,
                minDistance: newMinDistance,
                closestClusters: closest
            ));
            return true;
        }

        public string LastIterationInfo()
        {
            return State.Iterations.Last().ToString();
        }
    }
}
