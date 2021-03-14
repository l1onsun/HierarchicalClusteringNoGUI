using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace TestHierarchicalСlustering
{
    public static class HashSetExt
    {
        public static void Print(this HashSet<int> hs)
        {
            Console.Write("{");
            foreach (int i in hs)
            {
                Console.Write(" {0}", i);
            }
            Console.WriteLine(" }");
        }
    }
    class Program
    {
        static void TestingDistanceMatrix()
        {

        }
        static void Main()
        {
            Console.WriteLine("----");
            Console.WriteLine(RuntimeInformation.FrameworkDescription);

            List<HCPoint> points = DataReader.FromCsv("points.csv").ToPoints();
            
            HCMultiThreadedAlgorithm algorithm = new();
            algorithm.InitState(points);

            Console.WriteLine(algorithm.LastIterationInfo());
            
            while(algorithm.Step())
            {
                Console.WriteLine(algorithm.LastIterationInfo());
            }
        }
    }
}
