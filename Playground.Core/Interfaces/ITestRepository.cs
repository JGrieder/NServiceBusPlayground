using System;
using System.Collections.Generic;
using Playground.Core.Models;

namespace Playground.Core.Interfaces
{
    public interface ITestRepository : IDisposable
    {
        IEnumerable<Application> GetApplications();

        Application AddNewApplication(Application application);
    }
}