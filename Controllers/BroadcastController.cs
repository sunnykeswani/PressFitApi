using Newtonsoft.Json.Linq;
using PressFitApi.Models;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

using System.Web.Mvc;

namespace PressFitApi.Controllers
{
    [Authorize]
    public class BroadcastController : Controller
    {
        private PressFitApiContext db = new PressFitApiContext();
        // GET: Broadcast
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(Broadcast broadcastModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase uploadedFile = Request.Files[file];
                        SaveFile(uploadedFile);
                        string hostName = Request.Url.Host.ToString();

                        var httpPath = string.Empty;
                        var urlPath = string.Empty;
                        if (uploadedFile.FileName != string.Empty)
                        {
                            httpPath = @Url.Content("~/BroadcastUploads/" + uploadedFile.FileName);
                            urlPath = "http://" + Request.Url.Authority + httpPath;
                        }

                        //string returnUrl = VirtualPathUtility.ToAbsolute("~/BroadcastUploads/"+ uploadedFile.FileName);

                        if (urlPath != string.Empty)
                        {
                            Broadcast(urlPath, broadcastModel);
                        }
                        else
                        {
                            Broadcast(string.Empty, broadcastModel);
                        }
                        ModelState.Clear();
                        TempData["Success"] = "Broadcasted Successfully!";
                        return View("Index");
                        //SaveFile(uploadedFile, ref product);
                        //Save file here
                    }
                }
                return View("Index");
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        private void Broadcast(string httpPath, Broadcast broadcastModel)
        {
            try
            {

                SendIOSNotification(httpPath, broadcastModel);
                SendAndroidNotification(httpPath, broadcastModel);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void SendAndroidNotification(string httpPath, Broadcast broadcastModel)
        {
            try
            {

                List<string> tokenList = db.Token.Where(y => y.ChannelId.ToLower() == "android" && y.TokenId != null).Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToList();
                int tokenCount = tokenList.Count;
                int outerLoopCount = tokenCount / 1000;
                int lastNotificationCount = tokenCount % 1000;
                int notificationCounter = 0;
                int loopLength = 1000;
                string tokenIds = string.Empty;

                //to check token less than 1000
                if (tokenCount < 1000)
                {
                    loopLength = tokenCount;
                }
                
                for (int i = 0; i <= outerLoopCount; i++)
                {
                    tokenIds = getAndroidTokenIdBetweenNos(notificationCounter, loopLength, tokenList);
                    sendFCMMessage(tokenIds, broadcastModel, httpPath);

                    // increasing loop count 
                    if (i == outerLoopCount - 1)
                    {
                        notificationCounter = notificationCounter + 1000;
                        loopLength = lastNotificationCount;
                        //notificationCounter = notificationCounter + lastNotificationCount;
                    }
                    else
                    {
                        notificationCounter = notificationCounter + 1000;
                        loopLength = notificationCounter + 1000;
                    }

                }
                //string tokenIds = getAndroidTokenId();
                //string tokenIds = getAndroidTokenIdCount(100);





            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void SendIOSNotification(string httpPath, Broadcast broadcastModel)
        {
            try
            {
                string[] TokenIds = getIOSTokenId();
                //Get Certificate
                var appleCert = System.IO.File.ReadAllBytes(Server.MapPath("~/Files/APNS_PROD_Certificates.p12"));
                //var appleCert = System.IO.File.ReadAllBytes(Server.MapPath("~/Files/APNS_DEV_Certificates.p12"));
                // Configuration (NOTE: .pfx can also be used here)
                var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, appleCert, "");

                // Create a new broker

                foreach (var deviceToken in TokenIds)
                {
                    var apnsBroker = new ApnsServiceBroker(config);

                    //8FA978DB52FC2B09D79F039C9928BF4867A01B106689232F8D75234F37D04396


                    ////Get Certificate
                    //var appleCert = System.IO.File.ReadAllBytes(Server.MapPath("~/Files/APNS_PROD_Certificates.p12"));

                    //// Configuration (NOTE: .pfx can also be used here)
                    //var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, appleCert, "");

                    //// Create a new broker
                    //var apnsBroker = new ApnsServiceBroker(config);

                    // Wire up events
                    apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
                    {
                        aggregateEx.Handle(ex =>
    {
        // See what kind of exception it was to further diagnose
        if (ex is ApnsNotificationException)
        {
            var notificationException = (ApnsNotificationException)ex;

            // Deal with the failed notification
            var apnsNotification = notificationException.Notification;
            var statusCode = notificationException.ErrorStatusCode;
            string desc = $"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}";
            //               Console.WriteLine(desc);
            //lblStatus.Text = desc;
            //return  new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        else
        {
            string desc = $"Apple Notification Failed for some unknown reason : {ex.InnerException}";
            // Inner exception might hold more useful information like an ApnsConnectionException			
            Console.WriteLine(desc);
            //lblStatus.Text = desc;
        }

        // Mark it as handled
        return true;
    });
                    };

                    apnsBroker.OnNotificationSucceeded += (notification) =>
                    {
                        //lblStatus.Text = "Apple Notification Sent successfully!";

                        var test = notification;
                    };

                    var fbs = new FeedbackService(config);
                    fbs.FeedbackReceived += (string devicToken, DateTime timestamp) =>
                    {
                        // Remove the deviceToken from your database
                        // timestamp is the time the token was reported as expired
                    };

                    // Start Proccess 
                    apnsBroker.Start();
                    var jsonObject = JObject.Parse("{\"aps\":{ \"sound\":\"default\",\"alert\":{\"body\":\"" + broadcastModel.Message + "\",\"title\":\"" + broadcastModel.Title + "\"},\"mutable-content\":1},\"media-url\":\"" + httpPath + "\"}");
                    //var jsonObject = "{\"data\":{\"body\":\"" + broadcastModel.Message + "\",\"message\":\"" + broadcastModel.Title + "\",\"url\":\"" + httpPath + "\"}}";
                    //var jsonObject = JObject.Parse(("{\"aps\":{\"badge\":1,\"sound\":\"oven.caf\",\"alert\":\"" + (broadcastModel.Message + "\"}}")));
                    if (deviceToken != "")
                    {
                        apnsBroker.QueueNotification(new ApnsNotification
                        {
                            DeviceToken = deviceToken,
                            Payload = jsonObject
                            //Payload = JObject.Parse(("{\"aps\":{\"badge\":1,\"sound\":\"oven.caf\",\"alert\":\"" + (message + "\"}}")))
                        });
                    }

                    apnsBroker.Stop();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void sendFCMMessage(string tokenIds, Broadcast broadcastModel, string httpPath)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/send");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            string key = "AIzaSyCeWSgk4KlqsoqIMA0XRuSs___jN2lMy4I";

            client.DefaultRequestHeaders.Add("Authorization", " key = AIzaSyCeWSgk4KlqsoqIMA0XRuSs___jN2lMy4I");
            // var jsonObject = "{\"registration_ids\":[\"fP1x1T4AQYo:APA91bEu_aEVl-kv8PoUtUTTAykVX_-45XsB_b_bvBR5SArTLERX3mAM0-Yp369tRNUK4T3lvx5ohfkwLHYITqznIJDBns4X-HzCuDcqb_0XeFzRUT9pep38QiN0sTx_A4vty8_aaVdt\",\"fRjyHmcLdaM:APA91bGXK0ggaPrebWjeqlpGp2HLMm36hYEPGCj1hpkzEeikdBAw_lDOb-oid3ExtTYYzX20KIokZofPSAvffDDv7WW5oa3LZfdvsw7Zsgea6GuJ9oUYoHMsxvqMgUc8R15UkasLD2Cz\"],\"collapse_key\":\"type_a\",\"content_available\":true,\"mutable-content \":true,\"data\":{\"body\":\"New Diwali Diyas\",\"message\":\"Diwali offers\",\"url\":\"https://static1.squarespace.com/static/55705e90e4b03651f8736427/t/557062f6e4b0e9b175a7d533/1536068419238/?format=1500w\"}}";

            var jsonObject = "{\"registration_ids\":[" + tokenIds + "],\"collapse_key\":\"type_a\",\"content_available\":true,\"mutable-content \":true,\"data\":{\"body\":\"" + broadcastModel.Message + "\",\"message\":\"" + broadcastModel.Title + "\",\"url\":\"" + httpPath + "\"}}";
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(client.BaseAddress, content).Result;
        }


        private string[] getIOSTokenId()
        {
            try
            {


                string[] token_array = db.Token.Where(y => y.ChannelId.ToLower() == "ios").Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToArray();
                // string tokenIds = string.Join(",", token_array.Select(f => "\"" + f + "\""));
                string tokenIds = string.Join(",", token_array);

                return token_array;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private string getAndroidTokenId()
        {
            try
            {
                string[] token_array = db.Token.Where(y => y.ChannelId.ToLower() == "android" && y.TokenId != null).Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToArray();
                string tokenIds = string.Join(",", token_array.Select(f => "\"" + f + "\""));
                //string tokenIds = string.Join(",", token_array);

                return tokenIds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string getAndroidTokenIdBetweenNos(int fromNumber, int toNumber, List<string> tokenList)
        {
            try
            {
                //string[] token_array = db.Token.Where(y => y.ChannelId.ToLower() == "android" && y.TokenId != null).Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToArray();

                // List<string> tokenList = db.Token.Where(y => y.ChannelId.ToLower() == "android" && y.TokenId != null).Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToList();
                //string[] token_array = db.Token.Where(d => d.Id >= fromNumber
                //                        && d.Id <= toNumber).Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToArray();

                string[] token_array = tokenList.GetRange(fromNumber, toNumber).Select(i => i.ToString()).ToArray();
                string tokenIds = string.Join(",", token_array.Select(f => "\"" + f + "\""));

                return tokenIds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private int getAndroidTokenCount()
        {
            try
            {
                int tokenCount = db.Token.Where(y => y.ChannelId.ToLower() == "android" && y.TokenId != null).Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToArray().Count();
                //string tokenIds = string.Join(",", token_array);

                return tokenCount;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private string[] getAndroidTokenIdArray()
        {
            try
            {
                string[] token_array = db.Token.Where(y => y.ChannelId.ToLower() == "android" && y.TokenId != null).Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToArray();
                return token_array;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void SaveFile(HttpPostedFileBase file)
        {
            try
            {
                var broadcastPath = Server.MapPath("~/BroadcastUploads");
                if (!Directory.Exists(broadcastPath))
                {
                    Directory.CreateDirectory(broadcastPath);
                }
                var httpPath = string.Empty;
                if (file.ContentLength > 0 && file.ContentType.Contains("image"))
                {
                    var newpath = Path.Combine(Server.MapPath("~/BroadcastUploads"), file.FileName);
                    DeleteExistFile(broadcastPath);
                    file.SaveAs(newpath);
                    //httpPath = @Url.Content("~/BroadcastUploads/" + file.FileName);
                }

                //if (httpPath != string.Empty)
                //{
                //    Broadcast(httpPath);
                //}

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private void DeleteExistFile(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}