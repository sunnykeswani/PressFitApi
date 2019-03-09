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
            try
            {


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                sendEmail(objContactUs);
                return Json(new { success = true, responseText = "We have received your message. We will get in touch shortly.Thank you!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message });
            }
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
                else if (objContactUs.Subject.ToLower().Trim() != "report a bug in the app".Trim() && objContactUs.Subject.ToLower().Trim() != "sales enquiry".Trim())
                {
                    toAddresses = (ConfigurationManager.AppSettings["MailAddress"]).ToString().Split(';');
                }



                foreach (var toAddress in toAddresses)
                {
                    var message = new MailMessage(fromAddress, toAddress);
                    //message.Subject = (ConfigurationManager.AppSettings["Subject"]).ToString();
                    //message.Body = (ConfigurationManager.AppSettings["Body"]).ToString();
                    message.Subject = objContactUs.Subject;
                    message.Body = "Mobile App Enquiry:" + Environment.NewLine + "Subject:" + objContactUs.Subject + Environment.NewLine + "Name:" + objContactUs.Name + Environment.NewLine + "Email:" + objContactUs.Email + Environment.NewLine + "Contact:" + objContactUs.MobileNo + Environment.NewLine + "City:" + objContactUs.City + Environment.NewLine + "State:" + objContactUs.State + Environment.NewLine + "Message:" + objContactUs.Message + Environment.NewLine;
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
