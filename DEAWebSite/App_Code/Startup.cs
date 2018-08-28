using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DEA.Startup))]
namespace DEA
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
