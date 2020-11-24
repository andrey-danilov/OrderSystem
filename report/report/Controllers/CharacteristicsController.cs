using Microsoft.AspNet.Identity;
using PagedList;
using report.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace report.Controllers
{
    public class CharacteristicsController : Controller
    {
        // GET: Characteristics
        public ActionResult Index(string shop, int? page)
        {
            XmlDocument doc = new XmlDocument();
            List<MatchList> CurrentMatchList = new List<MatchList>();

            if (Session["MatchListFindReq"] != null) CurrentMatchList = Session["MatchListFindReq"] as List<MatchList>;
            else
            {
                if (Session["MatchList"] != null) CurrentMatchList = Session["MatchList"] as List<MatchList>;
                else
                {
                    var userId = User.Identity.GetUserId();
                    DirectoryInfo path = new DirectoryInfo(Server.MapPath("/") + "/Users/" + userId + "/" + shop);
                    doc.Load(path.FullName + @"\MatchList.xml");
                    foreach (XmlNode ML in doc.SelectNodes("/matchlist/match"))
                    {
                        CurrentMatchList.Add(new MatchList(
                                                            CurrentMatchList.Count.ToString(),
                                                            ML.SelectSingleNode(".//shopname").InnerText,
                                                            ML.SelectSingleNode(".//ourname").InnerText,
                                                            ML.SelectSingleNode(".//shopvalue").InnerText,
                                                            ML.SelectSingleNode(".//ourvalue").InnerText
                                                            ));                        
                    }
                    Session["MatchList"] = CurrentMatchList;
                    Session["ShopName"] = shop;
                }
            }
            if (Session["FilterMatchList"] != null) ViewBag.Filter = Session["FilterMatchList"] as AutoCorrect;
            else ViewBag.Filter = new MatchList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(CurrentMatchList.ToPagedList(pageNumber, pageSize));
        }


        [HttpPost]
        public ActionResult Index(MatchList ob, int? page)
        {
            List<MatchList> CurrentMatchList = new List<MatchList>();
            CurrentMatchList = Session["MatchList"] as List<MatchList>;

            List<MatchList> req = Session["MatchList"] as List<MatchList>;

            if (!String.IsNullOrEmpty(ob.ourname)) req = req.Where(x => x.ourname.Contains(ob.ourname)).ToList();
            if (!String.IsNullOrEmpty(ob.ourvalue)) req = req.Where(x => x.ourvalue.Contains(ob.ourvalue)).ToList();
            if (!String.IsNullOrEmpty(ob.shopname)) req = req.Where(x => x.shopname.Contains(ob.shopname)).ToList();
            if (!String.IsNullOrEmpty(ob.shopvalue)) req = req.Where(x => x.shopvalue.Contains(ob.shopvalue)).ToList();
            Session["MatchListFindReq"] = req;
            Session["FilterMatchList"] = ob;
            ViewBag.Filter = ob;
            int pageSize = 1;
            int pageNumber = (page ?? 1);
            return View("Index", req.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult SaveChange(MatchList ob)
        {
            List<MatchList> CurrentMatchList = new List<MatchList>();
            CurrentMatchList = Session["MatchList"] as List<MatchList>;
            CurrentMatchList[Convert.ToInt32(ob.id)] = ob;
            Session["MatchList"] = CurrentMatchList;
            return PartialView("MatchListDetails", ob);
        }

        public ActionResult Delete(string id)
        {
            List<MatchList> CurrentMatchList = new List<MatchList>();
            CurrentMatchList = Session["MatchList"] as List<MatchList>;
            CurrentMatchList.Remove(CurrentMatchList[Convert.ToInt32(id)]);
            if (Session["FilterMatchList"] != null) ViewBag.Filter = Session["FilterMatchList"] as MatchList;
            else ViewBag.Filter = new MatchList();
            Session["MatchList"] = CurrentMatchList;
            return View("Index", CurrentMatchList.ToPagedList(1, 10));
        }

        public ActionResult AddItem(MatchList ob)
        {
            List<MatchList> CurrentMatchList = new List<MatchList>();
            CurrentMatchList = Session["MatchList"] as List<MatchList>;
            CurrentMatchList.Add(ob);
            Session["MatchList"] = CurrentMatchList;
            if (Session["FilterMatchList"] != null) ViewBag.Filter = Session["FilterMatchList"] as MatchList;
            else ViewBag.Filter = new MatchList();
            return View("Index", CurrentMatchList);
        }



        public ActionResult SaveToFile()
        {
            List<MatchList> CurrentMatchList = new List<MatchList>();
            CurrentMatchList = Session["MatchList"] as List<MatchList>;

            var userId = User.Identity.GetUserId();


            DirectoryInfo user = new DirectoryInfo(Server.MapPath("/") + @"\Users\" + userId);
            FileInfo Konfig = new FileInfo(user.FullName + @"\" + Session["ShopName"] as string + @"\MatchList.xml");
            string MatchList = "<matchlist>\r\n";
            
            ////foreach (MatchList ML in CurrentMatchList)
            ////{
            ////    MatchList += String.Format("<match>\r\n") +
            ////                  String.Format("<shopname>{0}</shopname>\r\n", ML.shopname) +
            ////                  String.Format("<ourname>{0}</ourname>\r\n", ML.ourname) +
            ////                  String.Format("<shopvalue>{0}</shopvalue>\r\n", ML.shopvalue) +
            ////                  String.Format("<ourvalue>{0}</ourvalue>\r\n</match>\r\n", ML.ourvalue);
            ////}
            ////MatchList = "";
            ////for (int i = 0; i < CurrentMatchList.Count; i++)
            ////{
                
            ////    MatchList += String.Format("<match>\r\n<shopname>{0}</shopname>\r\n<ourname>{1}</ourname>\r\n<shopvalue>{2}</shopvalue>\r\n<ourvalue>{3}</ourvalue>\r\n</match>\r\n", CurrentMatchList[i].shopname, CurrentMatchList[i].ourname, CurrentMatchList[i].shopvalue, CurrentMatchList[i].ourvalue);           
            ////}


            
            if (Konfig.Exists != false)
            {

                FileStream fs = Konfig.Create();
                Byte[] first = new UTF8Encoding(true).GetBytes("<matchlist>\r\n");
                fs.Write(first, 0, first.Length);
                for (int i = 0; i < CurrentMatchList.Count; i++)
                {                    
                    MatchList = String.Format("<match>\r\n<shopname>{0}</shopname>\r\n<ourname>{1}</ourname>\r\n<shopvalue>{2}</shopvalue>\r\n<ourvalue>{3}</ourvalue>\r\n</match>\r\n", CurrentMatchList[i].shopname, CurrentMatchList[i].ourname, CurrentMatchList[i].shopvalue, CurrentMatchList[i].ourvalue);
                    Byte[] info = new UTF8Encoding(true).GetBytes(MatchList);
                    fs.Write(info, 0, info.Length);
                }
                Byte[] last = new UTF8Encoding(true).GetBytes("</matchlist>\r\n");
                fs.Write(last, 0, last.Length);

                fs.Close();
            }
            return PartialView("request", "Файл сохранен");
        }


    }
}