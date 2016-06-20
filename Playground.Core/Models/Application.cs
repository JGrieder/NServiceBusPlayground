using System;

namespace Playground.Core.Models
{
    public class Application
    {
        public int Id { get; set; }
        public Guid ApplicationKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Description { get; set; }
        public Definitions.ApplicationType Type { get; set; }
        public int AccountId { get; set; } 
    }
}