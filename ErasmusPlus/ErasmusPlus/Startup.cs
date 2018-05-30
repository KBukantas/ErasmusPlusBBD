using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ErasmusPlus.Startup))]
namespace ErasmusPlus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
