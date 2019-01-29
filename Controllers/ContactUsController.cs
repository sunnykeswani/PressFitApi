using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PressFitApi.Models;
using System.Web.Http.Description;
using System.Net.Mail;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PressFitApi.Controllers
{
    public class ContactUsController : ApiController
    {
        private PressFitApiContext db = new PressFitApiContext();


        [Route("GetSubjects")]
        public IHttpActionResult Get()
        {
            var request = new String[3];
            request[0] = "Service";
            request[1] = "Complaint";
            request[2] = "Service1";

            var json = JsonConvert.SerializeObject(request);
            return Ok(json);
        }

        [Route("PostContactUs")]
        [ResponseType(typeof(ContactUs))]
        public IHttpActionResult PostContactUs(ContactUs objContactUs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           // sendEmail(objContactUs);

            // EmailService objEmail = new EmailService();
            return Ok("Emailed Successfully");
            //objEmail.SendAsync()

            //db.Product.Add(ContactUs);
            //db.SaveChanges();
            //return OkResult;
            //return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        private void sendEmail(ContactUs objContactUs)
        {
            var fromAddress = new MailAddress(ConfigurationManager.AppSettings["FromAddress"], "From Name");
            var toAddress = new MailAddress(ConfigurationManager.AppSettings["FromAddress"], "To Name");
            string fromPassword = "#test$1234";


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = objContactUs.Subject,
                Body = objContactUs.Message
            })
            {
                smtp.Send(message);
            }

        }
    }
}
