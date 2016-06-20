using System;
using System.Data.Common;
using NServiceBus;
using NServiceBus.Persistence.NHibernate;
using Playground.Core.Interfaces;
using Playground.Core.Messages;
using Playground.Core.Utilities;

namespace Playground.Service.Handlers
{
    public class AddNewApplicationHandler : IHandleMessages<AddNewApplicationCommand>
    {
        private readonly ITestRepositoryContextFactory _contextFactory;
        private readonly NHibernateStorageContext _storageContext;
        private readonly IBus _bus;

        public AddNewApplicationHandler(IBus bus, ITestRepositoryContextFactory contextFactory, NHibernateStorageContext storageContext)
        {
            _bus = bus;
            _contextFactory = contextFactory;
            _storageContext = storageContext;
        }

        public void Handle(AddNewApplicationCommand message)
        {
            var newApplication = message.Convert();

            using (var context = _contextFactory.CreateTestRepositoryContext(1, _storageContext.Connection, (DbTransaction)_storageContext.DatabaseTransaction))
            {
                using (var testRepository = context.CreateTestRepository())
                {
                    var application = testRepository.AddNewApplication(newApplication);

                    //_storageContext.Session.Save(application);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("New Application Was Added");
                    Console.WriteLine("ApplicationId: " + application.Id);
                    Console.WriteLine("ApplicationKey: " + application.ApplicationKey);
                    Console.WriteLine("ApplicationDescription: " + application.Description);
                    Console.WriteLine("ApplicationCreateDate: " + application.CreateDate.ToShortDateString());
                    Console.ResetColor();
                }
            }
        }
    }
}