using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Reflection;

namespace report.Services
{
    /// <summary>
    ///     Класс для работы с сервисом
    /// </summary>
    public class SMSWorker : Service
    {
        // ключ сессии для хранения инстанса объекта
        private const string SessionKey = "SMSWorker_Instance";

        class SMSProxy
        {
            public string Host;
            public int Port;
        }

        // конструктор
        public SMSWorker()
        {
            this.CookieContainer = new CookieContainer();

            // Если используется прокси
            if (ConfigurationManager.AppSettings["UseProxy"] == "true")
            {
                // Вычитываем настройки прокси из Web.Config - а
                SMSProxy pr = new SMSProxy();
                pr.Host = ConfigurationManager.AppSettings["ProxyHost"];
                pr.Port = Convert.ToInt32(ConfigurationManager.AppSettings["ProxyPort"]);

                WebProxy proxy = new WebProxy(pr.Host, pr.Port);
                this.Proxy = proxy;
            }
        }

        public static SMSWorker GetInstance()
        {
            if (HttpContext.Current == null)
                return new SMSWorker();

            // определяем, существует ли объект в сессии
            SMSWorker worker = HttpContext.Current.Session[SessionKey] as SMSWorker;
            if (worker == null)
            {
                // создаем объект
                worker = new SMSWorker();
                // помещаем в сессию
                HttpContext.Current.Session[SessionKey] = worker;
            }
            return worker;
        }

        /// <summary>
        ///    Обнуляем объект в сессии (если нужно)
        /// </summary>
        public void CloseSession()
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Session[SessionKey] = null;
        }
    }
}
