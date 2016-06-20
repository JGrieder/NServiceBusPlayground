using System;
using System.Collections.Generic;
using Playground.Core;
using Playground.Core.Interfaces;
using Playground.Core.Models;
using Playground.Infrastructure.Database;
using Playground.Infrastructure.Database.Entities;

namespace Playground.Infrastructure.Repositories
{
    public class TestRepository : ITestRepository
    {

        private NServiceBusDatabase _database;

        public TestRepository(NServiceBusDatabase database)
        {
            _database = database;
        }

        ~TestRepository()
        {
            Dispose();
        }

        public IEnumerable<Application> GetApplications()
        {
            return new List<Application>
            {
                new Application { AccountId = 84, ApplicationKey = new Guid(), CreateDate = DateTime.Today, Description = "Test Desktop Application", Id = 1, ModifiedDate = DateTime.Today, Type = Definitions.ApplicationType.Desktop },
                new Application { AccountId = 84, ApplicationKey = new Guid(), CreateDate = DateTime.Today, Description = "Test Web Application", Id = 1, ModifiedDate = DateTime.Today, Type = Definitions.ApplicationType.Web },
                new Application { AccountId = 84, ApplicationKey = new Guid(), CreateDate = DateTime.Today, Description = "Test Mobile Application", Id = 1, ModifiedDate = DateTime.Today, Type = Definitions.ApplicationType.Mobile }
            };
        }

        public Application AddNewApplication(Application application)
        {
            application.ApplicationKey = Guid.NewGuid();
            application.CreateDate = DateTime.Now;
            application.ModifiedDate = DateTime.Now;

            _database.ApplicationRegistries.Add(application.Convert());

            var applicationId = _database.SaveChanges();

            if (applicationId == 0) throw new Exception("Insert Failed");

            application.Id = applicationId;

            return application;
        }

        #region IDisposable Implementation

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing) { /*Do Something here when required*/ }

            _database = null;

            _disposed = true;

        }

        #endregion
    }

    internal static class RepositoryExtensions
    {
        internal static ApplicationRegistry Convert(this Application source)
        {
            if (source == null) return null; //Throw Exception might be better options here

            return new ApplicationRegistry
            {
                AccountId = source.AccountId,
                ApplicationKey = source.ApplicationKey.ToString(),
                ApplicationType = (byte)source.Type,
                Description = source.Description,
                CreateDate = source.CreateDate,
                ModifiedDate = source.ModifiedDate,
                Id = source.Id
            };
        }
    }
}