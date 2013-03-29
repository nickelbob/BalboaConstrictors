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
            GPSData = new List<GPSCoord>();
            BoundaryData = new List<GPSCoord>();
        }

        public List<GPSCoord> GPSData { get; set; }
        public List<GPSCoord> BoundaryData { get; set; }
        public double minX { get; set; }
        public double maxX { get; set; }
        public double minY { get; set; }
        public double maxY { get; set; }
    }

    public class GPSCoord
    {
        public double x { get; set; }
        public double y { get; set; }
    }
}
