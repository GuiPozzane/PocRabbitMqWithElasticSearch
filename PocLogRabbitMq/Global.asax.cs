using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Network;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PocLogRabbitMq
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static IBusControl BusControl;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) => true;
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", "PocLogRabbitMq")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://localhost:9200"))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "logs-{0:yyyy.MM}",
                ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "5_h=3Wt1HRzc2x=ncJk*"),

            })
            .CreateLogger();
            Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Debug.WriteLine(msg));
            // Make sure to call this after configuring Serilog
            Log.Information("Application Starting Up");
            BusControl = GetBus();
            BusControl.Start();
            Log.Information("Bus Started");
        }
        private IBusControl GetBus()
        {
            return WebApiConfig.Container.GetService<IBusControl>();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            // Log critical errors
            Log.Fatal(exception, "An unhandled exception occurred.");
        }
        protected void Application_End()
        {
            Log.Information("Application Shutting Down");
            if (BusControl != null)
            {
                BusControl.Stop(TimeSpan.FromSeconds(30)); // Use StopAsync in async environments
                Log.Information("Bus is stopping");                                         // No explicit Dispose call is necessary after Stop; it handles cleanup.
            }
            
            Log.CloseAndFlush();
        }
    }
}
