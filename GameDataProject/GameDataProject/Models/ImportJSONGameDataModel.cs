using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameDataProject.Models
{
    public class ImportJSONGameDataModel
    {
        public ImportJSONGameDataModel()
        {
            games = new List<Game>();
            gameSelect = new List<SelectListItem>();
        }


        public bool importJSON { get; set; }
        public List<Game> games { get; set; }
        public List<SelectListItem> gameSelect { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string Status { get; set; }
    }
}