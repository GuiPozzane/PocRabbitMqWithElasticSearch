using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using PocLogRabbitMq.App_Start;
using PocLogRabbitMq.Rabbitmq.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace PocLogRabbitMq
{
    public static class WebApiConfig
    {
        public static IServiceProvider Container;
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);

            var services = new ServiceCollection();
            

            var provider = services.BuildServiceProvider();
            services.AddMassTransit(busConfiguration =>
            {
                busConfiguration.AddConsumers();

                busConfiguration.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", 5672, "/", host => {
                        host.Username("rabbit");
                        host.Password("rabbit");

                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
            // Configura que, por padrão, todos os métodos precisarão de Authentication via Bearer Token (possível sobrescrever essa regra em cada método específico).

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() };
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Register API controllers manually
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(ApiController).IsAssignableFrom(type))
                .ToList()
                .ForEach(type => services.AddTransient(type));

            // Build ServiceProvider
            Container = services.BuildServiceProvider();

            // Set Web API's dependency resolver to use our ServiceProvider
            config.DependencyResolver = new DefaultDependencyResolver(Container);
        }
    }

    public class ServiceProviderControllerActivator : IHttpControllerActivator
    {
        private readonly IServiceProvider _provider;

        public ServiceProviderControllerActivator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var scopeFactory = _provider.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            request.RegisterForDispose(scope);

            return scope.ServiceProvider.GetService(controllerType) as IHttpController;
        }
    }
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddControllersAsServices(this IServiceCollection services, IEnumerable<Type> serviceTypes)
        {
            foreach (var type in serviceTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }
    }
    public static class RabbitMqExtensions 
    {
        
        public static void AddConsumers(this IBusRegistrationConfigurator busConfigurator)
        {
            busConfigurator.AddConsumer<CijunMessageConsumer, CijunMessageConsumerDefinition>();
        }
    }
}
