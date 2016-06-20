using System;
using System.ServiceProcess;
using NServiceBus;
using NServiceBus.Logging;
using Playground.Core.Messages;

namespace Playground.Integration.Sender
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
                busConfiguration.EndpointName("Playground.Integration.Sender");
                busConfiguration.UseSerialization<JsonSerializer>();
                busConfiguration.DefineCriticalErrorAction(OnCriticalError);

                //TODO: For production use, please select a durable persistence.
                //http://docs.particular.net/nservicebus/persistence/
                busConfiguration.UsePersistence<InMemoryPersistence>();
                busConfiguration.UseTransport<MsmqTransport>();

                busConfiguration.CustomConfigurationSource(new ConfigMessageEndpointMappings());

                var conventionsBuilder = busConfiguration.Conventions();
                conventionsBuilder.DefiningCommandsAs(t => t.Namespace != null && t.Namespace == "Playground.Core.Messages" && t.Name.EndsWith("Command"));
                conventionsBuilder.DefiningEventsAs(t => t.Namespace != null && t.Namespace == "Playground.Core.Messages" && t.Name.EndsWith("Event"));
                conventionsBuilder.DefiningMessagesAs(t => t.Namespace != null && t.Namespace == "Playground.Core.Messages" && t.Name.EndsWith("Message"));

                //TODO: For production use, please script your installation.
                busConfiguration.EnableInstallers();

                var startableBus = Bus.Create(busConfiguration);
                _bus = startableBus.Start();
                StartEventLoop();

                
            }
            catch (Exception exception)
            {
                OnCriticalError("Failed to start the bus.", exception);
            }
        }

        public void StartEventLoop()
        {
            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.A:
                        
                        var addAppCommand = new AddNewApplicationCommand
                        {
                            AccountId = 1,
                            ApplicationType = 1,
                            Description = "New Desktop Application"
                        };

                        _bus.Send(new Address("Playground.Service", "DESKTOP-883BG27"), addAppCommand);

                        Console.WriteLine($"Sent AddNewApplicationCommand.");

                        continue;

                    case ConsoleKey.B:

                        var addNewWebAppCommand = new AddNewApplicationCommand
                        {
                            AccountId = 1,
                            ApplicationType = 2,
                            Description = "New Web Application"
                        };

                        _bus.Send(new Address("Playground.Service", "DESKTOP-883BG27"), addNewWebAppCommand);

                        Console.WriteLine($"Sent AddNewApplicationCommand");

                        continue;

                    case ConsoleKey.C:

                        var addNewMobileAppCommand = new AddNewApplicationCommand
                        {
                            AccountId = 1,
                            ApplicationType = 3,
                            Description = "New Mobile Application"
                        };

                        _bus.Send(new Address("Playground.Service", "DESKTOP-883BG27"), addNewMobileAppCommand);

                        Console.WriteLine($"Sent AddNewApplicationCommand");

                        continue;

                    default:
                        return;
                }
            }
        }

        public void OnCriticalError(string errorMessage, Exception exception)
        {
            //TODO: Decide if shutting down the process is the best response to a critical error
            //http://docs.particular.net/nservicebus/hosting/critical-errors
            var fatalMessage =
                $"The following critical error was encountered:\n{errorMessage}\nProcess is shutting down.";
            Logger.Fatal(fatalMessage, exception);
            Environment.FailFast(fatalMessage, exception);
        }

        protected override void OnStop()
        {
            _bus?.Dispose();
        }
    }
}