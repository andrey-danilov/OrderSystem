using CsQuery;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DBPars
{
    class Program
    {
        

        public static void Main(string[] args)
        {
            //string id = args[0];
            //string userId = args[1];
            //string path = args[2];

            string id = "45e02761-1453-4f60-a43d-19f32c5d9f9f";
            string userId = "aae2ed54-b1c2-496d-b503-64e1b56f4156";
            string path = @"W:\Users\Andrey\Desktop\report\report";
            string domen = "";
            Konfig CurrentKonfig = new Konfig();
            List<ProductPars> Products = new List<ProductPars>();
            List<AutoCorrect> AU = null;
            List<ShopCategories> ShC = null;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                CurrentKonfig = db.KonfigVe.Where(x => x.Id.Equals(id))
                   .Include(x => x.AdditionalPictures)
                   .Include(x => x.Denominations)
                   .Include(x => x.Descriptions)
                   .Include(x => x.Excepts)
                   .Include(x => x.Pagination)
                   .Include(x => x.ProductUrl)
                   .Include(x => x.ProductUrl.Availability)
                   .Include(x => x.ProductUrl.CheakPrise)
                   .Include(x => x.PriceLow)
                   .Include(x => x.PriceHigh)
                   .Include(x => x.MainPicture)
                   .Include(x => x.Availability)
                   .Include(x => x.Vendor)
                   .Include(x => x.Categores.Select(y => y.SubCatecories))
                   .First();
                CurrentKonfig.MainCharacteristics = db.Characteristics.Where(x => x.Konfig.Id.Equals(CurrentKonfig.Id) & x.SubCatecory == null).Include(x => x.Exceptions).ToList();

                foreach (var SCat in CurrentKonfig.Categores)
                {
                    foreach (var SC in SCat.SubCatecories)
                    {
                        foreach (var Ch in SC.Characteristics)
                        {
                            foreach (var Ex in Ch.Exceptions) { }
                        }
                    }
                }

                foreach (var p in db.ProductsPars.Where(x => x.ShopCode.Contains(CurrentKonfig.Id)))
                {
                    if (p.Relevance)
                    {
                        p.Relevance = false;

                    }

                }
                db.SaveChanges();
                Products = db.ProductsPars.Where(x=> x.ShopCode.Contains(CurrentKonfig.Id)).Include(x => x.Pictures).Include(x => x.Parameters).ToList();
                AU =  db.AutoCorrectList.Where(x => x.UserId.Equals(userId) & x.ShopName.Equals(CurrentKonfig.ShopName)).ToList();
                ShC = db.ShopCategories.Where(x => x.UserId.Equals(userId)).ToList();
            }
            for (int i = 0; i < Products.Count; i++)
            {
                Products[i].Relevance = false;
                
            }
            Console.Title = CurrentKonfig.ShopName;
            
            


            if (!String.IsNullOrEmpty(CurrentKonfig.password) & !String.IsNullOrEmpty(CurrentKonfig.login) & CurrentKonfig.selenium)
            {
                List<ProductParsListN> Product_links = new List<ProductParsListN>();
                  
                IWebDriver Dr;
                Console.Clear();
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments(new List<string>() { "headless" });
                Dr = new ChromeDriver(@"W:\pjs", chromeOptions);
                Dr.Navigate().GoToUrl(@"https://niala.com.ua");
                Thread.Sleep(2000);
                Dr.FindElement(By.CssSelector("#sideloginbox > form > div > div:nth-child(1) > input")).SendKeys("tennisballs@yandex.ua");
                Thread.Sleep(1000);
                Dr.FindElement(By.CssSelector("#sideloginbox > form > div > div:nth-child(2) > input")).SendKeys("aq1sw2de3fr4");
                Thread.Sleep(1000);
                Dr.FindElement(By.CssSelector("div.span6.login > input")).Click();
                var aaaa = Dr.PageSource;
                CQ cq = null;
                for (int i = 0; i < CurrentKonfig.Categores.Count; i++)
                {
                    cq = null;
                    Category C = CurrentKonfig.Categores[i];
                    List<Dictionary<string, List<string>>> CatUrsl = new List<Dictionary<string, List<string>>>();

                    if (CurrentKonfig.selenium)
                    {
                        try
                        {
                            //Dr.Navigate().GoToUrl(C.url);
                            Dr.Url = C.url;
                            Thread.Sleep(1000);
                            cq = CQ.Create(Dr.PageSource);
                        }
                        catch
                        {
                            
                            continue;
                        }
                    }
                    else
                    {
                        bool triger = true;
                        int c = 1;
                        while (triger)
                        {
                            try
                            {
                                cq = CQ.CreateFromUrl(C.url);
                                break;
                            }
                            catch
                            {
                                c++;
                                if (c > 3)
                                {
                                    
                                    triger = false;
                                    break;
                                }

                            }
                        }
                        if (!triger) continue;
                    }
                    /*загрузка страницы*/

                    List<string> pagUrls_done = new List<string>();
                    List<string> pagUrls = new List<string>();
                    List<CsQuery.IDomObject> urls = new List<CsQuery.IDomObject>();
                    while (true)
                    {
                        var Purls = CQ.Create();
                        Purls = cq.Find(CurrentKonfig.Pagination.value);

                        if (Purls.Length>0)
                        {
                            foreach (var CU in Purls)
                            {
                                if (!pagUrls.Contains(CU.GetAttribute("href")) & !pagUrls_done.Contains(CU.GetAttribute("href"))) pagUrls.Add(CU.GetAttribute("href"));
                            }
                        }


                        foreach (var a in cq.Find("div.table-responsive>table>tbody>tr"))
                        {
                            urls.Add(a);
                        }


                        if (pagUrls.Count == 0) break;
                        if (pagUrls.Count != 0)
                        {
                            string TU = "";
                            if (String.IsNullOrEmpty(pagUrls.First()))
                            {
                                foreach (string pp in pagUrls)
                                {
                                    if (!String.IsNullOrEmpty(pagUrls.First())) TU = pp;
                                }
                            }
                            else TU = pagUrls.First();
                            if (String.IsNullOrEmpty(TU))
                            {
                                break;
                            }

                            try
                            {
                                Dr.Navigate().GoToUrl(TU);
                            }
                            catch
                            {
                                Thread.Sleep(5000);
                                try
                                {
                                    Dr.Navigate().GoToUrl(TU);
                                }
                                catch
                                {
                                    continue;
                                }
                            }

                            try
                            {
                                //byte[] bytes = Encoding.Default.GetBytes(Dr.PageSource);
                                cq = CQ.Create(Dr.PageSource);
                            }
                            catch
                            {
                                Thread.Sleep(5000);
                                try
                                {
                                    //byte[] bytes = Encoding.Default.GetBytes(Dr.PageSource);
                                    cq = CQ.Create(Dr.PageSource);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            pagUrls_done.Add(pagUrls.First());
                            pagUrls.Remove(pagUrls.First());
                        }
                    }


                    foreach (var SC in C.SubCatecories)
                    {
                        Dictionary<string, List<string>> links = new Dictionary<string, List<string>>();
                        string LastName = "";
                        foreach (var a in urls)
                        {
                            CQ Product = new CQ(a);
                            string StName = Product.Find("td.name.is-over.is-hover > a").Text().Trim();
                            if (!String.IsNullOrEmpty(CurrentKonfig.ProductUrl.CheakPrise.value))
                            {


                            }

                            if (!String.IsNullOrEmpty(CurrentKonfig.ProductUrl.Availability.value))
                            {
                                var av = Product.Find(CurrentKonfig.ProductUrl.Availability.value);
                                string StAv = "";
                                switch (CurrentKonfig.ProductUrl.Availability.take)
                                {
                                    case ("Text"):
                                        StAv = av.FirstElement().InnerText.Trim();
                                        if (StAv.Equals(CurrentKonfig.ProductUrl.Availability.expression)) continue;
                                        break;
                                    case ("AllText"):
                                        StAv = av.Text().Trim();
                                        if (StAv.Equals(CurrentKonfig.ProductUrl.Availability.expression)) continue;
                                        break;
                                    default:
                                        StAv = av.FirstElement().Attributes[CurrentKonfig.ProductUrl.Availability.take].Trim();
                                        if (StAv.Equals(CurrentKonfig.ProductUrl.Availability.expression)) continue;
                                        break;
                                }
                            }

                            //string URL = "";
                            //URL = Product.Find("td.name.is-over.is-hover > a").FirstElement().Attributes["href"].ToString();
                            if (!String.IsNullOrEmpty(SC.СombinationName))
                            {
                                if (SC.СombinationValue)
                                {
                                    if(SC.СombinationName.Contains("&"))
                                    {
                                        bool triger = true;
                                        List<string> СombinationNameList = SC.СombinationName.Split('&').ToList();
                                        foreach(string CN in СombinationNameList)
                                        {
                                            if (!StName.Contains(CN)) triger = false;
                                        }
                                        if (!triger) continue;
                                    }
                                    else if(SC.СombinationName.Contains("|"))
                                    {
                                        bool triger = false;
                                        List<string> СombinationNameList = SC.СombinationName.Split('|').ToList();
                                        foreach (string CN in СombinationNameList)
                                        {
                                            if (StName.Contains(CN)) triger = true;
                                        }
                                        if(!triger) continue;
                                    }
                                    else
                                    {
                                        if (!StName.Contains(SC.СombinationName)) continue;
                                    }
                                }
                                else
                                {
                                    if (SC.СombinationName.Contains("&"))
                                    {
                                        bool triger = true;
                                        List<string> СombinationNameList = SC.СombinationName.Split('&').ToList();
                                        foreach (string CN in СombinationNameList)
                                        {
                                            if (StName.Contains(CN)) triger = false;
                                        }
                                        if (!triger) continue;
                                    }
                                    else if (SC.СombinationName.Contains("|"))
                                    {
                                        bool triger = false;
                                        List<string> СombinationNameList = SC.СombinationName.Split('|').ToList();
                                        foreach (string CN in СombinationNameList)
                                        {
                                            if (StName.Contains(CN)) triger = true;
                                        }
                                        if (!triger) continue;
                                    }
                                    else
                                    {
                                        if (StName.Contains(SC.СombinationName)) continue;
                                    }                                    
                                }
                            }

                            if (StName.Equals(LastName))
                            {
                                string size = Product.Find("td.size").FirstElement().InnerText;
                                if (size.Contains("-"))
                                {

                                    string Start = size.Remove(size.IndexOf("-"));
                                    string End = size.Substring(size.IndexOf("-") + 1);
                                    try
                                    {
                                        for (int t = Convert.ToInt32(Start); t <= Convert.ToInt32(End); t++)
                                        {
                                            if (!links.Last().Value.Contains(t.ToString()))
                                                links.Last().Value.Add(t.ToString());
                                        }
                                    }
                                    catch
                                    {
                                        links.Last().Value.Add(size);
                                    }
                                }
                                else if (size.Contains("/"))
                                {
                                    string Start = size.Remove(size.IndexOf("/"));
                                    string End = size.Substring(size.IndexOf("/") + 1);
                                    links.Last().Value.Add(Start);
                                    links.Last().Value.Add(End);
                                }
                                else if (String.IsNullOrEmpty(size)) { }
                                else
                                {
                                    links.Last().Value.Add(size);
                                }
                            }
                            else
                            {
                                try
                                {
                                    links.Add(Product.Find("td.name.is-over.is-hover > a").FirstElement().Attributes["href"], new List<string>());
                                }
                                catch
                                {
                                    continue;
                                }
                                string size = Product.Find("td.size").FirstElement().InnerText.Trim();


                                if (size.Contains("-"))
                                {

                                    string Start = size.Remove(size.IndexOf("-"));
                                    string End = size.Substring(size.IndexOf("-") + 1);
                                    try
                                    {
                                        for (int t = Convert.ToInt32(Start); t <= Convert.ToInt32(End); t++)
                                        {
                                            links.Last().Value.Add(t.ToString());
                                        }
                                    }
                                    catch
                                    {
                                        links.Last().Value.Add(size);
                                    }
                                }
                                else if (size.Contains("/"))
                                {
                                    string Start = size.Remove(size.IndexOf("/"));
                                    string End = size.Substring(size.IndexOf("/") + 1);
                                    links.Last().Value.Add(Start);
                                    links.Last().Value.Add(End);
                                }
                                else if (String.IsNullOrEmpty(size)) { }
                                else
                                {
                                    links.Last().Value.Add(size);
                                }
                            }
                            LastName = StName;
                        }
                        Product_links.Add(new ProductParsListN() { SubCatId = SC.Id, Url = links });
                    }
                    
                    Console.WriteLine(C.url + " "+ Product_links.Count.ToString() + " / "+ CurrentKonfig.Categores.Count );
                }

                int ProductCount = Products.Count + 1;


                foreach (ProductParsListN CatUrls in Product_links)
                {
                    SubCatecory currentSC = CurrentKonfig.Categores.FirstOrDefault(x => x.SubCatecories.Contains(x.SubCatecories.Where(y => y.Id.Equals(CatUrls.SubCatId)).First())).SubCatecories.FirstOrDefault(x => x.Id.Equals(CatUrls.SubCatId));
                    foreach (KeyValuePair<string, List<string>> url in CatUrls.Url)
                    {
                        string TU = url.Key;
                        if (!TU.Contains("http"))
                        {
                            if (TU[0] == '/')
                                TU = TU.Insert(0, domen);
                            else
                            {
                                TU = TU.Insert(0, "/");
                                TU = TU.Insert(0, domen);
                            }

                        }

                        if (CurrentKonfig.selenium)
                        {
                            Dr.Navigate().GoToUrl(TU);
                            Thread.Sleep(1000);
                            cq = CQ.Create(Dr.PageSource);
                        }
                        else
                        {
                            try
                            {
                                cq = CQ.CreateFromUrl(TU);
                            }
                            catch
                            {
                                Thread.Sleep(5000);
                                try
                                {
                                    cq = CQ.CreateFromUrl(TU);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }



                        try
                        {
                            string a = TakeString(cq, CurrentKonfig.Availability.take, CurrentKonfig.Availability.by, CurrentKonfig.Availability.value);
                            a = a.Trim();

                            if (!String.IsNullOrEmpty(CurrentKonfig.Availability.value))
                            {
                                if (a.Equals(CurrentKonfig.Availability.expression))
                                {
                                    continue;
                                }
                            }
                        }
                        catch { }


                        switch (currentSC.price)
                        {
                            case "h":
                                if (!String.IsNullOrEmpty(TakeString(cq, CurrentKonfig.PriceHigh.take, CurrentKonfig.PriceHigh.by, CurrentKonfig.PriceHigh.value))) { continue; }
                                break;
                            case "l":
                                if (!String.IsNullOrEmpty(TakeString(cq, CurrentKonfig.PriceLow.take, CurrentKonfig.PriceHigh.by, CurrentKonfig.PriceHigh.value))) { continue; }
                                break;
                            default:
                                break;
                        }

                        List<ProductPars> product = new List<ProductPars>();
                        try
                        {
                            product = Products.Where(x => x.Url.Equals(TU)).ToList();
                        }
                        catch
                        {

                        }
                        if (product.Count > 1)
                        {
                            var prod = product.OrderBy(x => x.Relevance).First();
                            using (ApplicationDbContext db = new ApplicationDbContext())
                            {
                                foreach (var p in prod.Pictures)
                                {
                                    db.Entry(p).State = EntityState.Deleted;
                                }
                                foreach (var ch in prod.Parameters)
                                {
                                    db.Entry(ch).State = EntityState.Deleted;
                                }
                                db.SaveChanges();
                            }

                            parsing(cq, prod, CurrentKonfig, currentSC, domen, ShC, AU, userId, path, true, url.Value);
                        }
                        else
                        {
                            if (product.Count == 1)
                            {
                                using (ApplicationDbContext db = new ApplicationDbContext())
                                {
                                    foreach (var p in product.First().Pictures)
                                    {
                                        db.Entry(p).State = EntityState.Deleted;
                                    }
                                    foreach (var ch in product.First().Parameters)
                                    {
                                        db.Entry(ch).State = EntityState.Deleted;
                                    }
                                    db.SaveChanges();
                                }

                                parsing(cq, product.First(), CurrentKonfig, currentSC, domen, ShC, AU, userId, path, true, url.Value);
                            }
                            else
                            {
                                ProductPars np = new ProductPars();
                                np.Id = Guid.NewGuid().ToString();
                                np.VendorCode = CurrentKonfig.VendorCode + new string('0', 5 - ProductCount.ToString().Length) + ProductCount.ToString();
                                np.UserId = userId;
                                np.Url = url.Key;
                                np.ShopCode = CurrentKonfig.VendorCode;

                                parsing(cq, np, CurrentKonfig, currentSC, domen, ShC, AU, userId, path, false, url.Value);
                                ProductCount++;
                            }
                        }

                    }

                }
                Dr.Close();
                Dr.Quit();
            }
            else
            {
                IWebDriver Dr = null;
                Console.Clear();
                if (CurrentKonfig.selenium)
                {
                    Dr = new PhantomJSDriver(@"C:\pjs");
                }
                
                domen = cheak_domen(CurrentKonfig.Categores[0].url);
                CQ cq = null;

                List<ProductParsList> Product_links = new List<ProductParsList>();


                for (int i = 0; i < CurrentKonfig.Categores.Count; i++)
                {
                    cq = null;
                    Category C = CurrentKonfig.Categores[i];
                    List<List<string>> CatUrsl = new List<List<string>>();

                    if (CurrentKonfig.selenium)
                    {
                        try
                        {
                            Dr.Navigate().GoToUrl(C.url);
                            Thread.Sleep(2000);
                            cq = CQ.Create(Dr.PageSource);
                        }
                        catch
                        {
                            
                            continue;
                        }

                    }
                    else
                    {
                        bool triger = true;
                        int c = 1;
                        while (triger)
                        {
                            try
                            {
                                cq = CQ.CreateFromUrl(C.url);
                                break;
                            }
                            catch
                            {
                                

                            }
                        }
                        if (!triger) continue;
                    }
                    /*загрузка страницы*/

                    List<string> pagUrls_done = new List<string>();
                    List<string> pagUrls = new List<string>();
                    List<CsQuery.IDomObject> urls = new List<CsQuery.IDomObject>();
                    string VendorSearch = "";
                    foreach(var VS in cq.Find(CurrentKonfig.VendorSearch.value))
                    {
                        var cqVS = CQ.Create(VS);
                        string temp = cqVS.Text();
                        if (temp.IndexOf("(") > -1) temp.Remove(temp.IndexOf("("));
                        VendorSearch += temp + "|";
                    }
                    while (true)
                    {
                        var Purls = CQ.Create();
                        Purls = cq.Find(CurrentKonfig.Pagination.value);

                        if (Purls.Length>0)
                        {
                            foreach (var CU in Purls)
                            {
                                if (!pagUrls.Contains(CU.GetAttribute("href")) & !pagUrls_done.Contains(CU.GetAttribute("href"))) pagUrls.Add(CU.GetAttribute("href"));
                            }
                        }


                        foreach (var a in cq.Find(CurrentKonfig.ProductUrl.value))
                        {
                            urls.Add(a);
                        }


                        if (pagUrls.Count == 0) break;
                        if (pagUrls.Count != 0)
                        {
                            string TU ="";
                            if(String.IsNullOrEmpty(pagUrls.First()))
                            {
                                foreach(string pp in pagUrls)
                                {
                                    if (!String.IsNullOrEmpty(pagUrls.First())) TU = pp; 
                                }
                            }
                            else TU = pagUrls.First();
                            if (String.IsNullOrEmpty(TU))
                            {
                                break;
                            }

                            if (!TU.Contains("http"))
                            {
                                if (TU[0] == '/')
                                    TU = TU.Insert(0, domen);
                                else
                                {
                                    TU = TU.Insert(0, "/");
                                    TU = TU.Insert(0, domen);
                                }

                            }

                            try
                            {
                                cq = CQ.CreateFromUrl(TU);
                            }
                            catch
                            {
                                Thread.Sleep(5000);
                                try
                                {
                                    cq = CQ.CreateFromUrl(TU);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            pagUrls_done.Add(pagUrls.First());
                            pagUrls.Remove(pagUrls.First());
                        }
                    }


                    foreach (var SC in C.SubCatecories)
                    {
                        SC.VendorSearch = VendorSearch;
                        List<string> links = new List<string>();
                        foreach (var a in urls)
                        {
                            CQ Product = new CQ(a);

                            if (!String.IsNullOrEmpty(CurrentKonfig.ProductUrl.CheakPrise.value))
                            {


                            }

                            if (!String.IsNullOrEmpty(CurrentKonfig.ProductUrl.Availability.value))
                            {
                                var av = Product.Find(CurrentKonfig.ProductUrl.Availability.value);
                                string StAv = "";
                                switch (CurrentKonfig.ProductUrl.Availability.take)
                                {
                                    case ("Text"):
                                        StAv = av.FirstElement().InnerText.Trim();
                                        if (StAv.Equals(CurrentKonfig.ProductUrl.Availability.expression)) continue;
                                        break;
                                    case ("AllText"):
                                        StAv = av.Text().Trim();
                                        if (StAv.Equals(CurrentKonfig.ProductUrl.Availability.expression)) continue;
                                        break;
                                    default:
                                        StAv = av.FirstElement().Attributes[CurrentKonfig.ProductUrl.Availability.take].Trim();
                                        if (StAv.Equals(CurrentKonfig.ProductUrl.Availability.expression)) continue;
                                        break;
                                }
                            }

                            string URL = "";
                            URL = Product.FirstElement().Attributes["href"].ToString();
                            if (!String.IsNullOrEmpty(SC.СombinationName))
                            {
                                string StName = "";
                                if (SC.СombinationValue)
                                {
                                    switch (CurrentKonfig.ProductUrl.take)
                                    {
                                        case "Text":
                                            StName = Product.FirstElement().InnerText.Trim();
                                            if (StName.Contains(SC.СombinationName)) links.Add(URL);
                                            break;
                                        case "AllText":
                                            StName = Product.Text().Trim();
                                            if (StName.Contains(SC.СombinationName)) links.Add(URL);
                                            break;
                                        default:
                                            StName = Product.FirstElement().Attributes[CurrentKonfig.ProductUrl.take].Trim();
                                            if (StName.Contains(SC.СombinationName)) links.Add(URL);
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (CurrentKonfig.ProductUrl.take)
                                    {
                                        case "Text":
                                            StName = Product.FirstElement().InnerText.Trim();
                                            if (!StName.Contains(SC.СombinationName)) links.Add(URL);
                                            break;
                                        case "AllText":
                                            StName = Product.Text().Trim();
                                            if (!StName.Contains(SC.СombinationName)) links.Add(URL);
                                            break;
                                        default:
                                            StName = Product.FirstElement().Attributes[CurrentKonfig.ProductUrl.take].Trim();
                                            if (!StName.Contains(SC.СombinationName)) links.Add(URL);
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                links.Add(URL);
                            }


                        }
                        Product_links.Add(new ProductParsList() { Url = links , SubCatId = SC.Id});
                        
                    }
                    
                    Console.WriteLine(C.url + " " + Product_links.Count.ToString() + " / " + CurrentKonfig.Categores.Count + " / " + Product_links.Last().Url.Count);
                }
                int urlCount = 0;
                foreach (ProductParsList ppl in Product_links)
                {
                    urlCount += ppl.Url.Count;
                }
                Console.WriteLine("Всего: " + urlCount.ToString());
                int ProductCount = Products.Count + 1;
                //string SKU = "";

                List<SubCatecory> SCat = new List<SubCatecory>();
                foreach (var a in CurrentKonfig.Categores)
                {
                    SCat.AddRange(a.SubCatecories);
                }

                foreach (ProductParsList CatUrls in Product_links)
                {

                    SubCatecory currentSC = SCat.FirstOrDefault(x=> x.Id.Equals(CatUrls.SubCatId));
                    foreach (string url in CatUrls.Url)
                    {
                        string TU = url;
                        if (!TU.Contains("http"))
                        {
                            if (TU[0] == '/')
                                TU = TU.Insert(0, domen);
                            else
                            {
                                TU = TU.Insert(0, "/");
                                TU = TU.Insert(0, domen);
                            }

                        }

                        if (CurrentKonfig.selenium)
                        {
                            Dr.Navigate().GoToUrl(TU);
                            Thread.Sleep(1000);
                            cq = CQ.Create(Dr.PageSource);
                        }
                        else
                        {
                            try
                            {
                                cq = CQ.CreateFromUrl(TU);
                            }
                            catch
                            {
                                Thread.Sleep(5000);
                                try
                                {
                                    cq = CQ.CreateFromUrl(TU);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }



                        try
                        {
                            string a = TakeString(cq, CurrentKonfig.Availability.take, CurrentKonfig.Availability.by, CurrentKonfig.Availability.value);
                            a = a.Trim();

                            if (!String.IsNullOrEmpty(CurrentKonfig.Availability.value))
                            {
                                if (a.Equals(CurrentKonfig.Availability.expression))
                                {
                                    continue;
                                }
                            }
                        }
                        catch { }


                        switch (currentSC.price)
                        {
                            case "h":
                                if (!String.IsNullOrEmpty(TakeString(cq, CurrentKonfig.PriceHigh.take, CurrentKonfig.PriceHigh.by, CurrentKonfig.PriceHigh.value))) { continue; }
                                break;
                            case "l":
                                if (!String.IsNullOrEmpty(TakeString(cq, CurrentKonfig.PriceLow.take, CurrentKonfig.PriceHigh.by, CurrentKonfig.PriceHigh.value))) { continue; }
                                break;
                            default:
                                break;
                        }

                        List<ProductPars> product = new List<ProductPars>();
                        try
                        {
                            product = Products.Where(x => x.Url.Equals(TU)).ToList();
                        }
                        catch
                        {

                        }
                        if (product.Count > 1)
                        {


                            var prod = product.OrderBy(x => x.Relevance).First();
                            using (ApplicationDbContext db = new ApplicationDbContext())
                            {
                                foreach (var p in prod.Pictures)
                                {
                                    db.Entry(p).State = EntityState.Deleted;
                                }
                                foreach (var ch in prod.Parameters)
                                {
                                    db.Entry(ch).State = EntityState.Deleted;
                                }
                                db.SaveChanges();
                            }

                            parsing(cq, prod, CurrentKonfig, currentSC, domen, ShC, AU, userId, path, true);
                        }
                        else
                        {
                            if (product.Count == 1)
                            {
                                using (ApplicationDbContext db = new ApplicationDbContext())
                                {
                                    foreach (var p in product.First().Pictures)
                                    {
                                        db.Entry(p).State = EntityState.Deleted;
                                    }
                                    foreach (var ch in product.First().Parameters)
                                    {
                                        db.Entry(ch).State = EntityState.Deleted;
                                    }
                                    db.SaveChanges();
                                }

                                parsing(cq, product.First(), CurrentKonfig, currentSC, domen, ShC, AU, userId, path, true);
                            }
                            else
                            {
                                ProductPars np = new ProductPars();
                                np.Id = Guid.NewGuid().ToString();
                                np.VendorCode = CurrentKonfig.VendorCode + new string('0', 5 - ProductCount.ToString().Length) + ProductCount.ToString();
                                np.UserId = userId;
                                np.Url = TU;
                                np.ShopCode = CurrentKonfig.Id;
                                parsing(cq, np, CurrentKonfig, currentSC, domen, ShC, AU, userId, path, false);
                                ProductCount++;
                            }
                        }

                    }
                }

                if(CurrentKonfig.selenium)
                {
                    Dr.Close();
                    Dr.Quit();
                }
                
            }
            
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                CurrentKonfig.Running = false;
                db.Entry(CurrentKonfig).State = EntityState.Modified;
                db.SaveChanges();
            }



        }

        public static async void parsing(CQ cq, ProductPars product, Konfig konfig, SubCatecory curentSC, string domen, List<ShopCategories> SC, List<AutoCorrect> AU, string userId, string path, bool AV)
        {
            product.CatUrl = konfig.Categores.FirstOrDefault(x => x.SubCatecories.Contains(curentSC)).url;
            product.CatUrl = curentSC.Id;
            product.OurCategori = curentSC.NameOurCategory;
            product.PortalCategoryId = curentSC.PortalCategoryId;
            WebClient webClient = new WebClient();
            product.Relevance = true;
            string PicPath = path + @"\Users\" + userId + @"\" + konfig.ShopName + @"\Photos\";
            product.Denomination = "";
            try
            {
                foreach (var den in konfig.Denominations)
                {
                    string VendorDen = TakeString(cq, den.take, den.by, den.value);
                    foreach (var a in AU.Where(x => !String.IsNullOrEmpty(x.MetodsName)))
                    {
                        switch (a.MetodsName)
                        {
                            case "RegexReplace":
                                if (Regex.IsMatch(a.nameStartIndex, VendorDen))
                                {
                                    Regex regex = new Regex(a.nameStartIndex);
                                    VendorDen = regex.Replace(VendorDen, a.name);
                                }
                                break;
                            case "Replace":
                                if (VendorDen.IndexOf(a.nameStartIndex) >= 0)
                                {                                    
                                    VendorDen = VendorDen.Remove(VendorDen.IndexOf(a.nameStartIndex), a.nameStartIndex.Length);
                                }
                                break;
                        }
                    }
                    if (!product.Denomination.Contains(VendorDen)) product.Denomination += VendorDen;
                }
            }
            catch
            {
                if (String.IsNullOrEmpty(product.Denomination))
                {
                    product.Relevance = false;
                    product.Errors += "Ошибка наименование|";
                }

            }
            product.Description = "";
            try
            {
                foreach (var des in konfig.Descriptions)
                {
                    string VendorDes = TakeString(cq, des.take, des.by, des.value);
                    foreach (var a in AU.Where(x => !String.IsNullOrEmpty(x.MetodsDescription)))
                    {
                        switch (a.MetodsDescription)
                        {
                            case "RegexReplace":
                                if (Regex.IsMatch(a.descriptionStartIndex, VendorDes))
                                {
                                    Regex regex = new Regex(a.descriptionStartIndex);
                                    VendorDes = regex.Replace(VendorDes, a.description);
                                }
                                break;
                            case "Replace":
                                if (VendorDes.IndexOf(a.descriptionStartIndex) >= 0)
                                {
                                    var dfd = VendorDes.IndexOf(a.descriptionStartIndex);
                                    VendorDes = VendorDes.Remove(VendorDes.IndexOf(a.descriptionStartIndex), a.descriptionStartIndex.Length);
                                }
                                break;
                        }
                    }
                    if (!product.Description.Contains(VendorDes)) product.Description += VendorDes;
                }
            }
            catch
            {

            }
            string VendorVen = "";
            try
            {
                if(!String.IsNullOrEmpty(curentSC.VendorSearch))
                {
                    List<string> VS = curentSC.VendorSearch.Split('|').ToList();
                    foreach(string ven in VS)
                    {
                        if (product.Denomination.Contains(ven))
                        {
                            product.Vendor = ven;
                            break;
                        }
                    }
                }
                else if(!String.IsNullOrEmpty(konfig.Vendor.take))
                {
                    VendorVen = TakeString(cq, konfig.Vendor.take, konfig.Vendor.by, konfig.Vendor.value);
                    if (!String.IsNullOrEmpty(VendorVen)) product.Vendor = VendorVen;
                }
                else if (!String.IsNullOrEmpty(curentSC.Vendor))
                {
                    product.Vendor = curentSC.Vendor;
                }
            }
            catch
            {

            }

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(PicPath);
                foreach (FileInfo file in dirInfo.GetFiles(product.VendorCode + "*"))
                {
                    file.Delete();
                }

                product.Pictures = new List<Picture>();
                string urlMP = "";
                if (!String.IsNullOrEmpty(konfig.MainPicture.value))
                {
                    urlMP = TakeString(cq, konfig.MainPicture.take, konfig.MainPicture.by, konfig.MainPicture.value);

                    if (!String.IsNullOrEmpty(urlMP))
                    {
                        if (!urlMP.Contains("http"))
                        {
                            if (urlMP[0] == '/')
                                urlMP = urlMP.Insert(0, domen);
                            else
                            {
                                urlMP = urlMP.Insert(0, "/");
                                urlMP = urlMP.Insert(0, domen);
                            }
                        }
                        string format = urlMP.Substring(urlMP.LastIndexOf("."));
                        if (!urlMP.Equals(konfig.MainPicture.empty))
                        {
                            try
                            {
                                webClient.DownloadFile(urlMP, PicPath + product.VendorCode + format);
                                product.Pictures.Add(new Picture("http://sshop.com.ua/Users/" + userId + "/" + konfig.ShopName + "/Photos/" + product.VendorCode + format));
                            }
                            catch
                            {

                            }
                        }

                    }
                }
            }
            catch { }
            try
            {
                foreach (AdditionalPicture AP in konfig.AdditionalPictures)
                {
                    int i = 1;
                    foreach (string PU in TakeListString(cq, AP.take, AP.by, AP.value))
                    {
                        string url = PU;
                        if (String.IsNullOrEmpty(url)) continue;

                        if (!url.Contains("http"))
                        {
                            if (url[0] == '/')
                                url = url.Insert(0, domen);
                            else
                            {
                                url = url.Insert(0, "/");
                                url = url.Insert(0, domen);
                            }

                        }
                        string format = url.Substring(url.LastIndexOf("."));
                        if (url.Equals(AP.empty)) continue;
                        try
                        {
                            webClient.DownloadFile(url, PicPath + product.VendorCode + "_" + i.ToString() + format);
                            product.Pictures.Add(new Picture("http://sshop.com.ua/Users/" + userId + "/" + konfig.ShopName + "/Photos/" + product.VendorCode + "_" + i.ToString() + format));
                            i++;
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch { }
            if (product.Pictures.Count == 0)
            {
                product.Relevance = false;
                product.Errors += "Отсутствие фото|";
            }


            product.Price = "";

            string VevPrice = "";
            try
            {
                switch (curentSC.price)
                {
                    case "h":
                        VevPrice = TakeString(cq, konfig.PriceHigh.take, konfig.PriceHigh.by, konfig.PriceHigh.value);
                        
                        if (!String.IsNullOrEmpty(konfig.PriceHigh.expression) & VevPrice.IndexOf(konfig.PriceLow.expression) > -1)
                        {
                            VevPrice = VevPrice.Remove(VevPrice.IndexOf(konfig.PriceHigh.expression));
                        }
                        if (VevPrice.Contains(".")) VevPrice.Replace(".", "");
                        if (konfig.PriceHigh.coins)
                        {
                            if (VevPrice.IndexOf(",") > -1)
                            {
                                VevPrice = VevPrice.Remove(VevPrice.IndexOf(","));
                            }
                            
                        }
                        else
                        {
                            VevPrice = Convert.ToInt32(Convert.ToDouble(Regex.Replace(VevPrice, @"[^\d]+", ""))).ToString();
                        }                        
                        break;
                    case "l":
                        VevPrice = TakeString(cq, konfig.PriceLow.take, konfig.PriceLow.by, konfig.PriceLow.value);
                        
                        if (!String.IsNullOrEmpty(konfig.PriceLow.expression) & VevPrice.IndexOf(konfig.PriceLow.expression)>-1)
                        {
                            VevPrice = VevPrice.Remove(VevPrice.IndexOf(konfig.PriceLow.expression));
                        }
                        if (VevPrice.Contains(".")) VevPrice.Replace(".", "");
                        if (konfig.PriceLow.coins)
                        {
                            if (VevPrice.IndexOf(",") > -1)
                            {
                                VevPrice = VevPrice.Remove(VevPrice.IndexOf(","));
                            }
                        }
                        else
                        {
                            VevPrice = Convert.ToInt32(Convert.ToDouble(Regex.Replace(VevPrice, @"[^\d]+", ""))).ToString();
                        }
                        break;
                    default:
                        if (String.IsNullOrEmpty(TakeString(cq, konfig.PriceHigh.take, konfig.PriceHigh.by, konfig.PriceHigh.value)))
                        {
                            if (String.IsNullOrEmpty(TakeString(cq, konfig.PriceLow.take, konfig.PriceLow.by, konfig.PriceLow.value)))
                            {
                                product.Relevance = false;
                                product.Errors += "Ошибка поиска цены|";
                            }
                            else
                            {
                                VevPrice = TakeString(cq, konfig.PriceLow.take, konfig.PriceLow.by, konfig.PriceLow.value);                                
                                if (!String.IsNullOrEmpty(konfig.PriceLow.expression) & VevPrice.IndexOf(konfig.PriceLow.expression) > -1)
                                {
                                    VevPrice = VevPrice.Remove(VevPrice.IndexOf(konfig.PriceLow.expression));
                                }
                                if (VevPrice.Contains(".")) VevPrice.Replace(".", "");
                                if (konfig.PriceLow.coins)
                                {
                                    if (VevPrice.IndexOf(",") > -1)
                                    {
                                        VevPrice = VevPrice.Remove(VevPrice.IndexOf(","));
                                    }
                                }
                                else
                                {
                                    VevPrice = Convert.ToInt32(Convert.ToDouble(Regex.Replace(VevPrice, @"[^\d]+", ""))).ToString();
                                }
                            }
                        }
                        else
                        {
                            VevPrice = TakeString(cq, konfig.PriceHigh.take, konfig.PriceHigh.by, konfig.PriceHigh.value);                            
                            if (!String.IsNullOrEmpty(konfig.PriceHigh.expression) & VevPrice.IndexOf(konfig.PriceLow.expression) > -1)
                            {
                                VevPrice = VevPrice.Remove(VevPrice.IndexOf(konfig.PriceHigh.expression));
                            }
                            if (VevPrice.Contains(".")) VevPrice.Replace(".", "");
                            if (konfig.PriceHigh.coins)
                            {
                                if (VevPrice.IndexOf(",") > -1)
                                {
                                    VevPrice = VevPrice.Remove(VevPrice.IndexOf(","));
                                }
                            }
                            else
                            {
                                VevPrice = Convert.ToInt32(Convert.ToDouble(Regex.Replace(VevPrice, @"[^\d]+", ""))).ToString();
                            }
                        }
                        break;
                }
            }
            catch
            {
                if (!String.IsNullOrEmpty(VevPrice) && !VevPrice.Equals("0"))
                {                    
                    product.Relevance = false;
                    product.Errors += "Ошибка в обработке цены|";
                }
            }
            if (VevPrice.Equals("0"))
            {
                product.Relevance = false;
                product.Errors += "Цена равна '0'|";
            }

            if (VevPrice.Equals("") | String.IsNullOrEmpty(VevPrice))
            {
                product.Relevance = false;
                product.Errors += "Цена равна 'null'|";
            }
            try
            {
                if (!String.IsNullOrEmpty(curentSC.price_processing) && !curentSC.price_processing.Equals("0") && !String.IsNullOrEmpty(VevPrice) && !VevPrice.Equals("0"))
                {
                    double Pr = Convert.ToDouble(VevPrice);
                    double k = Convert.ToDouble(curentSC.price_processing);
                    if (curentSC.price_processing.Contains("-"))
                    {
                        VevPrice = Convert.ToString(Convert.ToInt32(Pr - ((Pr / 100) * k)));
                    }
                    else
                    {
                        VevPrice = Convert.ToString(Convert.ToInt32(Pr + ((Pr / 100) * k)));
                    }
                }
            }
            catch {
                product.Relevance = false;
                product.Errors += "Ошибка при обработке цены|";
            }
            try
            {
                if (!String.IsNullOrEmpty(curentSC.discount) && !curentSC.discount.Equals("0") && !String.IsNullOrEmpty(VevPrice) && !VevPrice.Equals("0"))
                {
                    double Pr = Convert.ToDouble(VevPrice);
                    double k = Convert.ToDouble(curentSC.discount);
                    if (curentSC.discount.Contains("-"))
                    {
                        VevPrice = Convert.ToString(Convert.ToInt32(Pr - ((Pr / 100) * k)));
                    }
                    else
                    {
                        VevPrice = Convert.ToString(Convert.ToInt32(Pr + ((Pr / 100) * k)));
                    }
                }
            }
            catch {
                product.Relevance = false;
                product.Errors += "Ошибка при обработке скидки|";
            }
            product.Price = VevPrice;
            product.PriceOld = VevPrice;

            string KeyW = "";
            product.KeyWords = "";
            List<string> KeyId = new List<string>() { product.OurCategori };
            try
            {
                while (true)
                {
                    if (!String.IsNullOrEmpty(SC.FirstOrDefault(x => x.NumCat.Equals(KeyId.Last())).parentNum))
                    {
                        KeyId.Add(SC.FirstOrDefault(x => x.NumCat.Equals(KeyId.Last())).parentNum);
                    }
                    else break;
                }
            }
            catch { }

            if (KeyId.Count != 0)
            {
                try
                {
                    foreach (var c in KeyId)
                    {
                        KeyW += SC.FirstOrDefault(x => x.NumCat.Equals(c)).name + ", ";
                    }
                }
                catch { }
            }
                

            product.Parameters = new List<Parameters>();
            foreach (var ch in curentSC.Characteristics)
            {
                if (ch.Name.Equals("Наименование") | ch.Name.Equals("Назначение")) KeyW += ch.Value + ", ";
                product.Parameters.Add(new Parameters(ch.Name, ch.Value, ch.Group));
            }
            if (String.IsNullOrEmpty(KeyW))
            {
                product.Relevance = false;
                product.Errors += "Отсуцтвие ключевых слов|";
            }

            else product.KeyWords = KeyW;

            try
            {
                foreach (Characteristic ch in konfig.MainCharacteristics)
                {
                    List<List<string>> Characteristics = TakeCharacteristics(cq, ch);
                    foreach (var par in Characteristics)
                    {
                        product.Parameters.Add(new Parameters(par[0], par[1], ch.Group));
                    }
                }
            }
            catch
            {

            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //product.Relevance = true;
                if (AV)
                {
                    db.Entry(product).State = EntityState.Modified;
                }
                else
                {
                    db.Entry(product).State = EntityState.Added;
                }
                foreach (var p in product.Pictures)
                {
                    db.Entry(p).State = EntityState.Added;
                }
                foreach (var ch in product.Parameters)
                {
                    db.Entry(ch).State = EntityState.Added;
                }

                //db.ProductsPars.AddOrUpdate(product);
                await db.SaveChangesAsync();
            }
        }
        

        public static async void parsing(CQ cq, ProductPars product, Konfig konfig, SubCatecory curentSC, string domen, List<ShopCategories> SC, List<AutoCorrect> AU, string userId, string path, bool AV, List<string> Size)
        {
            product.OurCategori = curentSC.NameOurCategory;
            product.PortalCategoryId = curentSC.PortalCategoryId;
            WebClient webClient = new WebClient();
            product.Relevance = true;
            string PicPath = path + @"Users\" + userId + @"\" + konfig.ShopName + @"\Photos\";
            product.Denomination = "";
            foreach (var den in konfig.Denominations)
            {
                string VendorDen = TakeString(cq, den.take, den.by, den.value);
                foreach (var a in AU.Where(x => String.IsNullOrEmpty(x.MetodsName)))
                {
                    switch (a.MetodsName)
                    {
                        case "RegexReplace":
                            if (Regex.IsMatch(a.nameStartIndex, VendorDen))
                            {
                                Regex regex = new Regex(a.nameStartIndex);
                                VendorDen = regex.Replace(VendorDen, a.name);
                            }
                            break;
                        case "Replace":
                            if (VendorDen.Contains(a.nameStartIndex))
                            {
                                VendorDen = VendorDen.Replace(a.nameStartIndex, a.name);
                            }
                            break;
                    }
                }
                if (!product.Denomination.Contains(VendorDen)) product.Denomination += VendorDen;
            }
            product.Description = "";
            foreach (var des in konfig.Descriptions)
            {
                string VendorDes = TakeString(cq, des.take, des.by, des.value);
                foreach (var a in AU.Where(x => String.IsNullOrEmpty(x.MetodsDescription)))
                {
                    switch (a.MetodsDescription)
                    {
                        case "RegexReplace":
                            if (Regex.IsMatch(a.descriptionStartIndex, VendorDes))
                            {
                                Regex regex = new Regex(a.descriptionStartIndex);
                                VendorDes = regex.Replace(VendorDes, a.description);
                            }
                            break;
                        case "Replace":
                            if (VendorDes.Contains(a.descriptionStartIndex))
                            {
                                VendorDes = VendorDes.Replace(a.descriptionStartIndex, a.description);
                            }
                            break;
                    }
                }
                if (!product.Description.Contains(VendorDes)) product.Description += VendorDes;
            }
            string VendorVen = "";
            if (!String.IsNullOrEmpty(konfig.Vendor.take))
            {
                VendorVen = TakeString(cq, konfig.Vendor.take, konfig.Vendor.by, konfig.Vendor.value);
                if (!String.IsNullOrEmpty(VendorVen)) product.Vendor = VendorVen;
            }
            else if (!String.IsNullOrEmpty(curentSC.Vendor))
            {
                product.Vendor = curentSC.Vendor;
            }


            DirectoryInfo dirInfo = new DirectoryInfo(PicPath);
            foreach (FileInfo file in dirInfo.GetFiles(product.VendorCode + "*"))
            {
                file.Delete();
            }

            product.Pictures = new List<Picture>();
            string urlMP = "";
            if (!String.IsNullOrEmpty(konfig.MainPicture.value))
            {
                urlMP = TakeString(cq, konfig.MainPicture.take, konfig.MainPicture.by, konfig.MainPicture.value);

                if (!urlMP.Contains("http") & !String.IsNullOrEmpty(urlMP))
                {
                    if (urlMP[0] == '/')
                        urlMP = urlMP.Insert(0, domen);
                    else
                    {
                        urlMP = urlMP.Insert(0, "/");
                        urlMP = urlMP.Insert(0, domen);
                    }
                }
                string format = urlMP.Substring(urlMP.LastIndexOf("."));
                try
                {
                    webClient.DownloadFile(urlMP, PicPath + product.VendorCode + format);
                    product.Pictures.Add(new Picture("http://sshop.com.ua/Users/" + userId + "/" + konfig.ShopName + "/" + product.VendorCode + format));
                }
                catch
                {

                }
            }


            foreach (AdditionalPicture AP in konfig.AdditionalPictures)
            {
                int i = 1;
                foreach (string PU in TakeListString(cq, AP.take, AP.by, AP.value))
                {
                    string url = PU;
                    if (String.IsNullOrEmpty(url)) continue;
                    if (!url.Contains("http"))
                    {
                        if (url[0] == '/')
                            url = url.Insert(0, domen);
                        else
                        {
                            url = url.Insert(0, "/");
                            url = url.Insert(0, domen);
                        }

                    }
                    string format = url.Substring(url.LastIndexOf("."));
                    try
                    {
                        webClient.DownloadFile(url, PicPath + product.VendorCode + "_" + i.ToString() + format);
                        product.Pictures.Add(new Picture("http://sshop.com.ua/Users/" + userId + "/" + konfig.ShopName + "/" + product.VendorCode + "_" + i.ToString() + format));
                        i++;
                    }
                    catch
                    {

                    }
                }
            }
            if (product.Pictures.Count == 0) product.Relevance = false;

            product.Price = "";

            string VevPrice = "";
            switch (curentSC.price)
            {
                case "h":
                    VevPrice = Regex.Replace(TakeString(cq, konfig.PriceHigh.take, konfig.PriceHigh.by, konfig.PriceHigh.value), @"[^\d]+", "");
                    break;
                case "l":
                    VevPrice = Regex.Replace(TakeString(cq, konfig.PriceLow.take, konfig.PriceLow.by, konfig.PriceLow.value), @"[^\d]+", "");
                    break;
                default:
                    if (String.IsNullOrEmpty(TakeString(cq, konfig.PriceHigh.take, konfig.PriceHigh.by, konfig.PriceHigh.value)))
                    {
                        if (String.IsNullOrEmpty(TakeString(cq, konfig.PriceLow.take, konfig.PriceLow.by, konfig.PriceLow.value)))
                        {
                            //return;
                            product.Relevance = false;
                        }
                        else
                        {
                            VevPrice = Regex.Replace(TakeString(cq, konfig.PriceLow.take, konfig.PriceLow.by, konfig.PriceLow.value), @"[^\d]+", "");
                        }
                    }
                    else
                    {
                        VevPrice = Regex.Replace(TakeString(cq, konfig.PriceHigh.take, konfig.PriceHigh.by, konfig.PriceHigh.value), @"[^\d]+", "");
                    }
                    break;
            }
            if (VevPrice.Equals("0")) product.Relevance = false;
            if (!String.IsNullOrEmpty(curentSC.price_processing) && !curentSC.price_processing.Equals("0"))
            {
                double Pr = Convert.ToDouble(VevPrice);
                double k = Convert.ToDouble(curentSC.price_processing);
                if (curentSC.price_processing.Contains("-"))
                {
                    VevPrice = Convert.ToString(Pr - ((Pr / 100) * k));
                }
                else
                {
                    VevPrice = Convert.ToString(Pr + ((Pr / 100) * k));
                }
            }
            if (!String.IsNullOrEmpty(curentSC.discount) && !curentSC.discount.Equals("0"))
            {
                double Pr = Convert.ToDouble(VevPrice);
                double k = Convert.ToDouble(curentSC.discount);
                if (curentSC.discount.Contains("-"))
                {
                    VevPrice = Convert.ToString(Pr - ((Pr / 100) * k));
                }
                else
                {
                    VevPrice = Convert.ToString(Pr + ((Pr / 100) * k));
                }
            }
            product.Price = VevPrice;
            product.PriceOld = VevPrice;

            string KeyW = "";
            product.KeyWords = "";
            List<string> KeyId = new List<string>() { product.OurCategori };
            while (true)
            {
                if (!String.IsNullOrEmpty(SC.FirstOrDefault(x => x.NumCat.Equals(KeyId.Last())).parentNum))
                {
                    KeyId.Add(SC.FirstOrDefault(x => x.NumCat.Equals(KeyId.Last())).parentNum);
                }
                else break;
            }

            foreach (var c in KeyId)
            {
                KeyW += SC.FirstOrDefault(x => x.NumCat.Equals(c)).name + ", ";
            }


            product.Parameters = new List<Parameters>();
            foreach (var ch in curentSC.Characteristics)
            {
                if (ch.Name.Equals("Наименование") | ch.Name.Equals("Назначение")) KeyW += ch.Value + ", ";
                product.Parameters.Add(new Parameters(ch.Name, ch.Value, ch.Group));
            }


            foreach (Characteristic ch in konfig.MainCharacteristics)
            {
                List<List<string>> Characteristics = TakeCharacteristics(cq, ch);


                foreach (var par in Characteristics)
                {
                    product.Parameters.Add(new Parameters(par[0], par[1], ch.Group));
                }
            }

            foreach (string ch in Size)
            {

                product.Parameters.Add(new Parameters("Размеры", WebUtility.HtmlDecode(ch), true));

            }
            product.KeyWords = KeyW;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //product.Relevance = true;
                if (AV)
                {
                    db.Entry(product).State = EntityState.Modified;
                }
                else
                {
                    db.Entry(product).State = EntityState.Added;
                }
                foreach (var p in product.Pictures)
                {
                    db.Entry(p).State = EntityState.Added;
                }
                foreach (var ch in product.Parameters)
                {
                    db.Entry(ch).State = EntityState.Added;
                }

                //db.ProductsPars.AddOrUpdate(product);
                await db.SaveChangesAsync();
            }
        }


        public static string TakeString(CQ cq, string take, string by, string value)
        {
            string req = "";
            if (!String.IsNullOrEmpty(take) & !String.IsNullOrEmpty(by) & !String.IsNullOrEmpty(value))
            {
                try
                {
                    switch (take)
                    {
                        case "Text":
                            req = WebUtility.HtmlDecode(cq.Find(value).FirstElement().InnerText).Trim();
                            break;
                        case "AllText":
                            req = cq.Find(value).Text().Trim();
                            break;
                        default:
                            req = cq.Find(value).FirstElement().Attributes[take].ToString().Trim();
                            break;
                    }
                }
                catch { }
            }
            return req;

        }
        public static List<string> TakeListString(CQ cq, string take, string by, string value)
        {
            List<string> req = new List<string>();
            if (!String.IsNullOrEmpty(take) & !String.IsNullOrEmpty(by) & !String.IsNullOrEmpty(value))
            {
                try
                {
                    foreach (var a in cq.Find(value))
                    {
                        req.Add(WebUtility.HtmlDecode(a.Attributes[take].Trim()));
                    }
                }
                catch
                {
                    return req;
                }
            }
            return req;
        }

        public static List<List<string>> TakeCharacteristics(CQ cq, Characteristic ch)
        {
            List<List<string>> req = new List<List<string>>();
            List<CsQuery.IDomObject> par = new List<CsQuery.IDomObject>();
            try
            {
                foreach (var a in cq.Find(ch.Main))
                {
                    par.Add(a);
                }
            }
            catch
            {
                return req;
            }

            foreach (var p in par)
            {
                bool triger = false;
                CQ Par = new CQ(p);
                string vel = "";
                string name = "";
                switch (ch.NameBy)
                {
                    case "CSS":
                        vel = Par.Find(ch.Value).Text().Trim();
                        name = Par.Find(ch.Name).Text().Trim();
                        try
                        {


                            foreach (var a in ch.Exceptions.Where(x => !String.IsNullOrEmpty(x.name) & x.name.Equals("value")).ToList())
                            {
                                if (vel.Contains(a.value))
                                {
                                    triger = true;
                                    break;
                                }
                            }
                            if (triger) continue;
                            foreach (var a in ch.Exceptions.Where(x => !String.IsNullOrEmpty(x.name) & x.name.Equals("name")).ToList())
                            {
                                if (name.Contains(a.value))
                                {
                                    triger = true;
                                    break;
                                }
                            }
                        }
                        catch { }
                        if (triger) continue;
                        else
                        {
                            req.Add(new List<string>() { name, vel });
                        }

                        break;
                    case "Text":
                        switch (ch.ValueBy)
                        {
                            case "CSS": vel = Par.Find(ch.Value).Text().Trim(); break;
                            case "Text": vel = Par.Text().Trim(); break;
                        }
                        try
                        {
                            foreach (var a in ch.Exceptions.Where(x => !String.IsNullOrEmpty(x.name) & x.name.Equals("value")).ToList())
                            {
                                if (vel.Contains(a.value))
                                {
                                    triger = true;
                                    break;
                                }
                            }
                        }
                        catch { }
                        if (triger) continue;
                        try
                        {
                            foreach (var a in ch.Exceptions.Where(x => !String.IsNullOrEmpty(x.name) & x.name.Equals("value_separator")).ToList())
                            {
                                string pat = (@"\d*" + a.value + @"\d*");
                                if (Regex.IsMatch(vel, pat))
                                {
                                    string Start = vel.Remove(vel.IndexOf(a.value));
                                    string End = vel.Substring(vel.IndexOf(a.value) + 1);
                                    for (int t = Convert.ToInt32(Start); t <= Convert.ToInt32(End); t++)
                                    {
                                        req.Add(new List<string>() { ch.Name, t.ToString() });

                                    }
                                    triger = true;
                                }
                            }
                        }
                        catch { }
                        if (triger) continue;
                        else
                        {
                            req.Add(new List<string>() { ch.Name, vel });
                        }
                        break;
                }
            }

            return req;
        }

        public static string cheak_domen(string catalog_url)
        {
            string domen = catalog_url.Remove(catalog_url.IndexOf("//"));
            domen += "//";
            catalog_url = catalog_url.Replace(domen, "");
            domen += catalog_url.Remove(catalog_url.IndexOf("/"));

            return domen;
        }
    }
}

