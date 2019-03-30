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
        private testdatabaseBim dbBim = new testdatabaseBim();
        private testdatabaseProp dbProp = new testdatabaseProp();
        private testdatabaseRent dbRent = new testdatabaseRent();
        private testdatabaseHim dbHim = new testdatabaseHim();

        public ActionResult Index()
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
            return View();
        }
        public JsonResult GetSearchResult(string StreetNumber, string StreetName, string StreetDirection, string StreetSuffix, string City, string Zip, string APN, string bims2cb, string rent2cb, string hims2cb, string prop_site_address2cb)
        {
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

            //Number of records on a page;
            int pageSize = 10;
            int setSize = 20;//number of page displayed each set
            int bimcount, himcount, rentcount, propcount;
            // Number of record
            bimcount = himcount = rentcount = propcount = 0;

            //Number of page
            int bimnp, himnp, rentnp, propnp;
            bimnp = himnp = rentnp = propnp = 1;
            //Number of set, each set has 20 pages
            int bimns, himns, rentns, propns;
            bimns = himns = rentns = propns = 1;

            List<bims2> bvm2 = new List<bims2>();
            List<bims2> bvm3 = new List<bims2>();
            List<hims2> hvm2 = new List<hims2>();
            List<rent2> rvm2 = new List<rent2>();
            List<prop_site_address2> pvm2 = new List<prop_site_address2>();
            if (bims2cb != "")
            {
                bimcount = (from bim in dbBim.bims2
                        let APNString = SqlFunctions.StringConvert((double)bim.APN)
                        where (bim.Property_Address.Contains(abbv1) || bim.Property_Address.Contains(abbv2) || bim.Property_Address.Contains(abbv3) || bim.Property_Address.Contains(abbv4)) &&
                            (bim.Property_Address.Contains(StreetName) || StreetName == null) &&
                            (bim.Property_Address.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.Property_Address.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.Property_City_State_Zip.Contains(City) || City == null) &&
                            (bim.Property_City_State_Zip.Contains(Zip) || Zip == null) &&
                            (APNString.Contains(APN) || APN == null)
                        select bim).ToList().Count();
                bimnp = Convert.ToInt32(Math.Ceiling((double)bimcount / pageSize));
                bimns = Convert.ToInt32(Math.Ceiling((double)bimnp / setSize));
                bvm2 = (from bim in dbBim.bims2
                    let APNString = SqlFunctions.StringConvert((double)bim.APN)
                    where (bim.Property_Address.Contains(abbv1) || bim.Property_Address.Contains(abbv2) || bim.Property_Address.Contains(abbv3) || bim.Property_Address.Contains(abbv4)) &&
                        (bim.Property_Address.Contains(StreetName) || StreetName == null) &&
                        (bim.Property_Address.Contains(StreetNumber) || StreetNumber == null) &&
                        (bim.Property_Address.Contains(StreetDirection) || StreetDirection == null) &&
                        (bim.Property_City_State_Zip.Contains(City) || City == null) &&
                        (bim.Property_City_State_Zip.Contains(Zip) || Zip == null) &&
                        (APNString.Contains(APN) || APN == null)
                    orderby (bim.id)
                    select bim).Take(pageSize).ToList();
            };
            if (hims2cb != "")
            {
                himcount = (from bim in dbHim.hims2
                        let ZipCodeString = SqlFunctions.StringConvert((double)bim.ZipCode)
                        where (bim.StreetTypeCd.Contains(abbv1) || bim.StreetTypeCd.Contains(abbv2) || bim.StreetTypeCd.Contains(abbv3) || bim.StreetTypeCd.Contains(abbv4)) &&
                            (bim.StreetName.Contains(StreetName) || StreetName == null) &&
                            (bim.HouseNum.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.PreDirCd.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.City.Contains(City) || City == null || bim.City.Equals(null)|| bim.City.Equals("")) &&
                            (ZipCodeString.Contains(Zip) || Zip == null) &&
                            (bim.APN.Contains(APN) || APN == null)
                        select bim).ToList().Count();
                himnp = Convert.ToInt32(Math.Ceiling((double)himcount / pageSize));
                himns = Convert.ToInt32(Math.Ceiling((double)himnp / setSize));
                hvm2 = (from bim in dbHim.hims2
                        let ZipCodeString = SqlFunctions.StringConvert((double)bim.ZipCode)
                        where (bim.StreetTypeCd.Contains(abbv1) || bim.StreetTypeCd.Contains(abbv2) || bim.StreetTypeCd.Contains(abbv3) || bim.StreetTypeCd.Contains(abbv4)) &&
                            (bim.StreetName.Contains(StreetName) || StreetName == null) &&
                            (bim.HouseNum.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.PreDirCd.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.City.Contains(City) || City == null || bim.City.Equals(null) || bim.City.Equals("")) &&
                            (ZipCodeString.Contains(Zip) || Zip == null) &&
                            (bim.APN.Contains(APN) || APN == null)
                        orderby (bim.id)
                        select bim).Take(pageSize).ToList();
            };
            if (rent2cb != "")
            {
                rentcount = (from bim in dbRent.rent2
                        let APNString = SqlFunctions.StringConvert((double)bim.APN)
                        where (bim.Secondary_Address.Contains(abbv1) || bim.Secondary_Address.Contains(abbv2) || bim.Secondary_Address.Contains(abbv3) || bim.Secondary_Address.Contains(abbv4)) &&
                            (bim.Secondary_Address.Contains(StreetName) || StreetName == null) &&
                            (bim.Secondary_Address.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.Secondary_Address.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.Secondary_Address.Contains(City) || City == null) &&
                            (bim.Secondary_Address.Contains(Zip) || Zip == null) &&
                            (APNString.Contains(APN) || APN == null)
                        select bim).ToList().Count();
                rentnp = Convert.ToInt32(Math.Ceiling((double)rentcount / pageSize));
                rentns = Convert.ToInt32(Math.Ceiling((double)rentnp / setSize));
                rvm2 = (from bim in dbRent.rent2
                    let APNString = SqlFunctions.StringConvert((double)bim.APN)
                    where (bim.Secondary_Address.Contains(abbv1) || bim.Secondary_Address.Contains(abbv2) || bim.Secondary_Address.Contains(abbv3) || bim.Secondary_Address.Contains(abbv4)) &&
                        (bim.Secondary_Address.Contains(StreetName) || StreetName == null) &&
                        (bim.Secondary_Address.Contains(StreetNumber) || StreetNumber == null) &&
                        (bim.Secondary_Address.Contains(StreetDirection) || StreetDirection == null) &&
                        (bim.Secondary_Address.Contains(City) || City == null) &&
                        (bim.Secondary_Address.Contains(Zip) || Zip == null) &&
                        (APNString.Contains(APN) || APN == null)
                    orderby (bim.id)
                    select bim).Take(pageSize).ToList();
            };
            if (prop_site_address2cb != "")
            {
                propcount = (from bim in dbProp.prop_site_address2
                        let APNString = SqlFunctions.StringConvert((double)bim.Apn)
                        let HouseNumString = SqlFunctions.StringConvert((double)bim.HouseNum)
                        let ZipCodeString = SqlFunctions.StringConvert((double)bim.Zip)
                        where (bim.StreetTypeCd.Contains(abbv1) || bim.StreetTypeCd.Contains(abbv2) || bim.StreetTypeCd.Contains(abbv3) || bim.StreetTypeCd.Contains(abbv4)) &&
                            (bim.StreetName.Contains(StreetName) || StreetName == null) &&
                            (HouseNumString.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.PreDirCd.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.City.Contains(City) || City == null || bim.City.Equals(null)|| bim.City.Equals("")) &&
                            (ZipCodeString.Contains(Zip) || Zip == null) &&
                            (APNString.Contains(APN) || APN == null)
                        select bim).ToList().Count();
                propnp = Convert.ToInt32(Math.Ceiling((double)propcount / pageSize));
                propns = Convert.ToInt32(Math.Ceiling((double)propnp / setSize));
                pvm2 = (from bim in dbProp.prop_site_address2
                        let APNString = SqlFunctions.StringConvert((double)bim.Apn)
                        let HouseNumString = SqlFunctions.StringConvert((double)bim.HouseNum)
                        let ZipCodeString = SqlFunctions.StringConvert((double)bim.Zip)
                        where (bim.StreetTypeCd.Contains(abbv1) || bim.StreetTypeCd.Contains(abbv2) || bim.StreetTypeCd.Contains(abbv3) || bim.StreetTypeCd.Contains(abbv4)) &&
                            (bim.StreetName.Contains(StreetName) || StreetName == null) &&
                            (HouseNumString.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.PreDirCd.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.City.Contains(City) || City == null || bim.City.Equals(null) || bim.City.Equals("")) &&
                            (ZipCodeString.Contains(Zip) || Zip == null) &&
                            (APNString.Contains(APN) || APN == null)
                        orderby (bim.id)
                        select bim).Take(pageSize).ToList();
            }
            var data = new
            {
                PageSize = pageSize,
                SetSize = setSize,
                SearchBim = bims2cb,
                SearchHim = hims2cb,
                SearchRent = rent2cb,
                SearchProp = prop_site_address2cb,
                CountBim = bimcount,
                DataBim = bvm2,
                NoofPageBim = bimnp,
                CurrentPageBim = 1,
                CurrentSetBim = 1,
                NoofSetBim = bimns,
                CountHim = himcount,
                DataHim = hvm2,
                NoofPageHim = himnp,
                CurrentPageHim = 1,
                CurrentSetHim = 1,
                NoofSetHim = himns,
                CountRent = rentcount,
                DataRent = rvm2,
                NoofPageRent = rentnp,
                CurrentPageRent = 1,
                CurrentSetRent = 1,
                NoofSetRent = rentns,
                CountProp = propcount,
                DataProp = pvm2,
                NoofPageProp = propnp,
                CurrentPageProp = 1,
                CurrentSetProp = 1,
                NoofSetProp = propns
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //Get paged data from bim table
        public JsonResult GetBimSearchResult(string StreetNumber, string StreetName, string StreetDirection, string StreetSuffix, string City, string Zip, string APN, string PageNum, string CurrentSet)
        {
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
            int SSInt = int.Parse(StreetSuffix);
            string abbv1 = StSuffAbbv[SSInt][0];
            string abbv2 = StSuffAbbv[SSInt][1];
            string abbv3 = StSuffAbbv[SSInt][2];
            string abbv4 = StSuffAbbv[SSInt][3];
            //Number of records on a page;
            int pageSize = 10;
            int setSize = 20;//number of page displayed each set
            int bimcount, himcount, rentcount, propcount;
            // Number of record
            bimcount = himcount = rentcount = propcount = 0;

            //Number of page
            int bimnp, himnp, rentnp, propnp;
            bimnp = himnp = rentnp = propnp = 1;
            //Number of set, each set has 20 pages
            int bimns, himns, rentns, propns;
            bimns = himns = rentns = propns = 1;

            List<bims2> bvm2 = new List<bims2>();
                bvm2 = (from bim in dbBim.bims2
                        let APNString = SqlFunctions.StringConvert((double)bim.APN)
                        where (bim.Property_Address.Contains(abbv1) || bim.Property_Address.Contains(abbv2) || bim.Property_Address.Contains(abbv3) || bim.Property_Address.Contains(abbv4)) &&
                            (bim.Property_Address.Contains(StreetName) || StreetName == null) &&
                            (bim.Property_Address.Contains(StreetNumber) || StreetNumber == null) &&
                            (bim.Property_Address.Contains(StreetDirection) || StreetDirection == null) &&
                            (bim.Property_City_State_Zip.Contains(City) || City == null) &&
                            (bim.Property_City_State_Zip.Contains(Zip) || Zip == null) &&
                            (APNString.Contains(APN) || APN == null)
                        orderby (bim.id)
                        select bim).Skip((Convert.ToInt32(PageNum) - 1)*pageSize).Take(pageSize).ToList();
            var data = new
            {
                CurrentSetBim = CurrentSet,
                DataBim = bvm2,
                CurrentPageBim = PageNum
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //Get paged data from him table
        public JsonResult GetHimSearchResult(string StreetNumber, string StreetName, string StreetDirection, string StreetSuffix, string City, string Zip, string APN, string PageNum, string CurrentSet)
        {
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
            int SSInt = int.Parse(StreetSuffix);
            string abbv1 = StSuffAbbv[SSInt][0];
            string abbv2 = StSuffAbbv[SSInt][1];
            string abbv3 = StSuffAbbv[SSInt][2];
            string abbv4 = StSuffAbbv[SSInt][3];

            //Number of records on a page;
            int pageSize = 10;
            int setSize = 20;//number of page displayed each set
            int bimcount, himcount, rentcount, propcount;
            // Number of record
            bimcount = himcount = rentcount = propcount = 0;

            //Number of page
            int bimnp, himnp, rentnp, propnp;
            bimnp = himnp = rentnp = propnp = 1;
            //Number of set, each set has 20 pages
            int bimns, himns, rentns, propns;
            bimns = himns = rentns = propns = 1;

            List<hims2> hvm2 = new List<hims2>();
            hvm2 = (from bim in dbHim.hims2
                    let ZipCodeString = SqlFunctions.StringConvert((double)bim.ZipCode)
                    where (bim.StreetTypeCd.Contains(abbv1) || bim.StreetTypeCd.Contains(abbv2) || bim.StreetTypeCd.Contains(abbv3) || bim.StreetTypeCd.Contains(abbv4)) &&
                        (bim.StreetName.Contains(StreetName) || StreetName == null) &&
                        (bim.HouseNum.Contains(StreetNumber) || StreetNumber == null) &&
                        (bim.PreDirCd.Contains(StreetDirection) || StreetDirection == null) &&
                        (bim.City.Contains(City) || City == null || bim.City.Equals(null)|| bim.City.Equals("")) &&
                        (ZipCodeString.Contains(Zip) || Zip == null) &&
                        (bim.APN.Contains(APN) || APN == null)
                    orderby (bim.id)
                    select bim).Skip((Convert.ToInt32(PageNum) - 1) * pageSize).Take(pageSize).ToList();
            var data = new
            {
                DataHim = hvm2,
                CurrentPageHim = PageNum,
                CurrentSetHim = CurrentSet
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //Get paged data from rent table
        public JsonResult GetRentSearchResult(string StreetNumber, string StreetName, string StreetDirection, string StreetSuffix, string City, string Zip, string APN, string PageNum, string CurrentSet)
        {
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
            int SSInt = int.Parse(StreetSuffix);
            string abbv1 = StSuffAbbv[SSInt][0];
            string abbv2 = StSuffAbbv[SSInt][1];
            string abbv3 = StSuffAbbv[SSInt][2];
            string abbv4 = StSuffAbbv[SSInt][3];
            //Number of records on a page;
            int pageSize = 10;
            int setSize = 20;//number of page displayed each set
            int bimcount, himcount, rentcount, propcount;
            // Number of record
            bimcount = himcount = rentcount = propcount = 0;

            //Number of page
            int bimnp, himnp, rentnp, propnp;
            bimnp = himnp = rentnp = propnp = 1;
            //Number of set, each set has 20 pages
            int bimns, himns, rentns, propns;
            bimns = himns = rentns = propns = 1;

            List<rent2> rvm2 = new List<rent2>();
            rvm2 = (from bim in dbRent.rent2
                    let APNString = SqlFunctions.StringConvert((double)bim.APN)
                    where (bim.Secondary_Address.Contains(abbv1) || bim.Secondary_Address.Contains(abbv2) || bim.Secondary_Address.Contains(abbv3) || bim.Secondary_Address.Contains(abbv4)) &&
                        (bim.Secondary_Address.Contains(StreetName) || StreetName == null) &&
                        (bim.Secondary_Address.Contains(StreetNumber) || StreetNumber == null) &&
                        (bim.Secondary_Address.Contains(StreetDirection) || StreetDirection == null) &&
                        (bim.Secondary_Address.Contains(City) || City == null) &&
                        (bim.Secondary_Address.Contains(Zip) || Zip == null) &&
                        (APNString.Contains(APN) || APN == null)
                    orderby (bim.id)
                    select bim).Skip((Convert.ToInt32(PageNum) - 1) * pageSize).Take(pageSize).ToList();
            var data = new
            {
                DataRent = rvm2,
                CurrentPageRent = PageNum,
                CurrentSetRent = CurrentSet
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //Get paged data from Prop table
        public JsonResult GetPropSearchResult(string StreetNumber, string StreetName, string StreetDirection, string StreetSuffix, string City, string Zip, string APN, string PageNum, string CurrentSet)
        {
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
            int SSInt = int.Parse(StreetSuffix);
            string abbv1 = StSuffAbbv[SSInt][0];
            string abbv2 = StSuffAbbv[SSInt][1];
            string abbv3 = StSuffAbbv[SSInt][2];
            string abbv4 = StSuffAbbv[SSInt][3];
            //Number of records on a page;
            int pageSize = 10;
            int setSize = 20;//number of page displayed each set
            int bimcount, himcount, rentcount, propcount;
            // Number of record
            bimcount = himcount = rentcount = propcount = 0;

            //Number of page
            int bimnp, himnp, rentnp, propnp;
            bimnp = himnp = rentnp = propnp = 1;
            //Number of set, each set has 20 pages
            int bimns, himns, rentns, propns;
            bimns = himns = rentns = propns = 1;

            List<prop_site_address2> pvm2 = new List<prop_site_address2>();
            pvm2 = (from bim in dbProp.prop_site_address2
                    let APNString = SqlFunctions.StringConvert((double)bim.Apn)
                    let HouseNumString = SqlFunctions.StringConvert((double)bim.HouseNum)
                    let ZipCodeString = SqlFunctions.StringConvert((double)bim.Zip)
                    where (bim.StreetTypeCd.Contains(abbv1) || bim.StreetTypeCd.Contains(abbv2) || bim.StreetTypeCd.Contains(abbv3) || bim.StreetTypeCd.Contains(abbv4)) &&
                        (bim.StreetName.Contains(StreetName) || StreetName == null) &&
                        (HouseNumString.Contains(StreetNumber) || StreetNumber == null) &&
                        (bim.PreDirCd.Contains(StreetDirection) || StreetDirection == null) &&
                        (bim.City.Contains(City) || City == null || bim.City.Equals(null)|| bim.City.Equals("")) &&
                        (ZipCodeString.Contains(Zip) || Zip == null) &&
                        (APNString.Contains(APN) || APN == null)
                    orderby (bim.id)
                    select bim).Skip((Convert.ToInt32(PageNum) - 1) * pageSize).Take(pageSize).ToList();
            var data = new
            {
                DataProp = pvm2,
                CurrentPageProp = PageNum,
                CurrentSetProp = CurrentSet
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AboutPart()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}