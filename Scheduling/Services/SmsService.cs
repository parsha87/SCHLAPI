using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Data.EventEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Scheduling.Services
{
    public interface ISmsService
    {
        void SendSMS(string number, string smsText, string smsToSendUserId, string UserId);
        void LogSMS(string number, string smsText, string smsToSendUserId, string UserId, string ResponsefromServer);
    }

    public class SmsService: ISmsService
    {
        private readonly IMapper _mapper;
        private EventDBContext _eventDBContext;
        private IZoneTimeService _zoneTimeService;
        private readonly ILogger<ProjectService> _logger;

        public SmsService(EventDBContext eventDBContext, IZoneTimeService zoneTimeService, ILogger<ProjectService> logger, IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _zoneTimeService = zoneTimeService;
            _eventDBContext = eventDBContext;
        }

        /// <summary>
        /// send sms
        /// </summary>
        /// <param name="number"></param>
        /// <param name="smsText"></param>
        /// <param name="smsToSendUserId"></param>
        /// <param name="UserId"></param>
        public void SendSMS(string number, string smsText, string smsToSendUserId, string UserId)
        {
            string pagesource = "";
            try
            {
                string url = "http://jisl.co.in/test/sms/index.php";
                //WebClient webClient = new WebClient();

                //NameValueCollection formData = new NameValueCollection();
                ////formData["num"] = "9028183919";
                ////formData["mtr"] = "Test Messsage";
                //formData.Add("num", number);
                //formData.Add("mtr", HttpUtility.UrlEncode(smsText));
                ////webClient.UploadValues(url, formData);

                //byte[] responseBytes = webClient.UploadValues(url, "POST", formData);
                //string responsefromserver = Encoding.UTF8.GetString(responseBytes);
                ////TextArea1.InnerText = responsefromserver;
                //webClient.Dispose();

                using (WebClient client = new WebClient())
                {
                    System.Collections.Specialized.NameValueCollection postData =
                        new System.Collections.Specialized.NameValueCollection()
                            {
                                  { "num", number },
                                  { "mtr", HttpUtility.UrlEncode(smsText) }
                            };
                    pagesource = Encoding.UTF8.GetString(client.UploadValues(url, postData));

                }
            }
            catch (Exception ex)
            {
                pagesource = ex.ToString();
                _logger.LogError($"[{nameof(SmsService)}.{ nameof(SendSMS) }]{ex}");
            }
            finally
            {
                LogSMS(number, smsText, smsToSendUserId, UserId, pagesource);
            }
        }



        #region private functions

        /// <summary>
        /// log sms
        /// </summary>
        /// <param name="number"></param>
        /// <param name="smsText"></param>
        /// <param name="smsToSendUserId"></param>
        /// <param name="UserId"></param>
        /// <param name="ResponsefromServer"></param>
        public async void LogSMS(string number, string smsText, string smsToSendUserId, string UserId, string ResponsefromServer)
        {
            try
            {
                string strLogInfo = string.Empty;
                string strLogSMS = string.Empty;
                var dateTime = await _zoneTimeService.TimeZoneDateTime();
                strLogInfo = String.Format("{0:dd-MM-yyyy} {1}", dateTime, dateTime.ToLongTimeString());
                strLogSMS = String.Format("{0} ## Mob. No.: {1} ## MSG: {2}", strLogInfo, number, smsText);

                _logger.LogInformation(strLogSMS);

                #region add it to SMSLog table in event db
                var datetime = await _zoneTimeService.TimeZoneDateTime();
                Data.EventEntities.Smslog smsLogObj = new Data.EventEntities.Smslog();
                smsLogObj.LoginUserId = "fdf";
                smsLogObj.SmsToSendUserId = smsToSendUserId;
                smsLogObj.SmsText = smsText;
                smsLogObj.SmssendDateTime = datetime;
                smsLogObj.ResponsefromServer = ResponsefromServer;
                _eventDBContext.Smslog.Add(smsLogObj);
                _eventDBContext.SaveChanges();

                #endregion

                //string strPath = HttpContext.Current.Server.MapPath("../LogSMS/");
                //string strFileName = string.Empty;
                //string strLogTime = string.Empty;
                //StreamWriter writer = null;
                //if (!Directory.Exists(strPath))
                //{
                //    Directory.CreateDirectory(strPath);
                //}


                //strLogTime = String.Format("{0:dd-MM-yyyy} {1}", DateTime.Now, DateTime.Now.ToLongTimeString());
                //strFileName = String.Format("{0}{1:yyyyMMdd}.txt", strPath + "SMSLog", DateTime.Now);
                //if (!File.Exists(strFileName))
                //    writer = new StreamWriter(strFileName, false);
                //else
                //    writer = new StreamWriter(strFileName, true);
                //writer.WriteLine(String.Format("{0} ## Mob. No.: {1} ## MSG: {2}", strLogTime, number, smsText));
                ////writer.WriteLine("---");

                //writer.Flush();
                //writer.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SmsService)}.{ nameof(LogSMS) }]{ex}");
            }
        }
        #endregion
    }
}
