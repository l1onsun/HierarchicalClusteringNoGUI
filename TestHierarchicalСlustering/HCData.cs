using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestHierarchicalСlustering
{
    class HCPoint
    {
        public readonly string Name;
        public readonly double X, Y;
        public HCPoint(string name, double x, double y)
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
        }
        override public string ToString()
        {
            if (Name != null)
            {
                return Name;
            }
            else
            {
                string x = X.ToString();
                string y = Y.ToString();
                return $"({x},{y})";
            }
        }
    }
    class HCCluster
    {
        private static int nextClusterOrder = 0;

        public readonly List<HCPoint> Points;
        public readonly int Order;
        public HCCluster(List<HCPoint> points)
        {
            this.Order = nextClusterOrder;
            nextClusterOrder++;
            this.Points = points;
        }
        public static HCCluster FromSinglePoint(HCPoint point)
        {
            List<HCPoint> pointList = new();
            pointList.Add(point);
            return new(pointList);
        }
        public static HCCluster Join(HCCluster a, HCCluster b)
        {
            List<HCPoint> pointList = new(a.Points);
            pointList.AddRange(b.Points);
            return new(pointList);
        }
        public override string ToString()
        {
            // StringBuilder sb = new($"*{Order}{{");
            StringBuilder sb = new("{");
            foreach (HCPoint p in this.Points)
            {
                sb.Append(p.ToString());
            }
            sb.Append('}');
            return sb.ToString();
        }
        public static List<HCCluster> ClusterPerPoint(List<HCPoint> points)
        {
            List<HCCluster> clusters = new();
            foreach (HCPoint p in points)
            {
                clusters.Add(HCCluster.FromSinglePoint(p));
            }
            return clusters;
        }
    }
    class HCIteration
    {
        public readonly List<HCCluster> Clusters;
        public (HCCluster I, HCCluster J) ClosestClusters;
        public double MinDistance;

        public HCIteration(List<HCCluster> clusters, double minDistance, (HCCluster I, HCCluster J) closestClusters)
        {
            this.Clusters = clusters;
            this.MinDistance = minDistance;
            this.ClosestClusters = closestClusters;
        }
        public override string ToString()
        {
            StringBuilder sb = new($"< {MinDistance:0.####}: ");
            sb.Append(new String(' ', Math.Max(0, 10 - sb.Length)));

            foreach (HCCluster cluster in this.Clusters)
            {
                sb.Append($"{cluster} ");
            }
            sb.Append('>');
            return sb.ToString();
        }
    }
    class HCState
    {
        public readonly List<HCIteration> Iterations;

        public HCState()
        {
            this.Iterations = new();
        }
        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (HCIteration i in this.Iterations)
            {
                sb.Append(i.ToString());
            }
            return sb.ToString();
        }
    }
}
