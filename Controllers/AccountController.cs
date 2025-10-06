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
        private readonly IConfiguration _config;
        public AccountController(IMemoryCache cache, AppDBContext dbcontext, IConfiguration config)
        {
            _cache = cache;
            _dbContext = dbcontext;
            _config = config;
        }
        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return Json(new { success = false, message = "Email required" });

                var exist = await _dbContext.UsersMasters.FirstOrDefaultAsync(x => x.Email == email);
                if (exist == null)
                {
                    return Json(new { success = false, message = "Invalid Email Id or User Not Valid" });
                }
                // Generate OTP
                Random rnd = new Random();
                string otp = rnd.Next(1000, 9999).ToString();

                // Save in cache for 2 minutes
                _cache.Set(email, otp, TimeSpan.FromMinutes(2));

                // Send email (replace credentials)
                var mail = new MailMessage();
                mail.From = new MailAddress(_config.GetSection("Email:FromAddress")!.Value!, _config.GetSection("Email:FromName").Value);
                mail.To.Add(email);
                mail.Subject = "Your OTP Code";
                mail.Body = $"Your OTP is {otp}";

                using (var smtp = new SmtpClient(_config.GetSection("Email:SmtpHost").Value, Convert.ToInt32(_config.GetSection("Email:SmtpPort").Value)))
                {
                    smtp.Credentials = new NetworkCredential(_config.GetSection("Email:Username").Value, _config.GetSection("Email:Password").Value);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
                var user = await _dbContext.UsersMasters.FirstOrDefaultAsync(x => x.Email == email);
                if (user != null)
                {
                    user.OTPExpire = DateTime.Now.AddMinutes(5);
                    user.OTP = otp;
                    await _dbContext.SaveChangesAsync();
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
                HttpContext.Session.SetString("Id", user.Id.ToString());
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("IdCard", user.IdCard??"");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Invalid or expired OTP" });
        }
    }
}

