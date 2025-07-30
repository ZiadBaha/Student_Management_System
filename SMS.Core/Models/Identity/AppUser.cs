using Microsoft.AspNetCore.Identity;
using SMS.Core.Enums;
using SMS.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public UserRole UserRole { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
