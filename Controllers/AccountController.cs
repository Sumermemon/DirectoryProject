using DirectoryProject.DBHelper;
using DirectoryProject.Entity;
using DirectoryProject.Layer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DirectoryProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly AppDBContext _dbContext;
        public AccountController(IMemoryCache cache, AppDBContext dbcontext)
        {
            _cache = cache;
            _dbContext = dbcontext;
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
                var user = await _dbContext.UsersMasters.FirstOrDefaultAsync(x => x.Email == email);
                if (user != null)
                {
                    user.OTPExpire = DateTime.Now.AddMinutes(5);
                    user.OTP = otp;
                    _dbContext.UsersMasters.Add(user);
                    _dbContext.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(otp))
            {
                var user = await _dbContext.UsersMasters.FirstOrDefaultAsync(x => x.Email == email && x.OTP == otp && x.OTPExpire > DateTime.Now);
                if(user == null)
                {
                    return Json(new { success = false, message = "Invalid or expired OTP" });
                }
                HttpContext.Session.SetString("UserEmail", email);
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Invalid or expired OTP" });
        }
    }
}

