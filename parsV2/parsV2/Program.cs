using CsQuery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using OpenQA.Selenium;

using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.PhantomJS;
namespace parsV2
{
    public class Program
    {
        static public string path = Directory.GetCurrentDirectory();
        static public string prev_path = path.Remove(path.LastIndexOf(@"\"));
        static public Konfig CurrentKonfig = new Konfig();
        static public List<AutoCorrect> CurrentAutoCorrect = new List<AutoCorrect>();
        static public List<MatchList> CurrentMatchList = new List<MatchList>();
        static public List<Categories> Categories = new List<Categories>();
        static public string domen = "";
        static void Main(string[] args)
        {
            if (args.Length > 0)
                path = args[0];
            else
                path = Directory.GetCurrentDirectory();
            prev_path = path.Remove(path.LastIndexOf(@"\"));

            string name_file = path.Replace(prev_path, "");
            name_file = name_file.Replace(@"\", "");
            Console.Title = name_file;
            pars();
        }
        static public void pars()
        {
            
            XmlDocument doc = new XmlDocument();

            var document = new StreamReader(path + @"\Konfig.xml", Encoding.UTF8, false);
            var contents = document.ReadToEnd();
            string EncodedString = contents.Replace("&", "&amp;");
            doc.LoadXml(EncodedString);
            
            CurrentKonfig.MainOptions = new List<MainOption>();
            foreach (XmlNode MO in doc.SelectNodes("/shop/main_options"))
            {
                CurrentKonfig.MainOptions.Add(new MainOption(
                                                    MO.Attributes["name"].Value,
                                                    MO.Attributes["By"].Value,
                                                    MO.Attributes["take"].Value,
                                                    MO.InnerText));
            }
            CurrentKonfig.Categores = new List<Category>();
            int i = 0;

            foreach (XmlNode Cat in doc.SelectNodes("/shop/categories/category"))
            {
                List<SubCatecory> TempSubCatecory = new List<SubCatecory>();
                foreach (XmlNode SC in Cat.SelectNodes(".//SubCatecory"))
                {
                    bool mainFlaf = false;
                    List<Characteristic> ListCharectCat = new List<Characteristic>();
                    int j = 0;
                    foreach (XmlNode CH in SC.SelectNodes(".//characteristics"))
                    {
                        List<Except> ex = new List<Except>();
                        foreach (XmlNode Exep in CH.SelectNodes(".//exception"))
                        {
                            ex.Add(new Except(Exep.Attributes["name"].InnerText,
                                                Exep.InnerText));
                        }
                        Characteristic temp = new Characteristic(j.ToString(),
                                                            Convert.ToBoolean(CH.Attributes["group"].InnerText),
                                                            CH.SelectSingleNode(".//main").InnerText,
                                                            CH.SelectSingleNode(".//main").Attributes["By"].InnerText,
                                                            CH.SelectSingleNode(".//name").InnerText,
                                                            CH.SelectSingleNode(".//name").Attributes["By"].InnerText,
                                                            CH.SelectSingleNode(".//value").InnerText,
                                                            CH.SelectSingleNode(".//value").Attributes["By"].InnerText,
                                                            ex);
                        ListCharectCat.Add(temp);
                        j++;
                    }

                    TempSubCatecory.Add(new SubCatecory(
                                                        SC.SelectSingleNode(".//name_our_categori").InnerText,
                                                        SC.SelectSingleNode(".//PortalCategoryId").InnerText,
                                                        SC.SelectSingleNode(".//discount").InnerText,
                                                        SC.SelectSingleNode(".//price").InnerText,
                                                        SC.SelectSingleNode(".//price_processing").InnerText,
                                                        SC.SelectSingleNode(".//combination").InnerText,
                                                        SC.SelectSingleNode(".//vendor").InnerText,
                                                        Convert.ToBoolean(SC.SelectSingleNode(".//combination").Attributes["value"].InnerText),
                                                        mainFlaf,
                                                        ListCharectCat));
                }

                CurrentKonfig.Categores.Add(new Category(i, Cat.SelectSingleNode(".//url").InnerText,
                                                            TempSubCatecory));
                i++;
            }
            
            doc.Load(path + @"\AutoCorrect.xml");
            foreach (XmlNode MO in doc.SelectNodes("/products/product"))
            {
                CurrentAutoCorrect.Add(new AutoCorrect(
                                                    CurrentAutoCorrect.Count.ToString(),
                                                    MO.Attributes["id"].Value,
                                                    MO.SelectSingleNode(".//name").InnerText,
                                                    MO.SelectSingleNode(".//name").Attributes["metod"].Value,
                                                    MO.SelectSingleNode(".//name").Attributes["start"].Value,
                                                    MO.SelectSingleNode(".//name").Attributes["end"].Value,
                                                    MO.SelectSingleNode(".//description").InnerText,
                                                    MO.SelectSingleNode(".//description").Attributes["metod"].Value,
                                                    MO.SelectSingleNode(".//description").Attributes["start"].Value,
                                                    MO.SelectSingleNode(".//description").Attributes["end"].Value,
                                                    MO.SelectSingleNode(".//group").InnerText));

            }


            doc.Load(prev_path + @"\categories.xml");
            


            foreach (XmlNode Category in doc.SelectNodes("/categories/category"))
            {
                string parentId = "";
                if (Category.Attributes["parentId"] != null) parentId = Category.Attributes["parentId"].InnerText;
                bool flag = false;
                if (Categories.FirstOrDefault(x => x.name.Equals(Category.InnerText)) != null)
                    flag = true;
                Categories.Add(new Categories(
                                                Category.InnerText,
                                                Category.Attributes["id"].InnerText,
                                                parentId,
                                                flag));
            }
            List<ProductList> CurProductList = new List<ProductList>();

            if (File.Exists(path + @"\products_list.xml"))
            {
                string text = File.ReadAllText(path + @"\products_list.xml").Replace("&", "&amp;");
                doc.LoadXml(text);
            }           

            foreach (XmlNode PL in doc.SelectNodes("/PL/product"))
            {
                CurProductList.Add(new ProductList(PL.Attributes["Id"].Value, PL.Attributes["url"].Value));
            }


            Dictionary<string,string> ProductCatId = new Dictionary<string, string>();


            if (File.Exists(path + @"\products.xml"))
            {
                doc.LoadXml(File.ReadAllText(path + @"\products.xml").Replace("&", "&amp;"));
            }            

            foreach (XmlNode PCI in doc.SelectNodes("/of/offer"))
            {
                ProductCatId.Add(PCI.Attributes["id"].Value , PCI.SelectSingleNode(".//categoryId").InnerText);
            }

            
            doc.Load(path + @"\MatchList.xml");
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

            CQ cq = null;
            List<List<List<string>>> Product_links = new List<List<List<string>>>();
            

            foreach (Category C in CurrentKonfig.Categores)
            {
                List<List<string>> CatUrsl = new List<List<string>>();
                try
                {
                    cq = CQ.CreateFromUrl(C.url);
                }
                catch
                {
                    Thread.Sleep(5000);
                    try
                    {
                        cq = CQ.CreateFromUrl(C.url);
                    }
                    catch
                    {
                        Thread.Sleep(5000);
                        try
                        {
                            cq = CQ.CreateFromUrl(C.url);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                }
                Dictionary<string, string> urls = new Dictionary<string, string>();
                string h = cq.Find("head").FirstElement().OuterHTML;
                string b = cq.Find("body").FirstElement().OuterHTML;
                string s = "<html>" + h + b + "</html>";
                switch (CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("pagination")).take)
                {
                    case "urslList":
                        //try
                        //{
                        List<string> pagUrls_done = new List<string>();
                        List<string> pagUrls = new List<string>();
                        while (true)
                        {
                            var Purls = CQ.Create();
                            Purls = cq.Find(CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("pagination")).value);

                            if (Purls != null)
                            {
                                foreach (var CU in Purls)
                                {
                                    if (!pagUrls.Contains(CU.GetAttribute("href")) & !pagUrls_done.Contains(CU.GetAttribute("href"))) pagUrls.Add(CU.GetAttribute("href"));
                                }
                            }


                            foreach (var a in cq.Find(CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("product_url")).value))
                            {
                                if (String.IsNullOrEmpty(urls.FirstOrDefault(x => x.Key.Equals(a.Attributes["href"])).Key))
                                {
                                    urls.Add(a.Attributes["href"], a.InnerText);
                                }

                            }


                            if (pagUrls.Count == 0) break;
                            if (pagUrls.Count != 0)
                            {
                                try
                                {
                                    cq = CQ.CreateFromUrl(pagUrls.First());
                                }
                                catch
                                {
                                    Thread.Sleep(5000);
                                    try
                                    {
                                        cq = CQ.CreateFromUrl(pagUrls.First());
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

                        //}
                        //catch
                        //{
                        //    continue;
                        //}
                        foreach (var SC in C.SubCatecories)
                        {
                            List<string> links = new List<string>();
                            foreach (var a in urls)
                            {
                                if (!links.Contains(a.Key))
                                {
                                    if (!String.IsNullOrEmpty(SC.СombinationName))
                                    {
                                        if (SC.СombinationValue)
                                        {
                                            switch (CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("product_url")).take)
                                            {
                                                case "Text":

                                                    if (a.Value.Contains(SC.СombinationName)) links.Add(a.Key);
                                                    else continue;
                                                    break;
                                                default:
                                                    if (a.Value.Contains(SC.СombinationName)) links.Add(a.Key);
                                                    else continue;
                                                    break;

                                            }
                                        }
                                        else
                                        {
                                            switch (CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("product_url")).take)
                                            {
                                                case "Text":
                                                    if (!a.Value.Contains(SC.СombinationName)) links.Add(a.Key);
                                                    else continue;
                                                    break;
                                                default:
                                                    if (!a.Value.Contains(SC.СombinationName)) links.Add(a.Key);
                                                    else continue;
                                                    break;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            links.Add(a.Key);
                                        }
                                        catch { continue; }
                                    }
                                }
                            }
                            CatUrsl.Add(links);
                        }
                        break;
                }
                Product_links.Add(CatUrsl);
            }
            string date = DateTime.Now.Date.ToString().Replace(".", "_").Replace(":", "_") + ".xml";
            File.Copy(path + @"\products.xml", path + @"\products_old" + date);
            File.Copy(path + @"\products_list.xml", path + @"\products_list_old" + date);

            int ProductCount = ProductCatId.Count+1;
            string SKU = "";
            IWebDriver Dr;
            
            Dr = new PhantomJSDriver();
            File.WriteAllText(path + @"\products.xml", String.Empty);
            File.WriteAllText(path + @"\products_list.xml", String.Empty);
            SubCatecory curentSC = new SubCatecory();
            foreach (List<List<string>> CatUrls in Product_links)
            {
                foreach(List<string> SCUrls in CatUrls)
                {
                    curentSC = CurrentKonfig.Categores[Product_links.IndexOf(CatUrls)].SubCatecories[CatUrls.IndexOf(SCUrls)];
                    foreach (string url in SCUrls)
                    {

                        
                        Dr.Navigate().GoToUrl(url);
                        string aPS = Dr.PageSource;
                 
                        try
                        {
                            cq = CQ.CreateDocument(aPS);
                        }
                        catch
                        {
                            Thread.Sleep(5000);
                            try
                            {
                                cq = CQ.CreateFromUrl(url);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        MainOption AV = CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("availability"));
                        string a = WebUtility.HtmlDecode(Take(cq, AV).ToUpper());
                        a = a.Trim();
                        a = a.ToUpper();
                        if (!String.IsNullOrEmpty(AV.value))
                        {                            
                            if (a.Contains("НЕТ В НАЛИЧИИ"))
                            {
                                continue;
                            }
                        }

                        switch (curentSC.price)
                        {
                            case "all":
                                break;
                            default:
                                if (!String.IsNullOrEmpty(Take(cq, CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("price_low")))) & !String.IsNullOrEmpty(Take(cq, CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("price_high"))))) { continue; }
                                break;
                        }

                        ProductList product = CurProductList.FirstOrDefault(x=> x.url.Equals(url));
                        if(product !=null)
                        {
                            KeyValuePair<string,string> prodCat = ProductCatId.FirstOrDefault(x => x.Key.Equals(product.sku) & x.Value.Equals(curentSC.NameOurCategory));
                            if(!String.IsNullOrEmpty(prodCat.Key))
                            {
                                SKU = prodCat.Key;
                            }
                            else
                            {
                                SKU = CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("vendor_code")).value + new string('0', 5 - ProductCount.ToString().Length) + ProductCount.ToString();
                            }
                        }
                        else
                        {
                            SKU = CurrentKonfig.MainOptions.FirstOrDefault(x => x.name.Equals("vendor_code")).value + new string('0', 5 - ProductCount.ToString().Length) + ProductCount.ToString();
                        }
                        
                        parsing(cq, curentSC, SKU);
                        ProductCount++;
                    }
                }
            }
        }



        static public void parsing(CQ Dr , SubCatecory SC , string sku)
        {
            WebClient webClient = new WebClient();
            string products = "";
            string product = "";
            string product_to_List = "";
            List<AutoCorrect> productAU = new List<AutoCorrect>();
            productAU = CurrentAutoCorrect.Where(x => x.sku.Equals(sku) | x.sku.Equals("")).ToList();

            foreach (var Mparam in CurrentKonfig.MainOptions)
            {
                if (String.IsNullOrEmpty(Mparam.By))
                {
                    switch (Mparam.name)
                    {
                        case "vendor_code":
                            product += "<offer id=\"" + sku + "\" available=\"true\" selling_type=\"r\">" + "\r\n" + "<vendorCode>" + sku + "</vendorCode>" + "\r\n";
                            product_to_List = "<product Id=\"" + sku + "\" url=\"" + Dr.Find("link[rel=canonical]").FirstElement().GetAttribute("href") + "\" c_id=\"" + SC.NameOurCategory + "\"></product>";
                            break;
                    }
                }
                else
                {
                    switch (Mparam.name)
                    {
                        case "product_name":
                            string TempName = WebUtility.HtmlDecode(Take(Dr, Mparam)).Trim();
                            if (productAU != null)
                            {
                                foreach(AutoCorrect CAU in productAU)
                                {
                                    if (String.IsNullOrEmpty(CAU.MetodsName))
                                    {
                                        switch (CAU.MetodsName)
                                        {
                                            
                                            case "AddToTop":
                                                TempName = TempName.Insert(0, CAU.name+" ");
                                                break;
                                            case "AddToEnd":
                                                TempName += CAU.name;
                                                    break;
                                            case "Replace":
                                                if (TempName.Contains(CAU.nameStartIndex))
                                                {
                                                    TempName = TempName.Replace(CAU.nameStartIndex, CAU.name);
                                                }
                                                break;
                                            case "ReplaceFrom":
                                                
                                                break;
                                            default:
                                                
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        
                                    }
                                }
                                
                            }
                            else
                            {
                               
                            }
                            product += "<name>" + TempName + "</name>" + "\r\n";
                            break;
                        case "description":
                            string TempDesc = WebUtility.HtmlDecode(Take(Dr, Mparam));

                            if (productAU != null)
                            {
                                foreach (AutoCorrect CAU in productAU)
                                {
                                    if (String.IsNullOrEmpty(CAU.MetodsName))
                                    {
                                        switch (CAU.MetodsDescription)
                                        {

                                            case "AddToTop":
                                                TempDesc = TempDesc.Insert(0, CAU.description + " ");
                                                break;
                                            case "AddToEnd":
                                                TempDesc += CAU.description;
                                                break;
                                            case "Replace":
                                                if (TempDesc.Contains(CAU.descriptionStartIndex))
                                                {
                                                    TempDesc = TempDesc.Replace(CAU.descriptionStartIndex, CAU.description);
                                                }
                                                break;
                                            case "ReplaceFrom":

                                                break;
                                            default:
                                                TempDesc = CAU.description;
                                                break;
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                            else
                            {

                                
                            }
                            product += "<description>" + TempDesc + "</description>" + "\r\n";
                            break;
                        case "main_picture":
                            foreach (var f in Directory.GetFiles(@"W:\inetpub\wwwroot\Photos\", sku + "*").ToList())
                            {
                                File.Delete(f);
                            }
                            string a = Take(Dr, Mparam);
                            if (!a.Contains("http"))
                            {
                                if (a[0].Equals("/"))
                                    a = a.Insert(0, domen);
                                else
                                {
                                    a = a.Insert(0, "/");
                                    a = a.Insert(0, domen);
                                }

                            }
                            webClient.DownloadFile(a, @"W:\inetpub\wwwroot\Photos\" + sku + @".jpg");
                            product += "<picture>http://sshop.com.ua/Photos/" + sku + ".jpg</picture>" + "\r\n";

                            break;
                        case "additional_picture":
                            int c = 1;
                            try
                            {
                                foreach (var r in TakeList(Dr, Mparam))
                                {

                                    string a_a = r;
                                    if (!a_a.Contains("http"))
                                    {
                                        if(a_a[0].Equals("/"))
                                        a_a = a_a.Insert(0, domen);
                                        else
                                        {
                                            a_a = a_a.Insert(0, "/");
                                            a_a = a_a.Insert(0, domen);
                                        }
                                        
                                    }

                                    webClient.DownloadFile(a_a, @"W:\inetpub\wwwroot\Photos\" + sku + "_" + c + @".jpg");
                                    product += "<picture>http://sshop.com.ua/Photos/" + sku + "_" + c + ".jpg</picture>" + "\r\n";
                                    c++;
                                }
                            }
                            catch { }
                            break;
                        case "product_code":

                            break;
                        case "vendor":
                            product += "<vendor>" + WebUtility.HtmlDecode(Take(Dr, Mparam)) + "</vendor>" + "\r\n";
                            break;
                    }

                }
            }


            product += "<categoryId>" + SC.NameOurCategory + "</categoryId>" + "\r\n";
            product += "<portal_category_id>" + SC.PortalCategoryId + "</portal_category_id>" + "\r\n";

            if (!String.IsNullOrEmpty(SC.Vendor))
            {
                product += "<vendor>" + SC.Vendor + "</vendor>" + "\r\n";
            }


            string denomination = "";
            string appointment = "";
            List<string> group_prod = new List<string>();
            foreach (var Characteristic in SC.Characteristics.OrderBy(x => x.Group))
            {
                if (!String.IsNullOrEmpty(Characteristic.Main))
                {
                    bool triget = true;
                    List<string> n = new List<string>();
                    List<string> v = new List<string>();

                    foreach (var e in Dr.Find(Characteristic.Name))
                    {
                        n.Add(e.InnerText);
                    }
                    foreach (var e in Dr.Find(Characteristic.Value))
                    {
                        v.Add(e.InnerText);
                    }

                    if (n.Count != 0)
                    {
                        foreach (var el in n)
                        {


                            if (el.Contains("Назначение"))
                            {
                                appointment = v[n.IndexOf(el)];
                            }
                            if (n.Contains("Наименование"))
                            {
                                denomination = v[n.IndexOf(el)];
                            }

                            if (Characteristic.Exceptions.Count != 0)
                            {
                                foreach (var e in Characteristic.Exceptions)
                                {
                                    switch (e.name)
                                    {
                                        case "name":
                                            if (e.name.Equals(el)) triget = false;
                                            break;
                                        case "value":
                                            if (v[n.IndexOf(el)].Contains(e.value)) triget = false;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            if (!triget) continue;

                            product += "<p><param name=\"" + el + "\" unit=\"\">" + v[n.IndexOf(el)] + "</param></p>" + "\r\n";



                            if (CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) & x.shopvalue.Equals(v)) == null &
                                CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) & x.shopvalue.Equals(v)) == null)
                            {
                                CurrentMatchList.Add(new MatchList((CurrentMatchList.Count+1).ToString(), el, "", v[n.IndexOf(el)], ""));
                            }
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(Characteristic.NameBy))
                    {

                        if (String.IsNullOrEmpty(Characteristic.ValueBy))
                        {
                            string n = Characteristic.Name;
                            string v = Characteristic.Value;

                            if (n.Contains("Назначение"))
                            {
                                appointment = v;
                            }
                            if (n.Contains("Наименование"))
                            {
                                denomination = v;
                            }
                            if (v.Contains(",")) v.Replace(",", "|");
                            if (CurrentMatchList.FirstOrDefault(x => ((x.shopname.Equals(n) & x.shopvalue.Equals(v)) | (x.ourname.Equals(n) & x.ourvalue.Equals(v))) & !x.ourname.Equals("") & !x.ourvalue.Equals("")) != null)
                            {
                                n = CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) | x.ourname.Equals(n)).ourname;
                                v = CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) | x.ourname.Equals(n)).shopname;
                            }
                            product += "<p><param name=\"" + n + "\" unit=\"\">" + v + "</param></p>" + "\r\n";
                        }
                        else
                        {
                            if (Dr.Find(Characteristic.Value).Length > 0)
                            {
                                List<string> vel = new List<string>();

                                foreach (var e in Dr.Find(Characteristic.Value))
                                {
                                    vel.Add(e.InnerText);
                                }
                                for (int i = 0; i < vel.Count; i++)
                                {
                                    string pat = (@"\d:-\d:");
                                    if (Regex.IsMatch(vel[i], pat))
                                    {

                                        string Start = vel[i].Remove(vel[i].IndexOf("-"));
                                        string End = vel[i].Substring(vel[i].IndexOf("-") + 1);
                                        vel.Remove(vel[i]);
                                        for (int t = Convert.ToInt32(Start); t <= Convert.ToInt32(End); t++)
                                        {
                                            vel.Add(t.ToString());
                                        }
                                    }
                                }

                                if (Characteristic.Group)
                                {
                                    string tGProduct = product;
                                    foreach (var ch in vel)
                                    {

                                        string temp = tGProduct;
                                        bool triget = true;
                                        string n = Characteristic.Name;
                                        string v = "";

                                        v = ch.Trim();
                                        if (v.Contains("("))
                                        {
                                            v = v.Remove(v.IndexOf("("));
                                        }
                                        v = v.Trim();
                                        if (n.Contains("Назначение"))
                                        {
                                            appointment = v;
                                        }
                                        if (n.Contains("Наименование"))
                                        {
                                            denomination = v;
                                        }

                                        if (Characteristic.Exceptions.Count != 0)
                                        {
                                            foreach (var e in Characteristic.Exceptions)
                                            {
                                                switch (e.name)
                                                {
                                                    case "name":
                                                        if (e.name.Equals(n)) triget = false;
                                                        break;
                                                    case "value":
                                                        if (v.Contains(e.value)) triget = false;
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        if (!triget) continue;

                                        if (CurrentMatchList.FirstOrDefault(x => ((x.shopname.Equals(n) & x.shopvalue.Equals(v)) | (x.ourname.Equals(n) & x.ourvalue.Equals(v))) & !x.ourname.Equals("") & !x.ourvalue.Equals("")) != null)
                                        {
                                            n = CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) | x.ourname.Equals(n)).ourname;
                                            v = CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) | x.ourname.Equals(n)).shopname;
                                        }


                                        temp += "<p><param name=\"" + n + "\" unit=\"\">" + v + "</param></p>" + "\r\n";
                                        group_prod.Add(temp);

                                        if (CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) & x.shopvalue.Equals(v)) == null &
                                            CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) & x.shopvalue.Equals(v)) == null)
                                        {
                                            CurrentMatchList.Add(new MatchList((CurrentMatchList.Count + 1).ToString(),n, "", v, ""));
                                        }

                                    }
                                }
                                else
                                {
                                    foreach (var ch in vel)
                                    {

                                        string n = Characteristic.Name;
                                        string v = "";

                                        v = ch;

                                        if (n.Contains("Назначение"))
                                        {
                                            appointment = v;
                                        }
                                        if (n.Contains("Наименование"))
                                        {
                                            denomination = v;
                                        }

                                        if (CurrentMatchList.FirstOrDefault(x => ((x.shopname.Equals(n) & x.shopvalue.Equals(v)) | (x.ourname.Equals(n) & x.ourvalue.Equals(v))) & !x.ourname.Equals("") & !x.ourvalue.Equals("")) != null)
                                        {
                                            n = CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) | x.ourname.Equals(n)).ourname;
                                            v = CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) | x.ourname.Equals(n)).shopname;
                                        }
                                        product += "<p><param name=\"" + n + "\" unit=\"\">" + v + "</param></p>" + "\r\n";

                                        if (CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) & x.shopvalue.Equals(v)) == null &
                                            CurrentMatchList.FirstOrDefault(x => x.shopname.Equals(n) & x.shopvalue.Equals(v)) == null)
                                        {
                                            CurrentMatchList.Add(new MatchList((CurrentMatchList.Count + 1).ToString(),n, "", v, ""));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            string k_w = "";

           

            List<string> KeyId = new List<string>() { SC.NameOurCategory };
            while(true)
            {
                if (!String.IsNullOrEmpty(Categories.FirstOrDefault(x => x.id.Equals(KeyId.Last())).parentId))
                {
                    KeyId.Add(Categories.FirstOrDefault(x => x.id.Equals(KeyId.Last())).parentId);
                }
                else break;
            }

            foreach (var c in KeyId)
            {
                k_w += Categories.FirstOrDefault(x => x.id.Equals(c)).name + ", ";
            }



            if (!String.IsNullOrEmpty(denomination)) k_w += ", " + denomination;
            if (!String.IsNullOrEmpty(appointment)) k_w += ", " + appointment;

            string Key_string = "<keywords>" + k_w + "</keywords>";
            product += Key_string.Replace(", ,", "");
            if (group_prod.Count > 0)
            {


                foreach (string GP in group_prod)
                {
                    string temp = GP;
                    temp = temp.Replace("<offer id=\"" + sku, "<offer id=\"" + sku + "-" + (group_prod.IndexOf(GP) + 1).ToString());
                    temp = temp.Replace("selling_type=\"r\"", "selling_type =\"r\" group_id=\"" + sku + "\"");
                    temp += Key_string.Replace(", ,", "");
                    temp += "\r\n" + "</offer>" + "\r\n";
                    products += temp;
                }
            }
            else
            {
                product += "\r\n" + "</offer>" + "\r\n";
                products += product;
            }
            File.AppendAllText(path + @"\products.xml", products, Encoding.UTF8);
            File.AppendAllText(path + @"\products_list.xml", product_to_List, Encoding.UTF8);
            
            //FileInfo prod = new FileInfo(path + @"\products.xml");
            //if (prod.Exists != false)
            //{

            //    FileStream fs = prod.Create();
            //    Byte[] first = new UTF8Encoding(true).GetBytes(products);
            //    fs.Write(first, 0, first.Length);
            //    fs.Close();
            //}

            //FileInfo prodList = new FileInfo(path + @"\products_list.xml");
            //if (prodList.Exists != false)
            //{


            //    FileStream fs = prodList.Create();
            //    Byte[] first = new UTF8Encoding(true).GetBytes(product_to_List);
            //    fs.Write(first, 0, first.Length);
            //    fs.Close();
            //}

        }


        static public string Take(CQ Dr, MainOption Mparam)
        {
            string req = "";
            var a = CQ.Create();
            switch (Mparam.By)
            {
                //case "class":
                //    switch (Mparam.take)
                //    {
                //        case "Text":
                //            if (Dr.FindElement(By.ClassName(Mparam.value)) != null) req = Dr.FindElement(By.ClassName(Mparam.value)).Text;
                //             break;
                //        default:
                //            if (Dr.FindElement(By.ClassName(Mparam.value)) != null)  req = Dr.FindElement(By.ClassName(Mparam.value)).GetAttribute(Mparam.take);
                //            break;


                //    }
                //    break;
                //case "id":
                //    switch (Mparam.take)
                //    {
                //        case "Text":
                //            if (Dr.FindElement(By.Id(Mparam.value)) != null)  req = Dr.FindElement(By.Id(Mparam.value)).Text;
                //            break;
                //        default:
                //            if (Dr.FindElement(By.Id(Mparam.value)) != null)  req = Dr.FindElement(By.Id(Mparam.value)).GetAttribute(Mparam.take);
                //            break;
                //    }
                //    break;
                //case "XPath":
                //    switch (Mparam.take)
                //    {
                //        case "Text":
                //            var a = Dr.FindElement(By.XPath(Mparam.value));
                //            if (a != null)
                //            {
                //                req = a.GetAttribute("innerText");
                //            }
                //            break;
                //        default:
                //            var av = Dr.FindElement(By.XPath(Mparam.value));
                //            if (av != null)
                //            {
                //                req = av.GetAttribute(Mparam.take);
                //            }
                //            break;
                //    }
                //    break;
                case "Css":
                    switch (Mparam.take)
                    {
                        case "Text":
                            a = Dr.Find(Mparam.value);
                            if (a != null)
                            {
                                foreach (var r in a)
                                {
                                    req += r.InnerText;
                                }

                            }
                            break;
                        default:
                            a = Dr.Find(Mparam.value);
                            if (a != null)
                            {
                                foreach (var r in a)
                                {
                                    req = r.GetAttribute(Mparam.take);
                                }

                            }
                            break;
                    }
                    break;
            }

            return req;
        }


        static public List<string> TakeList(CQ Dr, MainOption Mparam)
        {
            var a = CQ.Create();
            List<string> req = new List<string>();
            switch (Mparam.By)
            {
                //case "class":
                //    switch (Mparam.take)
                //    {
                //        case "Text":
                //            foreach (var a in Dr.FindElements(By.ClassName(Mparam.value))) { req.Add(a.Text); }
                //            break;

                //        default:
                //            foreach (var a in Dr.FindElements(By.ClassName(Mparam.value))) { req.Add(a.GetAttribute(Mparam.take)); }
                //            break;

                //    }
                //    break;
                //case "id":
                //    switch (Mparam.take)
                //    {
                //        case "Text":
                //            foreach (var a in Dr.FindElements(By.Id(Mparam.value))) { req.Add(a.Text); }

                //            break;
                //        default:
                //            foreach (var a in Dr.FindElements(By.Id(Mparam.value))) { req.Add(a.GetAttribute(Mparam.take)); }
                //            break;
                //    }
                //    break;
                //case "XPath":
                //    switch (Mparam.take)
                //    {
                //        case "Text":

                //            foreach (var a in Dr.FindElements(By.XPath(Mparam.value))) { req.Add(a.Text); }
                //            break;
                //        default:
                //            foreach (var a in Dr.FindElements(By.XPath(Mparam.value))) { req.Add(a.GetAttribute(Mparam.take)); }
                //            break;
                //    }
                //    break;

                case "Css":
                    a = Dr.Find(Mparam.value);
                    switch (Mparam.take)
                    {
                        case "Text":

                            foreach (var el in a) { req.Add(el.InnerText.Trim()); }
                            break;
                        default:
                            foreach (var el in a)
                            {
                                req.Add(el.GetAttribute(Mparam.take).Trim());
                            }
                            break;
                    }
                    break;

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



    public class Konfig
    {
        public string ShopName { get; set; }
        public List<MainOption> MainOptions { get; set; }
        public List<Category> Categores { get; set; }
        public List<Characteristic> MainCharacteristics { get; set; }
    }

    public class MainOption
    {

        public string name { get; set; }
        public string By { get; set; }
        public string take { get; set; }
        public string value { get; set; }
        public MainOption(string name, string By, string take, string value)
        {
            this.name = name;
            this.By = By;
            this.value = value;
            this.take = take;
        }
        public MainOption() { }
    }
    public class Category
    {
        public string id { get; set; }
        public string url { get; set; }
        public List<SubCatecory> SubCatecories { get; set; }
        public Category() { }
        public Category(int id, string url, List<SubCatecory> SubCatecories)
        {
            this.id = id.ToString();
            this.url = url;
            this.SubCatecories = SubCatecories;
        }

    }

    public class SubCatecory
    {
        public string NameOurCategory { get; set; }
        public string PortalCategoryId { get; set; }
        public string discount { get; set; }
        public string price { get; set; }
        public string price_processing { get; set; }
        public string СombinationName { get; set; }
        public bool СombinationValue { get; set; }
        public string Vendor { get; set; }
        public List<Characteristic> Characteristics { get; set; }
        public bool EnableMainCharacteristics { get; set; }


        public SubCatecory() { }
        public SubCatecory(string NameOurCategory, string PortalCategoryId, string discount, string price, string price_processing, string СombinationName, string Vendor, bool СombinationValue, bool EnableMainCharacteristics, List<Characteristic> Characteristics)
        {
            this.NameOurCategory = NameOurCategory;
            this.PortalCategoryId = PortalCategoryId;
            this.discount = discount;
            this.price = price;
            this.price_processing = price_processing;
            this.СombinationName = СombinationName;
            this.Vendor = Vendor;
            this.СombinationValue = СombinationValue;
            this.EnableMainCharacteristics = EnableMainCharacteristics;
            this.Characteristics = Characteristics;
        }

        public string ToString(SubCatecory SB, List<Characteristic> LC)
        {
            string SBs = "";
            if (SB.EnableMainCharacteristics)
            {
                foreach (var c in LC) SB.Characteristics.Add(c);
            }

            string ch = "";
            foreach (var c in SB.Characteristics) ch += c.ToString(c);


            SBs += "<SubCatecory>\r\n" +
                        String.Format("<name_our_categori>{0}</name_our_categori>\r\n", SB.NameOurCategory) +
                         String.Format("<PortalCategoryId>{0}</PortalCategoryId>\r\n", SB.PortalCategoryId) +
                        String.Format("<discount>{0}</discount>\r\n", SB.discount) +
                        String.Format("<price>{0}</price>\r\n", SB.price) +
                        String.Format("<price_processing>{0}</price_processing>\r\n", SB.price_processing) +
                        String.Format("<combination value=\"{0}\">{1}</combination>\r\n", SB.СombinationValue.ToString(), SB.СombinationName) +
                        ch +
                        String.Format("<vendor>{0}</vendor>\r\n", SB.Vendor) +
                        "</SubCatecory>\r\n"
                        ;

            return SBs;
        }

    }
    public class Characteristic
    {
        public string id { get; set; }
        public bool Group { get; set; }
        public string Main { get; set; }
        public string MainBy { get; set; }
        public string Name { get; set; }
        public string NameBy { get; set; }
        public string Value { get; set; }
        public string ValueBy { get; set; }
        public List<Except> Exceptions { get; set; }

        public Characteristic() { }
        public Characteristic(string id, bool Group, string Main, string MainBy, string Name, string NameBy, string Value, string ValueBy, List<Except> Exceptions)
        {
            this.id = id;
            this.Group = Group;
            this.Main = Main;
            this.MainBy = MainBy;
            this.Name = Name;
            this.NameBy = NameBy;
            this.Value = Value;
            this.ValueBy = ValueBy;
            this.Exceptions = Exceptions;
        }
        public string ToString(Characteristic Ch)
        {
            string req = "";
            string te = "";
            foreach (var ex in Ch.Exceptions)
            {
                te += ex.ToString(ex);
            }
            req += String.Format("<characteristics group=\"{0}\">\r\n", Ch.Group.ToString()) +
                String.Format("<main By=\"{0}\">{1}</main>\r\n", Ch.MainBy, Ch.Main) +
                String.Format("<name By=\"{0}\">{1}</name>\r\n", Ch.NameBy, Ch.Name) +
                String.Format("<value By=\"{0}\">{1}</value>\r\n", Ch.ValueBy, Ch.Value) +
                te +
                "</characteristics>\r\n";
            return req;
        }
    }
    public class Except
    {
        public string name { get; set; }
        public string value { get; set; }
        public Except() { }
        public Except(string name, string value) { this.name = name; this.value = value; }
        public string ToString(Except ex)
        {
            return String.Format("<exception name=\"{0}\">{1}</exception>\r\n", ex.name, ex.value);
        }
    }


    public class Categories
    {
        public string name { get; set; }
        public string id { get; set; }
        public string parentId { get; set; }
        public bool repl { get; set; }
        public Categories() { }
        public Categories(string name, string id, string parentId, bool repl) { this.name = name; this.id = id; this.parentId = parentId; this.repl = repl; }
    }

    public class AutoCorrect
    {
        public string id { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public string MetodsName { get; set; }
        public string nameStartIndex { get; set; }
        public string nameEndIndex { get; set; }
        public string description { get; set; }
        public string MetodsDescription { get; set; }
        public string descriptionStartIndex { get; set; }
        public string descriptionEndIndex { get; set; }
        public string group { get; set; }

        public AutoCorrect() { }
        public AutoCorrect(string id, string sku, string name, string MetodsName, string nameStartIndex, string nameEndIndex, string description, string MetodsDescription, string descriptionStartIndex, string descriptionEndIndex, string group)
        {
            this.id = id;
            this.sku = sku;
            this.name = name;
            this.MetodsName = MetodsName;
            this.nameStartIndex = nameStartIndex;
            this.nameEndIndex = nameEndIndex;
            this.description = description;
            this.MetodsDescription = MetodsDescription;
            this.descriptionStartIndex = descriptionStartIndex;
            this.descriptionEndIndex = descriptionEndIndex;
            this.group = group;
        }
    }

    public class MatchList
    {
        public string id { get; set; }
        public string shopname { get; set; }
        public string ourname { get; set; }
        public string shopvalue { get; set; }
        public string ourvalue { get; set; }
        public MatchList() { }
        public MatchList(string id, string shopname, string ourname, string shopvalue, string ourvalue)
        {
            this.id = id;
            this.shopname = shopname;
            this.ourname = ourname;
            this.shopvalue = shopvalue;
            this.ourvalue = ourvalue;
        }
        public string ToString(Except ex)
        {
            return String.Format("<exception name=\"{0}\">{1}</exception>\r\n", ex.name, ex.value);
        }
    }

    public class ProductList
    {
        public string sku { get; set; }
        public string url { get; set; }
        
        public ProductList() { }
        public ProductList(string sku, string url)
        {
            this.sku = sku;
            this.url = url;
            
        }        
    }


}
