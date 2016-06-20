using System;

namespace Playground.Core.Messages
{
    public class NewWebApplicationAddedEvent
    {
        public int AccountId { get; set; }
        public string Description { get; set; }
        public short ApplicationType { get; set; }
        public DateTime CreateDate { get; set; }
        public string ApplicationKey { get; set; }
    }
}