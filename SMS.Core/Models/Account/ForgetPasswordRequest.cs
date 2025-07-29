using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Models.Account
{
    public class ForgetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

}
