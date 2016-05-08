using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IMS2.Startup))]
namespace IMS2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
