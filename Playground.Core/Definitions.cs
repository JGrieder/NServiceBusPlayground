namespace Playground.Core
{
    public class Definitions
    {
        public enum ApplicationType : short
        {
            Desktop = 1,
            Web = 2,
            Mobile = 3,
            Service = 4
        }

        public enum AccountStatus : short
        {
            Enabled = 0,
            Lockout = 1,
            Disabled = 2,
            Deleted = 3
        }
    }
}