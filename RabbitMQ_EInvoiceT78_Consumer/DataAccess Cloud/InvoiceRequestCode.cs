using pbs.BO;
using pbs.Helper;
using RabbitMQ_EInvoiceT78_Consumer.Base;
using RabbitMQ_EInvoiceT78_Consumer.Model;
using SPC.Services.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RabbitMQ_EInvoiceT78_Consumer.DataAccess_Cloud
{
    public class InvoiceRequestCode
    {
        //Update MCCQT to table EInvocieRequestCodeT78 on cloud
        public async Task UpdateMCCQTAndChangeStatus(InvoiceModel objInv, string MST)
        {
            var tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.TableCloudRequest.STR_SendEinvoiceCodeT78}{MST.Replace("-", "")}");
            await tableService.CreateTableIfNotExistsAsync();
            var data = await tableService.ReadAsync($"{MST}:{objInv.MaThongDiep}");
            data["MCCQT"] = objInv.MCCQT;
            data["STATUS"] = TVAN_CONST.STATUS_RESPONSE.SUCCESS_REQUEST;
            await tableService.WriteAsync(data);
        }


        public async Task ChangeStatusError(string MaThongDiep, string MST)
        {
            var tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.TableCloudRequest.STR_SendEinvoiceCodeT78}{MST.Replace("-", "")}");
            await tableService.CreateTableIfNotExistsAsync();
            var data = await tableService.ReadAsync($"{MST}:{MaThongDiep}");
            data["STATUS"] = TVAN_CONST.STATUS_RESPONSE.ERROR_REQUEST;
            await tableService.WriteAsync(data);
        }

        public async Task UploadSignedPdf(InvoiceModel objInv, string MST)
        {
            //add pdf file
            var rootFilePath = string.Format($"{System.IO.Directory.GetCurrentDirectory()}");
            var serviceFactory = SPC.ServicesContainer.Get<SPC.Services.Cloud.IBlobFactory>();
            var blobService = await serviceFactory.GetBlobClientAsync($"{TVAN_CONST.STR_LavaData}.signed-{MST}");
            await blobService.DownloadAsync($"{objInv.fileName}", $"{rootFilePath}");

            //Add MCCQT in pdf file
            var pdfoptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            pdfoptions.Add("Text", $"Mã của cơ quan thuế (Tax Authority Code): {objInv.MCCQT}");
            pdfoptions.Add("X", "-11");
            pdfoptions.Add("Y", "400");
            pdfoptions.Add("Fontsize", "8");
            var pdfFile = await SPC.ServicesContainer.Get<SPC.Services.DocumentAPI.ISignPdf>().PutStampAsync($"{rootFilePath}\\{objInv.fileName}", pdfoptions);

            //Upload to cloud pdf file
            var cmd = new pbs.Helper.pbsCmdArgs($"pbs.BO.Azure.BlobUploader?$account=lavadata&$permission=B&$container=signed-{MST.ToLower()}&$file={pdfFile}&$mode=I");
            var runable = new pbs.BO.Azure.BlobUploader();
            runable.Run(cmd);
            var ret = cmd.GetOutputVariable<string>();

            //Send mail to customer
            SendAzureLinkToCustomer(ret, objInv);
        }

        public async Task UploadSignedXml(string message,InvoiceModel pInv,string MST)
        {
            var rootFilePath = string.Format($"{System.IO.Directory.GetCurrentDirectory()}");
            var theXmlFileName = string.Format($"{rootFilePath}\\{pInv.fileName.Replace("pdf","xml")}");
            var theDoc = new XmlDocument();
            theDoc.LoadXml(message);
            theDoc.Save(theXmlFileName);
            var cmd = new pbs.Helper.pbsCmdArgs($"pbs.BO.Azure.BlobUploader?$account=lavadata&$permission=B&$container=signed-{MST.ToLower()}&$file={theXmlFileName}&$mode=I");
            var runable = new pbs.BO.Azure.BlobUploader();
            runable.Run(cmd);
        }
        private void SendAzureLinkToCustomer(string AzureUrl, InvoiceModel pInv)
        {
            string theContent = GetIssuedInvoiceNotificationContent(pInv, AzureUrl);

            var NtfDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            NtfDic.Add(nameof(pbs.BO.Mail.NTF.SendTo), pInv.CusMail);

            var sbj = ("Your Invoice# {0}/{1}").Replace("{0}", pInv.Serial);
            sbj = sbj.Replace("{1}", pInv.InvNo);

            NtfDic.Add(nameof(pbs.BO.Mail.NTF.Subject), sbj);
            NtfDic.Add(nameof(pbs.BO.Mail.NTF.Body), theContent);

            NtfDic.Add(nameof(pbs.BO.Mail.NTF.ClassId), "SINV");
            NtfDic.Add(nameof(pbs.BO.Mail.NTF.Reference), pInv.ProformaNo);
            NtfDic.Add(nameof(pbs.BO.Mail.NTF.MsgType), "INV_NOTI");
            pbs.Helper.MessageServices.SendMailService.SendNotification(NtfDic, null);
        }

        private static string GetIssuedInvoiceNotificationContent(InvoiceModel pInv, string AzureUrl)
        {
            string theContent = "";
            theContent = new XElement("content", $"Dear {pInv.ClientName} **, Below is the link for your invoice ** # {pInv.Serial}/{pInv.InvNo} from: {pInv.InvoiceDate} **:").ToString();
            theContent = pbs.BO.DM.MarkDownGenerator.ToHtml(theContent);
            return theContent;
        }

        public async Task DeleteFileInServer()
        {
            var rootFilePath = string.Format($"{System.IO.Directory.GetCurrentDirectory()}");

        }
    }
}
