using Microsoft.Extensions.Caching.Memory;
using OtpNet;
using SMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Repository.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateOtp(string email)
        {
            var key = KeyGeneration.GenerateRandomKey(32);
            _cache.Set(email + "_otp_key", key, TimeSpan.FromMinutes(10)); 

            var totp = new Totp(key, step: 600); 
            return totp.ComputeTotp();
        }

        public bool IsValidOtp(string email, string otp)
        {
            if (!_cache.TryGetValue(email + "_otp_key", out byte[]? key) || key is null)
                return false;

            var totp = new Totp(key, step: 600); 
            var isValid = totp.VerifyTotp(otp, out _, new VerificationWindow(1, 1));

            if (isValid)
            {
                _cache.Remove(email + "_otp_key");
                _cache.Set(email, true, TimeSpan.FromMinutes(10)); 
            }

            return isValid;
        }

    }

}
