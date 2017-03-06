using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TextPlagiarismWebApp.Startup))]
namespace TextPlagiarismWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
