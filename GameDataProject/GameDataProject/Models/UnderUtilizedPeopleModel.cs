using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDataProject.Models
{
    public class UnderUtilizedPeopleModel
    {
        public UnderUtilizedPeopleModel()
        {
            underUtilGameData = new Dictionary<string, Dictionary<string, UnderUtilGameData>>();
        }

        public Dictionary<string, Dictionary<string, UnderUtilGameData>> underUtilGameData { get; set; }
    }

    public class UnderUtilGameData
    {
        public UnderUtilGameData()
        {
            Names = new List<PlayerInfo>();
            Passes = new List<PassInfo>();
        }

        public List<PlayerInfo> Names { get; set; }
        public List<PassInfo> Passes { get; set; }
    }

    public class PlayerInfo
    {
        public string Name { get; set; }
        public float compRate { get; set; }
        public int numThrowsCatches { get; set; }
    }

    public class PassInfo
    {
        public int numThrows { get; set; }
        public float throwDist { get; set; }
    }
}

