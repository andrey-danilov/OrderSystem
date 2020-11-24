using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace report.Models
{
    public class TempProductList
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string VendorCode { get; set; }
        public short Quantity { get; set; }
        public string Name { get; set; }
        public TempProductList() { }
        public TempProductList(string UserId, string VendorCode, short Quantity, string Name)
        {
            Id = Guid.NewGuid().ToString();
            this.UserId = UserId;
            this.VendorCode = VendorCode;
            this.Quantity = Quantity;
            this.Name = Name;
        }
    }
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string KeyWorsd { get; set; }
        public string vendor { get; set; }
        public string vendorCode { get; set; }
        public int oldPrice { get; set; }
        public int Price { get; set; }
        public List<Parameters> ProductParameters { get; set; }
        public string Url { get; set; }
        public short Quantity { get; set; }
        public Product() { }
        public Product(string vendorCode, string Url, int Price, string Name, string Id)
        {
            this.vendorCode = vendorCode;
            this.Url = Url;
            this.Price = Price;
            this.Name = Name;
            this.Id = Id;
            this.Quantity = 1;
        }
        public Product(string vendorCode, string Url, int Price, int oldPrice, string Id, string Name, string KeyWorsd, string vendor)
        {
            this.vendorCode = vendorCode;
            this.Url = Url;
            this.Price = Price;
            this.oldPrice = oldPrice;
            this.Name = Name;
            this.Quantity = 1;
            this.Id = Guid.NewGuid().ToString();
            this.KeyWorsd = KeyWorsd;
            this.vendor = vendor;
        }






    }
    public class Parameters
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Group { get; set; }
        //public ProductPars ProductPars { get; set; }
        public string IdProd { get; set; }
        public List<ProductPars> ProductsPars { get; set; }
        public Parameters() {/* this.Id = Guid.NewGuid().ToString();*/ }
        public Parameters(string Name, string Value, bool Group)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = Name;
            this.Value = Value;
            this.Group = Group;
        }
    }
    
    public class OrderProm
    {
        public string Id { get; set; }
        public string Id_User { get; set; }
        public bool FromShop { get; set; }
        public string Order_Number { get; set; }
        public string ShopNumber { get; set; }
        public string Phone { get; set; }
        public string Data { get; set; }
        public string TotalPrice { get; set; }
        public string DeliveryInfo { get; set; }
        public string WaybillNumber { get; set; }
        public string Traking { get; set; }
        public int TrakingStatus { get; set; }
        public List<ProductProm> Products { get; set; }
        public List<Status> Statuses { get; set; }
        public List<Note> Notes { get; set; }

        public short number { get; set; }
        public OrderProm(string Id_User, string Order_Number, string Phone, string Data,
            string TotalPrice,
            string DeliveryInfo, short number)
        {
            Id = Guid.NewGuid().ToString();
            this.Id_User = Id_User;
            this.Order_Number = Order_Number;
            this.Phone = Phone;
            this.Data = Data;
            this.TotalPrice = TotalPrice;
            this.DeliveryInfo = DeliveryInfo;
            this.number = number;

        }
        public OrderProm()
        {
            Products = new List<ProductProm>();
            Statuses = new List<Status>();
            Notes = new List<Note>();
        }
    }
    public class ProductProm
    {
        public string Id { get; set; }
        public string VendorCode { get; set; }
        public string VendorUrl { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

        public ProductProm()
        {

        }

        public ProductProm(string VendorCode, string VendorUrl, string Price, string Quantity, string Url, string Name)
        {
            Id = Guid.NewGuid().ToString();
            this.VendorCode = VendorCode;
            this.VendorUrl = VendorUrl;
            this.Price = Price;
            this.Quantity = Quantity;
            this.Price = Price;
            this.Url = Url;
            this.Name = Name;
        }
        

        public ProductProm(string Id, string VendorCode, string VendorUrl, string Price, string Quantity, string Url, string Name)
        {
            this.Id = Id;
            this.VendorCode = VendorCode;
            this.VendorUrl = VendorUrl;
            this.Price = Price;
            this.Quantity = Quantity;
            this.Price = Price;
            this.Url = Url;
            this.Name = Name;
        }

        public ProductProm(string id)
        {
            this.Id = id;

        }
    }
    public class Status
    {
        public string Id { get; set; }
        public string StatusName { get; set; }
        public bool flag { get; set; }
        public short number { get; set; }

        public Status() { }
        public Status(string StatusName, bool flag, short number)
        {
            this.Id = Guid.NewGuid().ToString();
            this.StatusName = StatusName;
            this.flag = flag;
            this.number = number;
        }
    }
    public class Note
    {
        public string Id { get; set; }
        public string NoteName { get; set; }
        public bool flag { get; set; }
        public short number { get; set; }
        public OrderProm OrderProm { get; set; }
        public Note()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
    public class UserStatus
    {
        public string Id { get; set; }
        public string StatusName { get; set; }
        public bool DefoltFlag { get; set; }
        public short number { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
    public class UserNote
    {
        public string Id { get; set; }
        public string StatusName { get; set; }

        public short number { get; set; }

    }
    public class Konfig
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ShopName { get; set; }
        public string VendorCode { get; set; }
        public bool selenium { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string Phone { get; set; }
        public bool Running { set; get; }
        public bool InQueue { set; get; }
        public bool ToFile { set; get; }

        public ProductUrl ProductUrl { get; set; }
        public Pagination Pagination { get; set; }
        public Availability Availability { get; set; }
        public List<Denomination> Denominations { get; set; }
        public List<Description> Descriptions { get; set; }
        public PriceLow PriceLow { get; set; }
        public PriceHigh PriceHigh { get; set; }
        public MainPicture MainPicture { get; set; }
        public List<AdditionalPicture> AdditionalPictures { get; set; }
        public Vendor Vendor { get; set; }
        public Vendor VendorSearch { get; set; }
        public List<Except> Excepts { get; set; }
        public List<Category> Categores { get; set; }
        public List<Characteristic> MainCharacteristics { get; set; }
        public List<Contacts> Contacts { set; get; }
        public Konfig()
        {
            //this.ProductUrl = new ProductUrl();
            //this.Pagination = new Pagination();
            //this.Availability = new Availability();
            //this.Denominations = new List<Denomination>();
            //this.Descriptions = new List<Description>();
            //this.PriceLow = new PriceLow();
            //this.PriceHigh = new PriceHigh();
            //this.MainPicture = new MainPicture();
            //this.AdditionalPictures = new List<AdditionalPicture>();
            //this.Vendor = new Vendor();
            //this.Excepts = new List<Except>();
            //this.Categores = new List<Category>();
            //this.MainCharacteristics = new List<Characteristic>();
        }



        public Konfig(string name, string UserId)
        {
            this.Id = Guid.NewGuid().ToString();
            this.ShopName = name;
            this.UserId = UserId;
            this.ProductUrl = new ProductUrl(false);
            this.Pagination = new Pagination(false);
            this.Availability = new Availability(false);
            this.Denominations = new List<Denomination>() { new Denomination(false) };
            this.Descriptions = new List<Description>() { new Description(false) };
            this.PriceLow = new PriceLow(false);
            this.PriceHigh = new PriceHigh(false);
            this.MainPicture = new MainPicture(false);
            this.AdditionalPictures = new List<AdditionalPicture>() { new AdditionalPicture(false) };
            this.Vendor = new Vendor(false);
            this.VendorSearch = new Vendor(false);
            this.Excepts = new List<Except>() { new Except(false) };
            this.Categores = new List<Category>() { new Category(false) };
            this.MainCharacteristics = new List<Characteristic>() { new Characteristic(false) };
            this.Running = false;
        }
    }
    public class Contacts
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public Konfig Konfig { get; set; }
    }
    public class ProductUrl
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }

        public CheakPrise CheakPrise { get; set; }
        public Availability Availability { get; set; }
        public ProductUrl()
        { }
        public ProductUrl(bool f)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
            this.CheakPrise = new CheakPrise(false);
            this.Availability = new Availability(false);
        }


    }
    public class CheakPrise
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public CheakPrise()
        {
        }
        public CheakPrise(bool f)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
        }
    }
    public class Availability
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public string expression { get; set; }


        public Availability()
        { }
        public Availability(bool f)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
            this.expression = "";
        }
    }
    public class Pagination
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }

        public Pagination()
        { }
        public Pagination(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
        }
    }
    public class Denomination
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public Konfig Konfig { get; set; }
        public Denomination()
        {
        }
        public Denomination(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
        }
    }
    public class Description
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public Konfig Konfig { get; set; }
        public Description()
        { }
        public Description(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
        }
    }
    public class PriceLow
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public string expression { get; set; }
        public bool coins { get; set; }
        public PriceLow() { }
        public PriceLow(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
            this.expression = "";
            this.coins = false;
        }
    }
    public class PriceHigh
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public string expression { get; set; }
        public bool coins { get; set; }
        public PriceHigh() { }
        public PriceHigh(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
            this.expression = "";
            this.coins = false;
        }
    }
    public class MainPicture
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public string format { get; set; }
        public string empty { get; set; }

        public MainPicture()
        { }
        public MainPicture(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
            this.format = "";
            this.empty = "";
        }
    }
    public class AdditionalPicture
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public string format { get; set; }
        public string empty { get; set; }
        public Konfig Konfig { get; set; }
        public AdditionalPicture()
        { }
        public AdditionalPicture(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
            this.format = "";
            this.empty = "";
        }
    }
    public class Vendor
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }

        public Vendor()
        { }
        public Vendor(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";

        }
    }

    public class Except
    {
        public string Id { get; set; }
        public string take { get; set; }
        public string by { get; set; }
        public string value { get; set; }
        public string comparison { get; set; }
        public Konfig Konfig { get; set; }
        public Except()
        { }
        public Except(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.take = "";
            this.by = "";
            this.value = "";
            this.comparison = "";
        }
    }
    public class Category
    {
        public string Id { get; set; }
        public int num { get; set; }
        public string url { get; set; }
        public virtual Konfig Konfig { get; set; }

        public virtual List<SubCatecory> SubCatecories { get; set; }
        public Category()
        { }
        public Category(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.num = 0;
            this.url = "";
            this.SubCatecories = new List<SubCatecory>() { new SubCatecory(false) };
        }
        public Category(int num, string url, List<SubCatecory> SubCatecories)
        {
            this.Id = Guid.NewGuid().ToString();
            this.num = num;
            this.url = url;
            this.SubCatecories = SubCatecories;
        }

    }

    public class SubCatecory
    {
        public string Id { get; set; }
        public string NameOurCategory { get; set; }
        public string PortalCategoryId { get; set; }
        public string discount { get; set; }
        public string ShippingInUS { get; set; }
        public string ShippingToUkraine { get; set; }
        public string price { get; set; }
        public string price_processing { get; set; }
        public string fee { get; set; }
        public string MoneyTransfer { get; set; }
        public string CustomTaxes { get; set; }
        public string СombinationName { get; set; }
        public bool СombinationValue { get; set; }
        public string Vendor { get; set; }
        public string VendorSearch { get; set; }
        public virtual List<Characteristic> Characteristics { get; set; }
        public virtual List<KeyWord> KeyWord { get; set; }
        public bool EnableMainCharacteristics { get; set; }
        public Konfig Konfig { get; set; }
        public virtual Category Category { get; set; }
        public SubCatecory()
        { }
        public SubCatecory(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.NameOurCategory = "";
            this.PortalCategoryId = "";
            this.discount = "";
            this.price = price;
            this.price_processing = "";
            this.СombinationName = "";
            this.Vendor = "";
            this.VendorSearch = "";
            this.СombinationValue = false;
            this.EnableMainCharacteristics = false;
            this.Characteristics = new List<Characteristic>() { new Characteristic(false) };
        }
        public SubCatecory(string NameOurCategory, string PortalCategoryId, string discount, string price, string price_processing, string СombinationName, string Vendor, bool СombinationValue, bool EnableMainCharacteristics, List<Characteristic> Characteristics)
        {
            this.Id = Guid.NewGuid().ToString();
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

            if (String.IsNullOrEmpty(SB.price)) SB.price = "all";
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
        public string Id { get; set; }
        public bool Group { get; set; }
        public string Main { get; set; }
        public string MainBy { get; set; }
        public string Name { get; set; }
        public string NameBy { get; set; }
        public string Value { get; set; }
        public string ValueBy { get; set; }
        public virtual Konfig Konfig { get; set; }
        public virtual List<ExceptCH> Exceptions { get; set; }
        public virtual SubCatecory SubCatecory { get; set; }
        public Characteristic()
        { }
        public Characteristic(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Group = false;
            this.Main = "";
            this.MainBy = "";
            this.Name = "";
            this.NameBy = "";
            this.Value = "";
            this.ValueBy = "";
            this.Exceptions = new List<ExceptCH>() { new ExceptCH(false) };
        }
        public Characteristic(bool Group, string Main, string MainBy, string Name, string NameBy, string Value, string ValueBy, List<ExceptCH> Exceptions)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Group = Group;
            this.Main = Main;
            this.MainBy = MainBy;
            this.Name = Name;
            this.NameBy = NameBy;
            this.Value = Value;
            this.ValueBy = ValueBy;
            this.Exceptions = Exceptions;
        }
        public Characteristic(Characteristic ob)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Group = ob.Group;
            this.Main = ob.Main;
            this.MainBy = ob.MainBy;
            this.Name = ob.Name;
            this.NameBy = ob.NameBy;
            this.Value = ob.Value;
            this.ValueBy = ob.ValueBy;
            this.Exceptions = new List<ExceptCH>();
            foreach (ExceptCH ex in ob.Exceptions)
            {
                ex.Id = Guid.NewGuid().ToString();
                this.Exceptions.Add(ex);
            }
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
    public class ExceptCH
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public virtual Characteristic Characteristic { get; set; }
        public ExceptCH()
        {
        }
        public ExceptCH(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.name = "";
            this.value = "";

        }
        public ExceptCH(string name, string value) { this.name = name; this.value = value; }
        public string ToString(ExceptCH ex)
        {
            return String.Format("<exception name=\"{0}\">{1}</exception>\r\n", ex.name, ex.value);
        }
    }
    public class ShopCategories
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string name { get; set; }
        public string NumCat { get; set; }
        public string parentNum { get; set; }
        public bool repl { get; set; }
        public ShopCategories() { }
        public ShopCategories(string name, string UserId, string parentNum, string NumCat, bool repl)
        {
            this.Id = Guid.NewGuid().ToString();
            this.UserId = UserId;
            this.name = name;
            this.NumCat = NumCat;
            this.parentNum = parentNum;
            this.repl = repl;
        }
        public ShopCategories(bool a)
        {
            this.Id = Guid.NewGuid().ToString();
            this.UserId = "";
            this.name = "";
            this.NumCat = "";
            this.parentNum = "";
            this.repl = false;
        }
    }
    public class AutoCorrect
    {
        public string id { get; set; }
        public string ShopName { get; set; }
        public string UserId { get; set; }
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
        public AutoCorrect(bool a) { this.id = Guid.NewGuid().ToString(); }
        public AutoCorrect(string sku, string name, string MetodsName, string nameStartIndex, string nameEndIndex, string description, string MetodsDescription, string descriptionStartIndex, string descriptionEndIndex, string group)
        {
            this.id = Guid.NewGuid().ToString();
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
        public string ToString(ExceptCH ex)
        {
            return String.Format("<exception name=\"{0}\">{1}</exception>\r\n", ex.name, ex.value);
        }
    }

    public class VendorsName
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class ProductPars
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public bool Relevance { get; set; }
        public string ShopCode { get; set; }
        public string VendorCode { get; set; }
        public string OurCategori { get; set; }
        public string PortalCategoryId { get; set; }
        public string Price { get; set; }
        public string PriceOld { get; set; }
        public string Url { get; set; }
        public string Vendor { get; set; }
        public string Denomination { get; set; }
        public string Description { get; set; }
        public string KeyWords { get; set; }
        public string Errors { get; set; }
        public string CatNumber { get; set; }
        public string CatUrl { get; set; }
        public string GroupSKU { get; set; }
        public string Phone { get; set; }
        public List<Picture> Pictures { get; set; }
        public List<Parameters> Parameters { get; set; }

        public ProductPars()
        {

        }

        public ProductPars(string UserId, string ShopCode, string VendorCode, string OurCategori, string PortalCategoryId, string Vendor, string Price, string PriceOld, string Url, string Denomination, string Description, string KeyWords, List<Picture> Pictures, List<Parameters> Parameters)
        {
            Id = Guid.NewGuid().ToString();
            this.UserId = UserId;
            this.ShopCode = ShopCode;
            this.VendorCode = VendorCode;
            this.OurCategori = OurCategori;
            this.PortalCategoryId = PortalCategoryId;
            this.Price = Price;
            this.PriceOld = PriceOld;
            this.Url = Url;
            this.Vendor = Vendor;
            this.Denomination = Denomination;
            this.Description = Description;
            this.KeyWords = KeyWords;
            this.Pictures = Pictures;
            this.Parameters = Parameters;
        }
    }
    public class Picture
    {
        public string id { get; set; }
        public string url { get; set; }
        //public ProductPars ProductPars { get; set; }
        public string IdProd { get; set; }
        public List<ProductPars> ProductsPars { get; set; }
        public Picture() { }
        public Picture(string url)
        {
            this.id = Guid.NewGuid().ToString();
            this.url = url;


        }

    }
    public class KeyWord
    {
        public string Id { get; set; }
        public bool CheakVendor { get; set; }
        public string Name { get; set; }
        public bool StringFormat { get; set; }
        public virtual SubCatecory SubCatecory { get; set; }
    }


    public class Cart
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public ProductPars Product { get; set; }
        public short Quantity { get; set; }
        public bool InOrder { get; set; }
        
    }

    public class Order
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string MailOffice { get; set; }
        public string City { get; set; }
        public DateTime DateOfFormation { get; set; }
        //public DateTime DateOfPaying { get; set; }
        public List<Cart> CurrentCarts { get; set; }
        public double Amount { get; set; }
        //public bool Papayed { get; set; }
        public string Tracking { get; set; }
        public string TrackingNumber { get; set; }
        public int Index { get; set; }
        //public string Message { get; set; }
        public List<Status> Statuses { get; set; }
        public List<Note> Notes { get; set; }
    }
    
}