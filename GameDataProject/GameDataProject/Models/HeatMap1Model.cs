using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDataProject.Models
{
    public class HeatMap1Model
    {
        public HeatMap1Model()
        {
            GPSData = new Dictionary<string, List<GPSCoord>>();
            BoundaryData = new Dictionary<string, List<GPSCoord>>();
            minMax = new Dictionary<string, MinMaxObj>();
        }

        public Dictionary<string, List<GPSCoord>> GPSData { get; set; }
        public Dictionary<string, List<GPSCoord>> BoundaryData { get; set; }
        public Dictionary<string, MinMaxObj> minMax { get; set; }
        
    }

    public class GPSCoord
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class MinMaxObj
    {
        public MinMaxObj()
        {
            minY = Double.MaxValue;
            minX = Double.MaxValue;
            maxY = Double.MinValue;
            maxX = Double.MinValue;
        }

        public double minX { get; set; }
        public double maxX { get; set; }
        public double minY { get; set; }
        public double maxY { get; set; }
    }
}
