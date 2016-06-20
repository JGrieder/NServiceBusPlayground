using System;
using System.Data;
using System.Data.Common;
using Playground.Core.Interfaces;
using Playground.Core.Utilities;
using Playground.Infrastructure.Database;

namespace Playground.Infrastructure.Repositories
{
    [ServiceVersion(1)]
    public class TestRepositoryContext : ITestRepositoryContext
    {
        private NServiceBusDatabase _database;

        public TestRepositoryContext()
        {
            _database = new NServiceBusDatabase();
        }

        public TestRepositoryContext(IDbConnection connection, DbTransaction transaction)
        {
            _database = new NServiceBusDatabase((DbConnection)connection, false);
            _database.Database.UseTransaction(transaction);
        }

        ~TestRepositoryContext()
        {
            Dispose();
        }

        public ITestRepository CreateTestRepository()
        {
            if (_disposed) throw new InvalidOperationException("Instance is disposed.");

            return new TestRepository(_database);
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

            if (disposing) { _database.Dispose(); }

            _database = null;

            _disposed = true;

        }

        #endregion
    }
}
