using Playground.Core.Messages;
using Playground.Core.Models;

namespace Playground.Core.Utilities
{
    public static class CoreExtensions
    {
        public static Application Convert(this AddNewApplicationCommand source)
        {
            if (source == null)
                return null;

            return new Application
            {
                AccountId = source.AccountId,
                Description = source.Description,
                Type = (Definitions.ApplicationType)source.ApplicationType
            };
        }
    }
}