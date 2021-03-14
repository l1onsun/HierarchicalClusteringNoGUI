using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace TestHierarchicalСlustering
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("C# version:");
            Console.WriteLine(RuntimeInformation.FrameworkDescription);
            Console.WriteLine();

            List<HCPoint> points = DataReader.FromCsv("points2.csv").ToPoints();


            Console.WriteLine("Start sync HC:");

            HCSyncAlgorithm algorithmSync = new();
            algorithmSync.InitState(points);

            while (algorithmSync.Step());
            Console.WriteLine(algorithmSync.State);


            Console.WriteLine("Start concurrent HC:");

            HCConcurrentAlgorithm algorithmConcurrent = new();
            algorithmConcurrent.InitState(points);

            while (algorithmConcurrent.Step());
            Console.WriteLine(algorithmConcurrent.State);
        }
    }
}
