using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameDataProject.Models
{
    public class ImportKMLModel
    {
        public ImportKMLModel()
        {
            players = new List<Player>();
            playersSelect = new List<SelectListItem>();
            dataTypeSelect = new List<SelectListItem>();
            dataTypes = new List<DataType>();
        }

        public HttpPostedFileBase file { get; set; }
        public List<Player> players { get; set; }
        public List<SelectListItem> playersSelect { get; set; }
        public List<DataType> dataTypes { get; set; }
        public List<SelectListItem> dataTypeSelect { get; set; }
        public string Status{get;set;}
    }
}