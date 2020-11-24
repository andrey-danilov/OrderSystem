using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(report.Startup))]
namespace report
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
