using SMS.Core.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
    }
}
