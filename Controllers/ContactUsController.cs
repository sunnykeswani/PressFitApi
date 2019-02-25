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



        [Route("PostContactUs")]
        [ResponseType(typeof(ContactUs))]
        public IHttpActionResult PostContactUs(ContactUs objContactUs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            sendEmail(objContactUs);

            // EmailService objEmail = new EmailService();
            //return Ok("Emailed Successfully");
            return Json(new { success = true, responseText = "Your message successfuly sent!" });
            //objEmail.SendAsync()

            //db.Product.Add(ContactUs);
            //db.SaveChanges();
            //return OkResult;
            //return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        private void sendEmail(ContactUs objContactUs)
        {
            try
            {

                var fromAddress = (ConfigurationManager.AppSettings["FromAddress"]);
                var toAddresses = (ConfigurationManager.AppSettings["AppSuggestionMailAddresses"]).ToString().Split(';');
                var password = ConfigurationManager.AppSettings["Password"];

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress, password.ToString())
                };

                if (objContactUs.Subject.ToLower().Trim() == "sales enquiry".Trim())
                {
                    toAddresses = (ConfigurationManager.AppSettings["SalesAddress"]).ToString().Split(';');
                }
                else if (objContactUs.Subject.ToLower().Trim() != "app suggestion".Trim() && objContactUs.Subject.ToLower().Trim() != "sales enquiry".Trim())
                {
                    toAddresses = (ConfigurationManager.AppSettings["MailAddress"]).ToString().Split(';');
                }



                foreach (var toAddress in toAddresses)
                {
                    var message = new MailMessage(fromAddress, toAddress);
                    message.Subject = (ConfigurationManager.AppSettings["Subject"]).ToString();
                    message.Body = (ConfigurationManager.AppSettings["Body"]).ToString();
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
