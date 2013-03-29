using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDataProject.Models
{
    public class PassingChordDiagramModel
    {
        public PassingChordDiagramModel()
        {
            nameColorLst = new List<PlayerNameColor>();
        }

        public string playerPassingArray { get; set; }
        public List<PlayerNameColor> nameColorLst { get; set; }
    }

    public class PlayerNameColor
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
