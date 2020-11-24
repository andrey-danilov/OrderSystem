using Newtonsoft.Json;
using Quartz;
using report.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace report.Jobs
{

    public class ShopOrders : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task Execute(IJobExecutionContext context)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:57423/api/OrdersApi");
            request.Method = "GET";
            request.Accept = "application/json";
            request.UserAgent = "Mozilla/5.0 ....";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            StringBuilder output = new StringBuilder();
            output.Append(reader.ReadToEnd());
            response.Close();


            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(output.ToString());

            foreach(var order in orders)
            {
                OrderProm orderProm = new OrderProm();
                orderProm.Id = order.Id;
                orderProm.Id_User = "aae2ed54-b1c2-496d-b503-64e1b56f4156";
                orderProm.FromShop = true;
                orderProm.number = (short)(db.OP.ToList().Count+1);
                orderProm.Order_Number = order.Index.ToString();
                orderProm.Data = order.DateOfFormation.ToString();
                orderProm.TotalPrice = order.Amount.ToString();
                orderProm.DeliveryInfo = order.City +" отделение №"+ order.MailOffice;
                orderProm.Phone = order.Phone;
                List<UserStatus> US = db.UserStatuses.AsNoTracking().ToList();
                orderProm.Statuses = new List<Status>();
                orderProm.Products = new List<ProductProm>();
                orderProm.Notes = new List<Note>();
                foreach (UserStatus t in US)
                {
                    orderProm.Statuses.Add(new Status(t.StatusName, t.DefoltFlag, t.number));
                }
                foreach (var cart in order.CurrentCarts)
                {
                    orderProm.Products.Add(
                        new ProductProm(cart.Product.VendorCode,
                            cart.Product.Url,
                            cart.Product.Price,
                            cart.Quantity.ToString(),
                            "",
                            cart.Product.Denomination
                            ));
                }
                orderProm.Notes.Add(new Note() { Id = Guid.NewGuid().ToString(), flag = true, NoteName = "С нашего магазина", number = 0 });
                db.OP.Add(orderProm);
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch
            {
                
            }

        }
    }
}
