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
        static bool useDatabase = true;
        static string filename = String.Empty;

        static void Main(string[] args)
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

                filename = cmdline[cmdline.IndexOf("-f") + 1];
            }
            //entity = new RSSIRecorderEntities();
            client = new WlanClient();
            Console.WriteLine("Main thread: starting a timer");

            while (true)
            {
                Timer t = new Timer(getRSSI, 5, 0, 2000);
                Console.WriteLine("Main thread: Doing other work here...");
                Thread.Sleep(10000); // Simulating other work (10 seconds)
                t.Dispose(); // Cancel the timer now
            }
        }

        private static void getRSSI(Object state)
        {
            RSSIRecorderEntities entity = new RSSIRecorderEntities();

            Collection<String> connectedSsids = new Collection<string>();

            foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
            {
                Wlan.Dot11Ssid ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                string ssidName = new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength));
                connectedSsids.Add(ssidName);

                if (useDatabase)
                {
                    entity.RSSIRecords.Add(new RSSIRecord()
                    {
                        CreatedOnUtc = DateTime.Now,
                        SignalStrength = wlanInterface.RSSI,
                        SSID = ssidName
                    });

                    entity.SaveChanges();
                }
                else
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true))
                    {
                        file.WriteLine(String.Format("{0}\t{1}\t{2}", ssidName, wlanInterface.RSSI.ToString(), DateTime.Now.ToString("o")));
                    }  
                }
            }

            
        }
    }
}
