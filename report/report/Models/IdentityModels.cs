using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace report.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string PromLogin { get; set; }
        public string PromPassword { get; set; }
        public ICollection<UserStatus> UStatuses { get; set; }
        public ICollection<UserNote> UNotes { get; set; }
        public List<Product> Products { get; set; }

        public List<PromExport> PromExports { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class PromExport
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }



    public class SearchOrderProm
    {
        public string Order_Number { get; set; }
        public string ShopNumber { get; set; }
        public string Phone { get; set; }
        public string TotalPrice { get; set; }
        public string DeliveryInfo { get; set; }
        public string WaybillNumber { get; set; }
        public string Note { get; set; }
        public string ProductPrice { get; set; }
        public string ProductName { get; set; }
        public SearchOrderProm(string Order_Number, string Phone, DateTime FirstData, DateTime SecondtData,
            string TotalPrice,
            string DeliveryInfo)
        {
            this.Order_Number = Order_Number;
            this.Phone = Phone;
            
            this.TotalPrice = TotalPrice;
            this.DeliveryInfo = DeliveryInfo;
        }

        public SearchOrderProm()
        {

        }
    }
    

    public class ScrapingXML
    {
        public string OldCat { get; set; }
        public string OurCat { get; set; }
        public string PromCat { get; set; }
        public string ExtraCharge { get; set; }
        public string Vendor { get; set; }
        public Dictionary<string, string> par { get; set; }
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ProductProm> ProductsProm { get; set; }
        public DbSet<OrderProm> OP { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<UserStatus> UserStatuses { get; set; }
        public DbSet<UserNote> UserNotes { get; set; }
        public DbSet<Parameters> Parameterses { get; set; }
        public DbSet<TempProductList> TempProductLists { get; set; }
        public DbSet<ProductPars> ProductsPars { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Konfig> KonfigVe { get; set; }
        public DbSet<ProductUrl> ProductUrls { get; set; }
        public DbSet<CheakPrise> CheakPrises { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Pagination> Paginations { get; set; }
        public DbSet<Denomination> Denominations { get; set; }
        public DbSet<Description> Descriptions { get; set; }
        public DbSet<PriceLow> PriceLows { get; set; }
        public DbSet<PriceHigh> PriceHighs { get; set; }
        public DbSet<MainPicture> MainPictures { get; set; }
        public DbSet<AdditionalPicture> AdditionalPictures { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Except> Excepts { get; set; }
        public DbSet<Category> CategoryKonfig { get; set; }
        public DbSet<SubCatecory> SubCatecory { get; set; }
        public DbSet<Characteristic> Characteristics { get; set; }
        public DbSet<ExceptCH> ExceptCH { get; set; }
        public DbSet<AutoCorrect> AutoCorrectList { get; set; }
        public DbSet<VendorsName> VendorsNames { get; set; }
        public DbSet<KeyWord> KeyWords { get; set; }
        public DbSet<ShopCategories> ShopCategories { get; set; }
        public DbSet<PromExport> PromExports { get; set; }
        public ApplicationDbContext()
            : base("ReportConnection", throwIfV1Schema: false)
        {
          // this.Configuration.LazyLoadingEnabled = true;
        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<report.Models.Product> Products { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<Category>()
        //    //    .HasMany(p => p.SubCatecories)
        //    //    .WithRequired(p => p.Category)
        //    //    .WillCascadeOnDelete(true);            
        //    base.OnModelCreating(modelBuilder);
        //}
    }
    




}