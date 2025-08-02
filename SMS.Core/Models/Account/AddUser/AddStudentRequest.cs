using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Models.Account.AddUser
{
    public class AddStudentRequest : RegisterRequest
    {
        public DateTime DateOfBirth { get; set; }
    }
}
