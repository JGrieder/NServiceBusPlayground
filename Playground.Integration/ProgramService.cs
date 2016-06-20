using System;
using System.ServiceProcess;
using NServiceBus;
using NServiceBus.Logging;

namespace Playground.Integration
{
    public class ProgramService : ServiceBase
    {
        IBus _bus;

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
                busConfiguration.EndpointName("Playground.Integration.Client");
                busConfiguration.UseSerialization<JsonSerializer>();
                busConfiguration.DefineCriticalErrorAction(OnCriticalError);
                busConfiguration.UsePersistence<InMemoryPersistence>();

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