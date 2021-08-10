using IOTweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace IOTweb.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ContentResult JSON(int xStart = 0)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            var lstTemp = GetTempIndex();
            foreach (var temp in lstTemp)
            {
                var y = Convert.ToDouble(temp.Temperature);
                var x = Convert.ToDouble(temp.Id);
                dataPoints.Add(new DataPoint(xStart + x, y));
            }
            return Content(JsonConvert.SerializeObject(dataPoints, _jsonSetting), "application/json");
        }

        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };


        public List<Temp> GetTempIndex()
        {
            List<Temp> lstTemp = null;
            using (var dc = new DataClasses1DataContext())
            {
                lstTemp = dc.Temps.ToList();
            }
            return lstTemp;
        }

        public ActionResult GetTemp()
        {
            List<Temp> lstTemp = null;
            using (var dc = new DataClasses1DataContext())
            {
                lstTemp = dc.Temps.ToList();
            }
            return Json(lstTemp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetTemp(int Temperature)
        {
            var temp = new Temp();
            using (var dc = new DataClasses1DataContext())
            {
                temp.Time = DateTime.Now;
                if (Temperature < 100)
                {
                    temp.Temperature = Temperature;
                }
                dc.Temps.InsertOnSubmit(temp);
                dc.SubmitChanges();
            }
            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetTempRandom()
        {
            var temp = new Temp();
            using (var dc = new DataClasses1DataContext())
            {
                temp.Time = DateTime.Now;
                temp.Temperature = new Random().Next(0, 100);
                dc.Temps.InsertOnSubmit(temp);
                dc.SubmitChanges();
            }
            return Json(temp, JsonRequestBehavior.AllowGet);
        }

    }
}