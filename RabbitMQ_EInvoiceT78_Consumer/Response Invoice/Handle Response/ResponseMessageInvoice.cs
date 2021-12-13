
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using pbs.Helper;
using RabbitMQ_EInvoiceT78_Consumer.Base;
using RabbitMQ_EInvoiceT78_Consumer.DataAccess_Cloud;
using RabbitMQ_EInvoiceT78_Consumer.Log_Event.Log_Evnet_Model;
using static RabbitMQ_EInvoiceT78_Consumer.Base.TVAN_CONST;

namespace RabbitMQ_EInvoiceT78_Consumer.Register_Invoice.Handle_Response
{
    public class ResponseMessageRegisterInvoice
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
                            var pushEvent = new RegisterInvoiceEvent();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentTechnicalFeedBack(message), MST);
                        }
                    case MA_THONG_DIEP.RecieveRegisterInvoice:
                        {
                            var pushEvent = new RegisterInvoiceEvent();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentRecieveRegisterInvoiceFromCQT(message), MST);
                        }
                    case MA_THONG_DIEP.AcceptRegisterInvoice:
                        {
                            var pushEvent = new RegisterInvoiceEvent();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentAcceptRegisterInvoiceFromCQT(message), MST);
                        }
                    case MA_THONG_DIEP.SuccessRequestCodeInv:
                        {
                            var pushEvent = new RegisterInvoiceEvent();
                            return await pushEvent.PushEventFeedBack(await HandleResponseContentRecieveInvoiceSuccessRequestCodeFromCQT(message),MST);
                        }
                }
                //if (MLTDiep == MA_THONG_DIEP.TechnicalFeedBack)
                //{
                //    var pushEvent = new RegisterInvoiceEvent();
                //    return await pushEvent.PushEventTechnicalFeedBack(await HandleResponseContentTechnicalFeedBack(message), MST);
                //}
                //else if(MLTDiep==MA_THONG_DIEP.RecieveRegisterInvoice)
                //{
                //    var pushEvent = new RegisterInvoiceEvent();
                //    return await pushEvent.PushEventTechnicalFeedBack(await HandleResponseContentRecieveFromCQT(message,MLTDiep), MST);
                //}
                //else if(MLTDiep==MA_THONG_DIEP.AcceptRegisterInvoice)
                //{
                //    var pushEvent = new RegisterInvoiceEvent();
                //    return await pushEvent.PushEventTechnicalFeedBack(await HandleResponseContentRecieveFromCQT(message,MLTDiep), MST);
                //}
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
            return logObj;
        }
    }
}
