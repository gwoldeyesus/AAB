using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AAB.MVC.Startup))]
namespace AAB.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
