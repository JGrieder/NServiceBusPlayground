using System;

namespace Playground.Core.Utilities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceVersionAttribute : Attribute
    {
        public short Version { get; set; }

        public ServiceVersionAttribute(short version)
        {
            Version = version;
        }

    }
}