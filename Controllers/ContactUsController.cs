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
using System.Web;
using System.IO;


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
                // var httpRequest = HttpContext.Current.Request;

                //if (!Request.Content.IsMimeMultipartContent())
                //    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                SaveDetails(objContactUs);
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
                var lstSubject = db.Subject.ToList();
                var fromAddress = (ConfigurationManager.AppSettings["FromAddress"]);
                //var toAddresses = (ConfigurationManager.AppSettings["AppSuggestionMailAddresses"]).ToString().Split(';');
                var password = ConfigurationManager.AppSettings["Password"];

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress, password.ToString())
                };


                //CR22-09-2019 replacing hardcoded code with new subjectlist
                var commaSeperatedAddress = lstSubject.Where(x => x.Subject.ToLower().Equals(objContactUs.Subject.ToLower())).ToList().Select(y => y.EmailId).ToList();
                var toAddresses = commaSeperatedAddress[0].Split(',').ToList();
                //previous code commented on 22-09-19
                //if (objContactUs.Subject.ToLower().Trim() == "sales enquiry".Trim())
                //{
                //    toAddresses = (ConfigurationManager.AppSettings["SalesAddress"]).ToString().Split(';');
                //}
                //else if (objContactUs.Subject.ToLower().Trim() != "report a bug in the app".Trim() && objContactUs.Subject.ToLower().Trim() != "sales enquiry".Trim())
                //{
                //    toAddresses = (ConfigurationManager.AppSettings["MailAddress"]).ToString().Split(';');
                //}



                foreach (var toAddress in toAddresses)
                {
                    if (toAddress != string.Empty)
                    {
                        var message = new MailMessage(fromAddress, toAddress.ToString());
                        //message.Subject = (ConfigurationManager.AppSettings["Subject"]).ToString();
                        //message.Body = (ConfigurationManager.AppSettings["Body"]).ToString();
                        message.Subject = objContactUs.Subject;
                        message.Body = "Mobile App Enquiry:" + Environment.NewLine + "Subject:" + objContactUs.Subject + Environment.NewLine + "Name:" + objContactUs.Name + Environment.NewLine + "Email:" + objContactUs.Email + Environment.NewLine + "Contact:" + objContactUs.MobileNo + Environment.NewLine + "City:" + objContactUs.City + Environment.NewLine + "State:" + objContactUs.State + Environment.NewLine + "Message:" + objContactUs.Message + Environment.NewLine;

                        if (objContactUs.ScreenshotArray != null)
                        {
                            // attachment code 
                            var fileName = objContactUs.Id + "_img_";
                            var files1 = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/ImageStorage")).ToList();
                            var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/ImageStorage")).ToList().Where(x => Path.GetFileName(x).StartsWith(fileName));
                            foreach (var item in files)
                            {
                                message.Attachments.Add(new Attachment(item));
                            }
                        }

                        smtp.Send(message);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void SaveDetails(ContactUs objContactUs)
        {
            try
            {

                db.ContactUs.Add(objContactUs);
                db.SaveChanges();

                // save screenshots 
                string screenshots = string.Empty;
                if (objContactUs.ScreenshotArray != null && objContactUs.ScreenshotArray.Count() > 1)
                {
                    objContactUs.Screenshot = ConvertArrayToString(objContactUs.ScreenshotArray);

                    foreach (var screenshot in objContactUs.ScreenshotArray)
                    {
                        var Id = db.ContactUs.OrderByDescending(p => p.Id).FirstOrDefault().Id;
                        string ImgName = Id + "_img_" + DateTime.UtcNow.Millisecond;
                        SaveImage(screenshot, ImgName);
                    }
                }

                //var Array = SplitStringToArray(screenshots);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string ConvertArrayToString(string[] arr)
        {
            try
            {
                var JoinedString = String.Join("$$$$", arr);
                return JoinedString;
            }
            catch (Exception)
            {

                throw new NotImplementedException();
            }

        }

        private string[] SplitStringToArray(string str)
        {
            try
            {
                var Arr = str.Split(new string[] { "$$$$" }, StringSplitOptions.None);
                return Arr;
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }

        }

        public bool SaveImage(string ImgStr, string ImgName)
        {
            try
            {


                String path = HttpContext.Current.Server.MapPath("~/ImageStorage"); //Path

                //Check if directory exist
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                string imageName = ImgName + ".jpg";

                //set the image path
                string imgPath = Path.Combine(path, imageName);

                byte[] imageBytes = Convert.FromBase64String(ImgStr);

                File.WriteAllBytes(imgPath, imageBytes);

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }


        public IEnumerable<string> GetImage() =>
      Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/ImageStorage")).Select(Path.GetFileName);
    }
}
