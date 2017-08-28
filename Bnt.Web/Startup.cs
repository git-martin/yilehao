using Microsoft.Owin;
using Owin;

//[assembly: OwinStartupAttribute(typeof(Bnt.Web.Startup))]
namespace Bnt.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use((context, next) => {
                context.Response.Headers.Append("X-Generator", "BntWeb");
                return next();
            });
            ConfigureAuth(app);
        }
    }
}
