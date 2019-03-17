using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class HomeController : Controller
    {
        private hcid_lookupEntities db = new hcid_lookupEntities();
        public ActionResult Index(string StreetAddress, string StreetNumber, string StreetName, string StreetDirection, string StreetSuffix, string City, string Zip, string APN, string bims2, string rent2, string hims2, string prop_site_address2)
        {
            // input for options in forms in views
            List<SelectListItem> stdirs = new List<SelectListItem>();
            stdirs.Add(new SelectListItem { Text = "Anything or others", Value = "" });
            stdirs.Add(new SelectListItem { Text = "East", Value = "e" });
            stdirs.Add(new SelectListItem { Text = "West", Value = "w" });
            stdirs.Add(new SelectListItem { Text = "North", Value = "n", Selected = true });
            stdirs.Add(new SelectListItem { Text = "South", Value = "s" });
            ViewBag.StreetDirection = stdirs;
            List<SelectListItem> stsuff = new List<SelectListItem>();
            stsuff.Add(new SelectListItem { Text = "Anything or others", Value = "0" });
            stsuff.Add(new SelectListItem { Text = "Street", Value = "1" });
            stsuff.Add(new SelectListItem { Text = "Avenue", Value = "2" });
            stsuff.Add(new SelectListItem { Text = "Boulevard", Value = "3", Selected = true });
            stsuff.Add(new SelectListItem { Text = "Highway", Value = "4" });
            stsuff.Add(new SelectListItem { Text = "Drive", Value = "5" });
            stsuff.Add(new SelectListItem { Text = "Place", Value = "6" });
            stsuff.Add(new SelectListItem { Text = "Lane", Value = "7" });
            stsuff.Add(new SelectListItem { Text = "Route", Value = "8" });
            stsuff.Add(new SelectListItem { Text = "Way", Value = "9" });
            ViewBag.StreetSuffix = stsuff;
            // prepare a list of variants of street type
            Dictionary<int, List<string>> StSuffAbbv = new Dictionary<int, List<string>>();
            StSuffAbbv.Add(0, new List<string> { "", "", "", "" });
            StSuffAbbv.Add(1, new List<string> { "st", "st", "st", "st" });
            StSuffAbbv.Add(2, new List<string> { "av", "av", "av", "av" });
            StSuffAbbv.Add(3, new List<string> { "blvd", "boul", "boul", "boul" });
            StSuffAbbv.Add(4, new List<string> { "high", "hwy", "hiwy", "way" });
            StSuffAbbv.Add(5, new List<string> { "dr", "dr", "dr", "dr" });
            StSuffAbbv.Add(6, new List<string> { "pl", "pl", "pl", "pl" });
            StSuffAbbv.Add(7, new List<string> { "lane", "ln", "ln", "ln" });
            StSuffAbbv.Add(8, new List<string> { "route", "rte", "rte", "rte" });
            StSuffAbbv.Add(9, new List<string> { "way", "wy", "wy", "wy" });
            //StreetSuffix has value of null when the page is first loaded. This causes the following code line to mess up.
            StreetSuffix = StreetSuffix == null ? "0" : StreetSuffix;
            int SSInt = int.Parse(StreetSuffix);
            string abbv1 = StSuffAbbv[SSInt][0];
            string abbv2 = StSuffAbbv[SSInt][1];
            string abbv3 = StSuffAbbv[SSInt][2];
            string abbv4 = StSuffAbbv[SSInt][3];
            List<bims2> bvm2 = new List<bims2>();
            List<hims2> hvm2 = new List<hims2>();
            List<rent2> rvm2 = new List<rent2>();
            List<prop_site_address2> pvm2 = new List<prop_site_address2>();
            if (bims2 != null)
            {
                bvm2 = (from bim in db.bims2
                        let APNString = SqlFunctions.StringConvert((double)bim.APN)
                        where (bim.Property_Address.Contains(abbv1) || bim.Property_Address.Contains(abbv2) || bim.Property_Address.Contains(abbv3) || bim.Property_Address.Contains(abbv4)) &&
                            (bim.Property_Address.Contains(StreetName) || StreetName == null) &&
                            (bim.Property_Address.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.Property_Address.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.Property_City_State_Zip.Contains(City) || City == null) &&
                            (bim.Property_City_State_Zip.Contains(Zip) || Zip == null) &&
                            (APNString.Contains(APN) || APN == null)
                        select bim).Take(10).ToList();
            };
            if (hims2 != null)
            {
                hvm2 = (from bim in db.hims2
                        let ZipCodeString = SqlFunctions.StringConvert((double)bim.ZipCode)
                        where (bim.StreetTypeCd.Contains(abbv1) || bim.StreetTypeCd.Contains(abbv2) || bim.StreetTypeCd.Contains(abbv3) || bim.StreetTypeCd.Contains(abbv4)) &&
                            (bim.StreetName.Contains(StreetName) || StreetName == null) &&
                            (bim.HouseNum.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.PreDirCd.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.City.Contains(City) || City == null || bim.City.Equals(null)) &&
                            (ZipCodeString.Contains(Zip) || Zip == null) &&
                            (bim.APN.Contains(APN) || APN == null)
                        select bim).Take(10).ToList();
            };
            if (rent2 != null)
            {
                rvm2 = (from bim in db.rent2
                        let APNString = SqlFunctions.StringConvert((double)bim.APN)
                        where (bim.Secondary_Address.Contains(abbv1) || bim.Secondary_Address.Contains(abbv2) || bim.Secondary_Address.Contains(abbv3) || bim.Secondary_Address.Contains(abbv4)) &&
                            (bim.Secondary_Address.Contains(StreetName) || StreetName == null) &&
                            (bim.Secondary_Address.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.Secondary_Address.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.Secondary_Address.Contains(City) || City == null) &&
                            (bim.Secondary_Address.Contains(Zip) || Zip == null) &&
                            (APNString.Contains(APN) || APN == null)
                        select bim).Take(10).ToList();
            };
            if (prop_site_address2 != null)
            {
                pvm2 = (from bim in db.prop_site_address2
                        let APNString = SqlFunctions.StringConvert((double)bim.Apn)
                        let HouseNumString = SqlFunctions.StringConvert((double)bim.HouseNum)
                        let ZipCodeString = SqlFunctions.StringConvert((double)bim.Zip)
                        where (bim.StreetTypeCd.Contains(abbv1) || bim.StreetTypeCd.Contains(abbv2) || bim.StreetTypeCd.Contains(abbv3) || bim.StreetTypeCd.Contains(abbv4)) &&
                            (bim.StreetName.Contains(StreetName) || StreetName == null) &&
                            (HouseNumString.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.PreDirCd.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.City.Contains(City) || City == null || bim.City.Equals(null)) &&
                            (ZipCodeString.Contains(Zip) || Zip == null) &&
                            (APNString.Contains(APN) || APN == null)
                        select bim).Take(10).ToList();
            }
            //var data = new { CountBim = bvm2.Count(), DataBim = bvm2, CountHim = hvm2.Count(), DataHim = hvm2, CountRent = rvm2.Count(), DataRent = rvm2, CountPropSite = pvm2.Count(), DataPropSite = pvm2 };
            ModelTotal finalItem = new ModelTotal();
            finalItem.Bims2List = bvm2;
            finalItem.Rent2List = rvm2;
            return View(finalItem);
        }
        public ActionResult AboutPart()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}