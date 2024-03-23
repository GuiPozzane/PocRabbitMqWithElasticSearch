using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace PocLogRabbitMq.App_Start
{
    public class DefaultDependencyResolver : IDependencyResolver
    {
        protected IServiceProvider serviceProvider;

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IDependencyScope BeginScope()
        {
            return new DefaultDependencyScope(serviceProvider.CreateScope());
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceProvider.GetServices(serviceType);
        }

        public void Dispose()
        {
            // No need to dispose anything here
            // If using a scope, it should be disposed by the scope itself
        }
    }

    public class DefaultDependencyScope : IDependencyScope
    {
        protected IServiceScope scope;

        public DefaultDependencyScope(IServiceScope scope)
        {
            this.scope = scope;
        }

        public object GetService(Type serviceType)
        {
            return scope.ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return scope.ServiceProvider.GetServices(serviceType);
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}