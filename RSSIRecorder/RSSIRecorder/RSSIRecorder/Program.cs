using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NativeWifi;

namespace RSSIRecorder
{
    public class RSSIRecorder
    {
        //static RSSIRecorderEntities entity;
        static WlanClient client;
        static bool useDatabase = true, outputSQL = false, help = false;
        static string fileName = String.Empty, tableName = "RSSIRecorder";
        static int pingInterval = 2000;

        static void Main(string[] args)
        {
            parseCmdLine(args);

            if (help)
                return;

            //entity = new RSSIRecorderEntities();
            client = new WlanClient();
            Console.WriteLine("Main thread: starting to ping -");

            while (true)
            {
                getRSSI();
                Thread.Sleep(pingInterval);
            }

            //getRSSI(null);
        }

        private static void parseCmdLine(string[] args)
        {
            var cmdline = args.ToList();

            if (cmdline.Any(x => x == "-f"))
            {
                useDatabase = false;

                if (cmdline.Count() < cmdline.IndexOf("-f") + 1)
                {
                    Console.WriteLine("Please supply a filename after the -f flag.");
                    return;
                }

                fileName = cmdline[cmdline.IndexOf("-f") + 1];
            }

            if (cmdline.Any(x => x == "-t"))
            {
                outputSQL = true;

                if (cmdline.Count() >= cmdline.IndexOf("-t") + 1)
                {
                    tableName = cmdline[cmdline.IndexOf("-t") + 1];   
                }
            }

            //only scan all networks if we're not currently connected to a network
            //if (cmdline.Any(x => x == "-s"))
            //{
            //    multipleNetworks = false;
            //}

            if (cmdline.Any(x => x == "-i"))
            {
                if (cmdline.Count() < cmdline.IndexOf("-i") + 1)
                {
                    Console.WriteLine("Please supply a number of milliseconds after the '-i' flag to use as a ping interval.");
                    return;
                }

                if (!Int32.TryParse(cmdline[cmdline.IndexOf("-i") + 1], out pingInterval))
                {
                    Console.WriteLine("Ping interval parsing was not successful. Using 2000 milliseconds as default.");
                }

            }

            if (cmdline.Any(x => x == "-h"))
            {
                Console.WriteLine("Usage: The RSSI Recorder will print out the SSID, time stamp, and the signal strength of the WiFi network you are currently connected to.\n");
                Console.WriteLine("FLAGS\n\n-f <filename> - By default the program will write to the DB pointed to in the exe.config file but supplying the -f option will write to a file instead.\n\n-t <table name> - The t flag only works if the f flag is passed as well. The t flag will print SQL insert statements to a file for later import into a DB of your choosing. Some modification may be neccessary if your DB is not MSSQL Server. Your table rows must be in this order - SSID SignalStrength Date.\n\n-i <milliseconds> - The i flag allows you to control the polling interval. 2000 (2 sec) is the default.");

                help = true;
            }
        }

        private static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private static void getRSSI()
        {
            try
            {
                foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
                {
                    //Poll all networks if we're not conect to any of them at the moment
                    if (wlanInterface.InterfaceState == Wlan.WlanInterfaceState.Disconnected)
                    {
                        Wlan.WlanBssEntry[] wlanBssEntries = wlanInterface.GetNetworkBssList();
                        foreach (Wlan.WlanBssEntry network in wlanBssEntries)
                        {
                            string ssidName = GetStringForSSID(network.dot11Ssid);

                            Console.WriteLine(String.Format("{0}\t{1}\t{2}", ssidName, network.rssi.ToString(), DateTime.Now.ToString("o")));

                            persistData(network.rssi, ssidName);
                        }
                    }
                    else
                    {
                        Wlan.Dot11Ssid ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid;

                        string ssidName = new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength));

                        Console.WriteLine(String.Format("{0}\t{1}\t{2}", ssidName, wlanInterface.RSSI.ToString(), DateTime.Now.ToString("o")));

                        persistData(wlanInterface.RSSI, ssidName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured while trying to retrieve signal strength. Message - " + ex.Message + " Stack Trace - " + ex.StackTrace);
                return;
            }
            
        }

        private static void persistData(int rssi, string ssidName)
        {
            if (useDatabase)
            {
                using (RSSIRecorderEntities entity = new RSSIRecorderEntities())
                {
                    entity.RSSIRecords.Add(new RSSIRecord()
                    {
                        CreatedOnUtc = DateTime.Now,
                        SignalStrength = rssi,
                        SSID = ssidName
                    });
                    entity.SaveChanges();
                }
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
                {
                    if (outputSQL)
                    {
                        file.WriteLine(String.Format("insert into [{0}] values('{1}', {2}, '{3}');", tableName, ssidName, rssi.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")));
                    }
                    else
                    {
                        file.WriteLine(String.Format("{0}\t{1}\t{2}", ssidName, rssi.ToString(), DateTime.Now.ToString("o")));
                    }
                }
            }

        }
    }
}
