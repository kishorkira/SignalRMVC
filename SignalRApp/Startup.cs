using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SignalRApp.Startup))]
namespace SignalRApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
