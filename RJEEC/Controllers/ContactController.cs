using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RJEEC.Models;

namespace RJEEC.Controllers
{
    public class ContactController : Controller
    {
        private readonly IConfiguration _config;
        public ContactController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(Contact contact)
        {
            if(ModelState.IsValid)
            {
                string host = _config.GetSection("SMTP").GetSection("Host").Value;
                string rjeecContactMail = _config.GetSection("SMTP").GetSection("From").Value;
                string pass = _config.GetSection("SMTP").GetSection("Password").Value;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(rjeecContactMail);
                mailMessage.To.Add(rjeecContactMail);
                mailMessage.Body = contact.Message + contact.Email;
                mailMessage.Subject = "[RJEEC] Message from " + contact.FirstName + " " + contact.LastName;
                mailMessage.IsBodyHtml = true;

                using (var smtp = new SmtpClient(host, 587))
                {
                    var credential = new NetworkCredential
                    {
                        UserName = rjeecContactMail,  // replace with valid value
                        Password = pass  // replace with valid value
                    };
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = credential;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mailMessage);
                    return RedirectToAction("MessageSent");
                }
            }
            return View(contact);
        }
        [AllowAnonymous]
        public ActionResult MessageSent()
        {
            return View();
        }
    }
}