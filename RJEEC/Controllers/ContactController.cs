using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Contact contact)
        {
            try
            {
                string host = _config.GetSection("SMTP").GetSection("Host").Value;
                string from = _config.GetSection("SMTP").GetSection("From").Value;
                using (SmtpClient client = new SmtpClient(host))
                {
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(contact.Email);
                    mailMessage.To.Add(from);
                    mailMessage.Body = contact.Message;
                    mailMessage.Subject = "[RJEEC] Message from " + contact.FirstName + " " + contact.LastName;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    client.Send(mailMessage);
                    return View("MessageSent");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }            
    }
}