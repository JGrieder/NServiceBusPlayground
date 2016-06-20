using System;

namespace Playground.Core.Interfaces
{
    public interface ITestRepositoryContext : IDisposable
    {
        ITestRepository CreateTestRepository();
    }
}