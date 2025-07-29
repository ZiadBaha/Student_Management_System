using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface IOtpService
    {
        string GenerateOtp(string email);
        bool IsValidOtp(string email, string otp);
    }
}
