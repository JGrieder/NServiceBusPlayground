using System.Configuration;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

namespace Playground.Integration.Sender
{
    public class ConfigMessageEndpointMappings : IConfigurationSource
    {
        public T GetConfiguration<T>() where T : class, new()
        {
            if (typeof (T) != typeof (UnicastBusConfig))
                return ConfigurationManager.GetSection(typeof (T).Name) as T;

            var config = (UnicastBusConfig)ConfigurationManager.GetSection(typeof(UnicastBusConfig).Name);

            if (config == null)
            {
                config = new UnicastBusConfig
                {
                    MessageEndpointMappings = new MessageEndpointMappingCollection()
                };
            }

            config.MessageEndpointMappings.Add(
                new MessageEndpointMapping
                {
                    AssemblyName = "Playground.Core",
                    Namespace = "Playground.Core.Messages",
                    Endpoint = "Playground.Service"
                });

            return config as T;
        }
    }
}
