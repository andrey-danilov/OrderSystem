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
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace report.Controllers
{
    public class ProductParsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ProductPars
        [HttpGet]
        public IEnumerable<ProductPars> GetProductsPars()
        {
            
            return db.ProductsPars.Where(x => x.Relevance).ToList();
        }

        // GET: api/ProductPars/5
        [ResponseType(typeof(ProductPars))]
        public async Task<IHttpActionResult> GetProductPars(string id)
        {
            ProductPars productPars = await db.ProductsPars.FindAsync(id);
            if (productPars == null)
            {
                return NotFound();
            }

            return Ok(productPars);
        }

        // PUT: api/ProductPars/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProductPars(string id, ProductPars productPars)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productPars.Id)
            {
                return BadRequest();
            }

            db.Entry(productPars).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductParsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ProductPars
        [ResponseType(typeof(ProductPars))]
        public async Task<IHttpActionResult> PostProductPars(ProductPars productPars)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProductsPars.Add(productPars);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductParsExists(productPars.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = productPars.Id }, productPars);
        }

        // DELETE: api/ProductPars/5
        [ResponseType(typeof(ProductPars))]
        public async Task<IHttpActionResult> DeleteProductPars(string id)
        {
            ProductPars productPars = await db.ProductsPars.FindAsync(id);
            if (productPars == null)
            {
                return NotFound();
            }

            db.ProductsPars.Remove(productPars);
            await db.SaveChangesAsync();

            return Ok(productPars);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductParsExists(string id)
        {
            return db.ProductsPars.Count(e => e.Id == id) > 0;
        }
        


        

    }
}