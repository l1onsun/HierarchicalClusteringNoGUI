using System.Collections.Generic;
using System.IO;

namespace TestHierarchicalСlustering
{
    class DataReader
    {
        private readonly List<string[]> data;
        public DataReader()
        {
            this.data = new();
        }
        public DataReader ReadCsv(string path)
        {
            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                this.data.Add(reader.ReadLine().Split(';'));
            }
            return this;
        }
        public static DataReader FromCsv(string path)
        {
            DataReader dataReader = new();
            dataReader.ReadCsv(path);
            return dataReader;
        }
        public List<HCPoint> ToPoints()
        {
            List<HCPoint> points = new();
            foreach (string[] line in this.data)
            {
                points.Add(
                    new HCPoint(
                        line[0],
                        double.Parse(line[2], System.Globalization.CultureInfo.InvariantCulture),
                        double.Parse(line[3], System.Globalization.CultureInfo.InvariantCulture)
                    )
                );
            }
            return points;
        }
    }
}
