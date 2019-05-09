using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShopParser.Startup))]
namespace ShopParser
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}