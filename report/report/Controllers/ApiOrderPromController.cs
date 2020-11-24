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

namespace report.Controllers
{
    public class ApiOrderPromController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ApiOrderProm
        public IQueryable<OrderProm> GetOP()
        {
            return db.OP;
        }

        // GET: api/ApiOrderProm/5
        [ResponseType(typeof(OrderProm))]
        public async Task<IHttpActionResult> GetOrderProm(string id)
        {
            OrderProm orderProm = await db.OP.FindAsync(id);
            if (orderProm == null)
            {
                return NotFound();
            }

            return Ok(orderProm);
        }

        // PUT: api/ApiOrderProm/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrderProm(string id, OrderProm orderProm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderProm.Id)
            {
                return BadRequest();
            }

            db.Entry(orderProm).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderPromExists(id))
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

        // POST: api/ApiOrderProm
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> PostOrderProm(string order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            OrderProm orderProm = new OrderProm();
            //orderProm.Id = order.Id;
            //orderProm.Id_User = Guid.NewGuid().ToString();
            //orderProm.Notes = order.Notes;
            //orderProm.number = (short)db.OP.ToList().Count;
            //orderProm.Order_Number = order.Index.ToString();
            //orderProm.Data = order.DateOfFormation.ToString();
            //orderProm.TotalPrice = order.Amount.ToString();
            //orderProm.DeliveryInfo = order.City + order.MailOffice;
            //orderProm.Phone = order.Phone;

            //List<UserStatus> US = db.UserStatuses.AsNoTracking().ToList();
            //orderProm.Statuses = new List<Status>();
            //orderProm.Products = new List<ProductProm>();
            //orderProm.Notes = new List<Note>();
            //foreach (UserStatus t in US)
            //{
            //    orderProm.Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
            //}
            //foreach(var cart in order.CurrentCarts)
            //{
            //    orderProm.Products.Add(
            //        new ProductProm(cart.Product.VendorCode,
            //            cart.Product.Url,
            //            cart.Product.Price,
            //            cart.Quantity.ToString(),
            //            "",
            //            cart.Product.Denomination                        
            //            ));
            //}
            //orderProm.Notes.Add(new Note() { Id = Guid.NewGuid().ToString(), flag = true, NoteName="С нашего магазина", number=0 });
            db.OP.Add(orderProm);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderPromExists(orderProm.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = orderProm.Id }, orderProm);
        }

        // DELETE: api/ApiOrderProm/5
        [ResponseType(typeof(OrderProm))]
        public async Task<IHttpActionResult> DeleteOrderProm(string id)
        {
            OrderProm orderProm = await db.OP.FindAsync(id);
            if (orderProm == null)
            {
                return NotFound();
            }

            db.OP.Remove(orderProm);
            await db.SaveChangesAsync();

            return Ok(orderProm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderPromExists(string id)
        {
            return db.OP.Count(e => e.Id == id) > 0;
        }
    }
}