using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Playground.Core.Interfaces;
using Playground.Core.Utilities;

namespace Playground.Infrastructure.Factories
{
    public class TestRepositoryContextFactory : ITestRepositoryContextFactory
    {
        ~TestRepositoryContextFactory()
        {
            Dispose();
        }

        public ITestRepositoryContext CreateTestRepositoryContext(short version)
        {
            return CreateGenericRepositoryContext<ITestRepositoryContext>(version);
        }

        public ITestRepositoryContext CreateTestRepositoryContext(short version, IDbConnection connection, DbTransaction transaction)
        {
            return CreateGenericRepositoryContext<ITestRepositoryContext>(version, connection, transaction);
        }

        private static T CreateGenericRepositoryContext<T>(short version, params object[] constructorArguments)
        {
            var repo = typeof (T)
                .GetInstantiableImplementors(AppDomain.CurrentDomain.GetAssemblies())
                .FirstOrDefault(r =>r.GetCustomAttributes(typeof (ServiceVersionAttribute), false)
                            .Cast<ServiceVersionAttribute>()
                            .Any(x => x.Version == version));

            if (repo == null) throw new NullReferenceException("The Repository doesn't exist");

            var instance = Activator.CreateInstance(repo, constructorArguments);

            return (T) instance;
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

            _disposed = true;

        }

        #endregion
    }
}