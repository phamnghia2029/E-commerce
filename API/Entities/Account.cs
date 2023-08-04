using System;
using System.Collections.Generic;

namespace API.Entities
{
    public partial class Account
    {
        public int AccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public int? Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpireAt { get; set; }
        public string? AccessToken { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
