using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using QuikStatsFileProcessor.Game_Data;
using QuikStatsFileProcessor.Data_Processing;


namespace QuikStatsFileProcessor
{
    public partial class Form1 : Form
    {
        public GameData gameData;

        public Form1()
        {
            InitializeComponent();
            gameData = new GameData();
        }

        private void buttonLoadOuterField_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            XmlDocument doc = new XmlDocument();

            if (result == DialogResult.OK) // Test result.
            {
                try
                {    
                    //Create the XmlDocument.
                    doc.Load(openFileDialog1.FileName);
                }
                catch (IOException)
                {
                }

                gameData.fieldData.SetFieldCorners(doc);
            }
        }

        private void buttonLoadInnerField_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
        }

        private void buttonLoadGPS_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            XmlDocument doc = new XmlDocument();

            if (result == DialogResult.OK) // Test result.
            {
                try
                {
                    //Create the XmlDocument.
                    doc.Load(openFileDialog1.FileName);
                }
                catch (IOException)
                {
                }

                gameData.gpsData.LoadGPSData(doc);
            }
        }

        private void buttonLoadQuikStats_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            

            if (result == DialogResult.OK) // Test result.
            {
                try
                {
                    StreamReader csv = new StreamReader(openFileDialog1.FileName);
                    gameData.eventData.LoadEventData(csv);

                    //Create the XmlDocument.
                    //csv.Load(openFileDialog1.FileName);
                }
                catch (IOException)
                {
                }

            }
        }

        private void buttonGPSToArrayVar_Click(object sender, EventArgs e)
        {
            gameData.gpsData.GPStoArrayVar();
        }

        private void buttonFlipGPS_Click(object sender, EventArgs e)
        {
            foreach(TSEvent tsEvent in gameData.eventData.tsEvents)
            {
                if(tsEvent.what == "Point")
                {
                    foreach(TSCoord tsCoord in gameData.gpsData.tsCoords)
                    {
                        if (tsCoord.time.TimeOfDay > tsEvent.when.TimeOfDay)
                        {
                            tsCoord.flip(gameData.fieldData.centerLat, gameData.fieldData.centerLng);
                        }
                    }
                }
            }
        }
    }
}
