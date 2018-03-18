using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(northopsService.Startup))]

namespace northopsService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}