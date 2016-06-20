using System;

namespace Playground.Core.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Definitions.AccountStatus Status { get; set; } 
    }
}