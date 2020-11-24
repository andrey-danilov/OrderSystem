using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using report.Models;
using System.Web.Script.Serialization;

namespace report.Controllers
{
    public class ProductParsApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ProductParsApi
        public async Task<List<ProductPars>> GetProductsPars()
        {
            List<ProductPars> Products = await db.ProductsPars.Where(x => x.ShopCode.Equals("96860cfe-2c54-4e6d-8204-e5fcacb3113f") & x.Relevance).Include(x => x.Parameters).Include(x => x.Pictures).ToListAsync();

            //var Par = await db.Parameterses.Include(x => x.ProductsPars).ToListAsync();
            //var Pic = await db.Pictures.Include(x => x.ProductsPars).ToListAsync();
            
            return Products;
        }
    }
}