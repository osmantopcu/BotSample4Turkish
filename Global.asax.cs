namespace LuisBot
{
    using System.Web.Http;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_Error()
        {
            // Code that runs when an unhandled error occurs

        }
    }
}
