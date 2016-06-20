using System;
using System.ServiceProcess;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using Playground.Core.Interfaces;
using Playground.Infrastructure.Factories;
using StructureMap;
using Environment = System.Environment;

namespace Playground.Service
{
    public class ProgramService : ServiceBase
    {
        private IBus _bus;

        private static readonly ILog Logger = LogManager.GetLogger<ProgramService>();

        public static void Main()
        {
            using (var service = new ProgramService())
            {
                // so we can run interactive from Visual Studio or as a windows service
                if (Environment.UserInteractive)
                {
                    //Console.CancelKeyPress += (sender, e) =>
                    //{
                    //    service.OnStop();
                    //};
                    service.OnStart(null);
                    Console.WriteLine("\r\nPress enter key to stop program\r\n");
                    Console.Read();
                    service.OnStop();
                    return;
                }
                Run(service);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var busConfiguration = new BusConfiguration();
                busConfiguration.EndpointName("Playground.Service");
                busConfiguration.UseSerialization<JsonSerializer>();
                busConfiguration.DefineCriticalErrorAction(OnCriticalError);
                busConfiguration.UseTransport<MsmqTransport>();

                var hibernateConfig = new Configuration();
                hibernateConfig.DataBaseIntegration(db =>
                {
                    db.ConnectionStringName = "NServiceBusPlaygroundDbContext";
                    db.Dialect<MsSql2012Dialect>();
                    db.Driver<SqlClientDriver>();
                });

                //var mapper = new ModelMapper();
                //mapper.AddMapping<>();

                busConfiguration.UsePersistence<NHibernatePersistence>()
                    .UseConfiguration(hibernateConfig)
                    .RegisterManagedSessionInTheContainer();
                

                busConfiguration.CustomConfigurationSource(new ConfigMessageEndpointMappings());

                var container = new Container(registry =>
                {
                    registry.For<ITestRepositoryContextFactory>().Use(new TestRepositoryContextFactory());
                });
                busConfiguration.UseContainer<StructureMapBuilder>(c => c.ExistingContainer(container));

                var conventionsBuilder = busConfiguration.Conventions();
                conventionsBuilder.DefiningCommandsAs(t => t.Namespace != null && t.Namespace == "Playground.Core.Messages" && t.Name.EndsWith("Command"));
                conventionsBuilder.DefiningEventsAs(t => t.Namespace != null && t.Namespace == "Playground.Core.Messages" && t.Name.EndsWith("Event"));
                conventionsBuilder.DefiningMessagesAs(t => t.Namespace != null && t.Namespace == "Playground.Core.Messages" && t.Name.EndsWith("Message"));

                ////TODO: this if is here to prevent you from accidentally deploy to production without considering important actions
                //if (Environment.UserInteractive && Debugger.IsAttached)
                //{
                //    //TODO: For production use, please select a durable persistence.
                //    //http://docs.particular.net/nservicebus/persistence/
                //    busConfiguration.UsePersistence<InMemoryPersistence>();

                //}



                //TODO: For production use, please script your installation.
                busConfiguration.EnableInstallers();

                var startableBus = Bus.Create(busConfiguration);
                _bus = startableBus.Start();
            }
            catch (Exception exception)
            {
                OnCriticalError("Failed to start the bus.", exception);
            }
        }

        public void OnCriticalError(string errorMessage, Exception exception)
        {
            //TODO: Decide if shutting down the process is the best response to a critical error
            //http://docs.particular.net/nservicebus/hosting/critical-errors
            var fatalMessage = string.Format("The following critical error was encountered:\n{0}\nProcess is shutting down.", errorMessage);
            Logger.Fatal(fatalMessage, exception);
            Environment.FailFast(fatalMessage, exception);
        }

        protected override void OnStop()
        {
            _bus?.Dispose();
        }
    }
}