using System;
using System.Data;
using System.Data.Common;

namespace Playground.Core.Interfaces
{
    public interface ITestRepositoryContextFactory : IDisposable
    {
        ITestRepositoryContext CreateTestRepositoryContext(short version);

        ITestRepositoryContext CreateTestRepositoryContext(short version, IDbConnection connection,
            DbTransaction transaction);
    }
}