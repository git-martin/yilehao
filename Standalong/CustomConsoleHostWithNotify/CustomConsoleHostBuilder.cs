using System;
using Topshelf;
using Topshelf.Builders;
using Topshelf.Hosts;
using Topshelf.Runtime;

namespace Topshelf.Extension
{
    public class CustomConsoleHostBuilder :
        HostBuilder
    {

        readonly HostSettings settings;
        readonly HostEnvironment environment;

        public CustomConsoleHostBuilder(HostEnvironment environment, HostSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.environment = environment;
            this.settings = settings;
        }

        public HostEnvironment Environment
        {
            get { return environment; }
        }

        public HostSettings Settings
        {
            get { return settings; }
        }

        public virtual Host Build(ServiceBuilder serviceBuilder)
        {
            ServiceHandle serviceHandle = serviceBuilder.Build(settings);

            return CreateHost(serviceHandle);
        }

        public void Match<T>(Action<T> callback)
            where T : class, HostBuilder
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            var self = this as T;
            if (self != null)
            {
                callback(self);
            }
        }

        Host CreateHost(ServiceHandle serviceHandle)
        {
            if (environment.IsRunningAsAService)
            {
                return environment.CreateServiceHost(settings, serviceHandle);
            }

            return new CustomConsoleHost(settings, environment, serviceHandle);
        }
    }
}
