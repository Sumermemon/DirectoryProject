using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Mail;

namespace DirectoryProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMemoryCache _cache;
        public AccountController(IMemoryCache cache)
        {
            _cache = cache;
        }
        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return Json(new { success = false, message = "Email required" });

                // Generate OTP
                Random rnd = new Random();
                string otp = rnd.Next(1000, 9999).ToString();

                // Save in cache for 2 minutes
                _cache.Set(email, otp, TimeSpan.FromMinutes(2));

                // Send email (replace credentials)
                var mail = new MailMessage();
                mail.From = new MailAddress("yourgmail@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Your OTP Code";
                mail.Body = $"Your OTP is {otp}";

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("yourgmail@gmail.com", "your-app-password");
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult VerifyOtp(string email, string otp)
        {
            if (_cache.TryGetValue(email, out string savedOtp))
            {
                if (savedOtp == otp)
                {
                    _cache.Remove(email);
                    HttpContext.Session.SetString("UserEmail", email);
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Invalid or expired OTP" });
        }
    }
}

