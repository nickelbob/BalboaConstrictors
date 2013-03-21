using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using GameDataProject.Models;

namespace GameDataProject.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportKML()
        {
            ImportKMLModel model = new ImportKMLModel();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult ImportKML(ImportKMLModel model)
        {
            // Verify that the user selected a file
            if (model.file != null && model.file.ContentLength > 0)
            {
                // extract only the filename
                var fileName = Path.GetFileName(model.file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                model.file.SaveAs(path);

                ImportKMLFile(path);
            }

            return View(model);
        }

        private void ImportKMLFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                XDocument feedXml = XDocument.Load(fs);
                var feeds = feedXml.Descendants("Placemark");

                feeds = feeds.Where(x => x.HasAttributes);

                var feedList = feedXml.Document.Descendants().ToList();
                var Placemark = feedList.Where(x => x.Name.LocalName.Contains("Placemark") && x.HasAttributes);

                
            }
        }
    }
}
