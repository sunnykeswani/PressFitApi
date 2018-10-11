using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PressFitApi.Startup))]
namespace PressFitApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
