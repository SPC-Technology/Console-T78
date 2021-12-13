using RabbitMQ_EInvoiceT78_Consumer.Base;
using RabbitMQ_EInvoiceT78_Consumer.DataAccess_Cloud;
using RabbitMQ_EInvoiceT78_Consumer.Log_Event.Log_Evnet_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static RabbitMQ_EInvoiceT78_Consumer.Base.TVAN_CONST;
using pbs.Helper;
using System.Xml;
using Newtonsoft.Json;
using RabbitMQ_EInvoiceT78_Consumer.Model;

namespace RabbitMQ_EInvoiceT78_Consumer.Handle_Response
{
    public class HandleResponse
    {
        public async Task<string> GetResponse(string message)
        {
            try
            {
                var doc = XElement.Parse(message);
                var MLTDiep = (from itm in doc.Descendants("TTChung") select itm.GetString("MLTDiep")).FirstOrDefault();
                var MST = (from itm in doc.Descendants("TTChung") select itm.GetString("MST")).FirstOrDefault();

                switch (MLTDiep)
                {
                    case MA_THONG_DIEP.TechnicalFeedBack:
                        {
                            var pushEvent = new DataAccess();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentTechnicalFeedBack(message), MST);
                        }
                    case MA_THONG_DIEP.RecieveRegisterInvoice:
                        {
                            var pushEvent = new DataAccess();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentRecieveRegisterInvoiceFromCQT(message), MST);
                        }
                    case MA_THONG_DIEP.AcceptRegisterInvoice:
                        {
                            var pushEvent = new DataAccess();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentAcceptRegisterInvoiceFromCQT(message), MST);
                        }
                    case MA_THONG_DIEP.SuccessRequestCodeInv:
                        {
                            var pushEvent = new DataAccess();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentRecieveInvoiceSuccessRequestCodeFromCQT(message), MST);
                        }
                    case MA_THONG_DIEP.ErrorRequestCodeInv:
                        {
                            var pushEvent = new DataAccess();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentErrorInvoiceRequestCodeFormCQT(message), MST);
                        }
                }
                return TVAN_CONST.TAG_QUEUE.ERROR;
            }
            catch (Exception ex)
            {
                return TVAN_CONST.TAG_QUEUE.ERROR;
            }

        }


        private LogEventModel GetTTChungTechnicalFeedBack(XElement doc)
        {
            var logObj = new Log_Event.Log_Evnet_Model.LogEventModel();
            logObj.MaThongDiep = (from itm in doc.Descendants("TTChung") select itm.GetString("MTDiep")).FirstOrDefault();
            logObj.MaThongDiepThamChieu = (from itm in doc.Descendants("TTChung") select itm.GetString("MTDTChieu")).FirstOrDefault();
            logObj.MLTDiep = (from itm in doc.Descendants("TTChung") select itm.GetString("MLTDiep")).FirstOrDefault();
            logObj.NoiGui = (from itm in doc.Descendants("TTChung") select itm.GetString("MNGui")).FirstOrDefault();
            logObj.NoiNhan = (from itm in doc.Descendants("TTChung") select itm.GetString("MNNhan")).FirstOrDefault();
            logObj.ThongDiep = new TVAN_CONST.TCT_THONGDIEP().DescriptionMessage(logObj.MLTDiep.ToInteger());
            logObj.FileData = doc.ToString().Compress();
            return logObj;
        }


        //Handle feedback technical
        private async Task<LogEventModel> HandleResponseContentTechnicalFeedBack(string message)
        {
            var logObj = new Log_Event.Log_Evnet_Model.LogEventModel();
            var doc = XElement.Parse(message);
            var MST = (from itm in doc.Descendants("TTChung") select itm.GetString("MST")).FirstOrDefault();
            var MaLoi = (from itm in doc.Descendants("TBao") select itm.GetString("TTTNhan")).FirstOrDefault();
            logObj = GetTTChungTechnicalFeedBack(doc);
            logObj.NgayGiaoDich = (from itm in doc.Descendants("TBao") select itm.GetString("NNhan")).FirstOrDefault();

            if (MaLoi == "1")
            {
                logObj.TrangThai = TVAN_CONST.STATUS_RESPONSE.LOI;
                var updateStatusMaster = new RegisterInvoiceEvent();
                await updateStatusMaster.UpdateStatusRegisterInvoiceMaster(logObj.MaThongDiepThamChieu, MST, TVAN_CONST.STATUS_RESPONSE.LOI);
            }
            else
            {
                logObj.TrangThai = TVAN_CONST.STATUS_RESPONSE.DANHAN;
            }
            return logObj;

        }

        //Handle feedback register invoice from CQT
        private async Task<LogEventModel> HandleResponseContentRecieveRegisterInvoiceFromCQT(string message)
        {
            var logObj = new Log_Event.Log_Evnet_Model.LogEventModel();
            var doc = XElement.Parse(message);
            var MST = (from itm in doc.Descendants("TTChung") select itm.GetString("MST")).FirstOrDefault();
            logObj = GetTTChungTechnicalFeedBack(doc);
            logObj.NgayGiaoDich = (from itm in doc.Descendants("DLieu").Descendants("TBao").Descendants("DLTBao") select itm.GetString("TGNhan")).FirstOrDefault();
            var MaLoi = (from itm in doc.Descendants("DLieu").Descendants("TBao").Descendants("DLTBao") select itm.GetString("THop")).FirstOrDefault();
            if (MaLoi == "1")
            {
                logObj.TrangThai = TVAN_CONST.STATUS_RESPONSE.DAXULI;
            }
            else
            {
                logObj.TrangThai = TVAN_CONST.STATUS_RESPONSE.LOI;
                var updateStatusMaster = new RegisterInvoiceEvent();
                await updateStatusMaster.UpdateStatusRegisterInvoiceMaster(logObj.MaThongDiepThamChieu, MST, TVAN_CONST.STATUS_RESPONSE.DKLOI);
            }
            return logObj;
        }

        private async Task<LogEventModel> HandleResponseContentAcceptRegisterInvoiceFromCQT(string message)
        {
            var logObj = new Log_Event.Log_Evnet_Model.LogEventModel();
            var doc = XElement.Parse(message);
            var MST = (from itm in doc.Descendants("TTChung") select itm.GetString("MST")).FirstOrDefault();
            logObj = GetTTChungTechnicalFeedBack(doc);

            logObj.NgayGiaoDich = (from itm in doc.Descendants("DLieu").Descendants("TBao").Descendants("STBao") select itm.GetString("NTBao")).FirstOrDefault();
            var MaLoi = (from itm in doc.Descendants("DLieu").Descendants("TBao").Descendants("DLTBao") select itm.GetString("TTXNCQT")).FirstOrDefault();
            if (MaLoi == "1")
            {
                logObj.TrangThai = TVAN_CONST.STATUS_RESPONSE.DKTHANHCONG;
                var updateStatusMaster = new RegisterInvoiceEvent();
                await updateStatusMaster.UpdateStatusRegisterInvoiceMaster(logObj.MaThongDiepThamChieu, MST, TVAN_CONST.STATUS_RESPONSE.DKTHANHCONG);
            }
            else
            {
                logObj.TrangThai = TVAN_CONST.STATUS_RESPONSE.DKLOI;
                var updateStatusMaster = new RegisterInvoiceEvent();
                await updateStatusMaster.UpdateStatusRegisterInvoiceMaster(logObj.MaThongDiepThamChieu, MST, TVAN_CONST.STATUS_RESPONSE.DKLOI);
            }
            return logObj;
        }


        //Handle feedback send invocie request code fromCQT
        private async Task<LogEventModel> HandleResponseContentRecieveInvoiceSuccessRequestCodeFromCQT(string message)
        {
            var logObj = new Log_Event.Log_Evnet_Model.LogEventModel();
            var doc = XElement.Parse(message);
            var MST = (from itm in doc.Descendants("TTChung") select itm.GetString("MST")).FirstOrDefault();
            logObj = GetTTChungTechnicalFeedBack(doc);
            logObj.TrangThai = TVAN_CONST.STATUS_RESPONSE.SUCCESS_REQUEST_CODE;
            logObj.NgayGiaoDich = GetTimeStamp(message);

            //Handle push xml signed file and pdf file to Cloud
            var objInv = new InvoiceModel();
            objInv.MCCQT = (from itm in doc.Descendants("DLieu").Descendants("HDon") select itm.GetString("MCCQT")).FirstOrDefault();
            objInv.ProformaNo = (from itm in doc.Descendants("DLieu").Descendants("HDon").Descendants("TTin") where itm.GetString("TTruong") == "ProformaNo" select itm.GetString("DLieu")).FirstOrDefault();
            objInv.MaThongDiep = logObj.MaThongDiepThamChieu;
            objInv.fileName=(from itm in doc.Descendants("DLieu").Descendants("HDon").Descendants("TTin") where itm.GetString("TTruong") == "ExtDesc2" select itm.GetString("DLieu")).FirstOrDefault();
            
            //Upload pdfFile and send mail to customer
            var invRequest = new InvoiceRequestCode();
            await invRequest.UploadSignedPdfXml(objInv,MST);

            //update MCCQT and satus to Invoie Request Code table cloud
            await invRequest.UpdateMCCQTAndChangeStatus(objInv, MST);

            return logObj;
        }

        private async Task<LogEventModel> HandleResponseContentErrorInvoiceRequestCodeFormCQT(string message)
        {
            var logObj = new Log_Event.Log_Evnet_Model.LogEventModel();
            var doc = XElement.Parse(message);
            var MST = (from itm in doc.Descendants("TTChung") select itm.GetString("MST")).FirstOrDefault();
            logObj = GetTTChungTechnicalFeedBack(doc);
            logObj.TrangThai = TVAN_CONST.STATUS_RESPONSE.ERROR_REQUEST_CODE;
            logObj.NgayGiaoDich = GetTimeStamp(message);

            //Update status to Invoie Request Code table cloud
            var invRequest = new InvoiceRequestCode();
            await invRequest.ChangeStatusError(logObj.MaThongDiepThamChieu, MST);
            return logObj;
        }

        //Get timestamp server
        private string GetTimeStamp(string message)
        {
            //Get TimeStamp from TimeStamp Server
            //Add TmeStamp
            var pdoc = new XmlDocument();
            pdoc.LoadXml(message);
            var timeStamp = new TimeStamp_Server.TimeStamp();
            var TSA = timeStamp.GetSignedHashFromTsa(pdoc);
            var tsaHash = timeStamp.GetTsaTimeFromSignedHash(TSA);
            string nTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nTimeZoneKey);
            return TimeZoneInfo.ConvertTimeFromUtc(tsaHash, nzTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
