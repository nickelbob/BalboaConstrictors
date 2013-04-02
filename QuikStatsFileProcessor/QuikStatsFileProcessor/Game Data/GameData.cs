using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace QuikStatsFileProcessor.Game_Data
{
    public class FieldData
    {
        double cornerLatSE;
        double cornerLngSE;
        double cornerLatSW;
        double cornerLngSW;
        double cornerLatNW;
        double cornerLngNW;
        double cornerLatNE;
        double cornerLngNE;

        double endZoneLatSE;
        double endZoneLngSE;
        double endZoneLatSW;
        double endZoneLngSW;
        double endZoneLatNW;
        double endZoneLngNW;
        double endZoneLatNE;
        double endZoneLngNE;

        double maxLat;
        double minLat;
        double maxLng;
        double minLng;

        public double centerLat;
        public double centerLng;

        double fieldLength;
        double fieldWidth;

        int fieldOrientation; // 0 runs long N/S, 1 runs long E/W

        public void SetFieldCorners(XmlDocument doc)
        {
            //Look through all the coordinates recording max and min
            //  Lng W      Lat N
            // -117.159302 32.732874 58.0
            XmlNodeList elemListGPS = doc.GetElementsByTagName("gx:coord");

            double lat = double.Parse(elemListGPS[0].InnerXml.Split(' ')[1]);
            double lng = double.Parse(elemListGPS[0].InnerXml.Split(' ')[0]);

            cornerLatSE = lat;
            cornerLngSE = lng;
            cornerLatSW = lat;
            cornerLngSW = lng;
            cornerLatNW = lat;
            cornerLngNW = lng;
            cornerLatNE = lat;
            cornerLngNE = lng;

            maxLat = lat;
            minLat = lat;
            maxLng = lng;
            minLng = lng;

            for (int i = 0; i < elemListGPS.Count; i++)
            {
                Console.WriteLine(elemListGPS[i].InnerXml);

                lat = double.Parse(elemListGPS[i].InnerXml.Split(' ')[1]);
                lng = double.Parse(elemListGPS[i].InnerXml.Split(' ')[0]);

                if (lat > maxLat) maxLat = lat;
                if (lat < minLat) minLat = lat;
                if (lng > maxLng) maxLng = lng;
                if (lng < minLng) minLng = lng;
            }

            centerLat = (maxLat + minLat) / 2.0;
            centerLng = (maxLng + minLng) / 2.0;
            
            fieldLength = maxLat - minLat;
            fieldWidth = maxLng - minLng;
            fieldOrientation = 0; // runs long N/S
            if (fieldLength < fieldWidth)
            {
                double tmp = fieldLength;
                fieldLength = fieldWidth;
                fieldWidth = tmp;
                fieldOrientation = 1; // runs long E/W
            }

            //Display all the times.
            XmlNodeList elemListTimes = doc.GetElementsByTagName("when");
            for (int i = 0; i < elemListTimes.Count; i++)
            {
                Console.WriteLine(elemListTimes[i].InnerXml);
            } 
        }
    }

    public class TSCoord
    {
        public DateTime time;
        public double lat;
        public double lng;

        public void flip(double centerLat, double centerLng)
        {
            lat = centerLat - (lat - centerLat);
            lng = centerLng - (lng - centerLng);
        }
    }

    public class GPSData
    {
        public List<TSCoord> tsCoords;

        public GPSData()
        {
            tsCoords = new List<TSCoord>();
        }

        public void LoadGPSData(XmlDocument doc)
        {
            XmlNodeList elemListTimes = doc.GetElementsByTagName("when");
            for (int i = 0; i < elemListTimes.Count; i++) // Just Debug, write the times out
            {
                Console.WriteLine(elemListTimes[i].InnerXml);
            }

            //Look through all the coordinates recording max and min
            //  Lng W      Lat N
            // -117.159302 32.732874 58.0
            XmlNodeList elemListGPS = doc.GetElementsByTagName("gx:coord");
            for (int i = 0; i < elemListGPS.Count; i++)
            {
                Console.WriteLine(elemListGPS[i].InnerXml);

                TSCoord tsCoord = new TSCoord();
                tsCoord.time = DateTime.Parse(elemListTimes[i].InnerXml);
                tsCoord.lat = double.Parse(elemListGPS[i].InnerXml.Split(' ')[1]);
                tsCoord.lng = double.Parse(elemListGPS[i].InnerXml.Split(' ')[0]);
                tsCoords.Add(tsCoord);
            }
        }

        public void GPStoArrayVar()
        {
            StringBuilder arrayVar = new StringBuilder();

            foreach (TSCoord tsCoord in tsCoords)
            {
                arrayVar.Append("{\"x\":");
                arrayVar.Append(tsCoord.lng);
                arrayVar.Append(",\"y\":");
                arrayVar.Append(tsCoord.lat);
                arrayVar.Append("},");
            }
            arrayVar.Length -= 1;

            Clipboard.SetText(arrayVar.ToString());
        }
    }

    public class TSEvent
    {
        public string who { get; set; }
        public DateTime when { get; set; }
        public string what { get; set; }
    }

    public class EventData
    {
        public List<TSEvent> tsEvents;

        public EventData()
        {
            tsEvents = new List<TSEvent>();
        }

        public void LoadEventData(StreamReader csv)
        {
            var fields = csv.ReadLine().Split(','); //clear header
            while (!csv.EndOfStream)
            {
                fields = csv.ReadLine().Split(',');
                tsEvents.Add(new TSEvent()
                {
                    who = fields[0],
                    what = fields[1],
                    when = DateTime.ParseExact(fields[2], "HH:mm:ss:fff", CultureInfo.InvariantCulture)
                    //Convert.ToDateTime(fields[2])
                });
            }
        }
    }

    public class GameData
    {
        public FieldData fieldData;
        public GPSData gpsData;
        public EventData eventData;

        public GameData()
        {
            fieldData = new FieldData();
            gpsData = new GPSData();
            eventData = new EventData();
        }
    }
}
