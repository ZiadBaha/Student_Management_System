using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Models.Account.AddUser
{
    public class AddTeacherRequest : RegisterRequest
    {
        public int DepartmentId { get; set; }
    }
}
