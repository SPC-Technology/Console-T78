using pbs.Helper;
using RabbitMQ_EInvoiceT78_Consumer.Base;
using RabbitMQ_EInvoiceT78_Consumer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RabbitMQ_EInvoiceT78_Consumer.DataAccess_Cloud
{
    public class InvoiceRequestCode
    {
        //Update MCCQT to table EInvocieRequestCodeT78 on cloud
        public async Task UpdateMCCQTAndChangeStatus(InvoiceModel objInv,string MST)
        {
            var tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.TableCloudRequest.STR_SendEinvoiceCodeT78}{MST.Replace("-","")}");
            await tableService.CreateTableIfNotExistsAsync();
            var data = await tableService.ReadAsync($"{MST}:{objInv.MaThongDiep}");
            data["MCCQT"] = objInv.MCCQT;
            data["STATUS"] = TVAN_CONST.STATUS_RESPONSE.SUCCESS_REQUEST;
            await tableService.WriteAsync(data);
        }


        public async Task ChangeStatusError(string MaThongDiep,string MST)
        {
            var tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.TableCloudRequest.STR_SendEinvoiceCodeT78}{MST.Replace("-", "")}");
            await tableService.CreateTableIfNotExistsAsync();
            var data = await tableService.ReadAsync($"{MST}:{MaThongDiep}");
            data["STATUS"] = TVAN_CONST.STATUS_RESPONSE.ERROR_REQUEST;
            await tableService.WriteAsync(data);
        }

        public async Task UploadSignedPdfXml(InvoiceModel objInv,string MST)
        {
            //add pdf file
            var rootFilePath= string.Format($"{System.IO.Directory.GetCurrentDirectory()}");
            var serviceFactory = SPC.ServicesContainer.Get<SPC.Services.Cloud.IBlobFactory>();
            var blobService = await serviceFactory.GetBlobClientAsync($"{TVAN_CONST.STR_LavaData}.signed-{MST}");
            await blobService.DownloadAsync($"{objInv.fileName}",$"{rootFilePath}");

            //Add MCCQT in pdf file
            var pdfoptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            pdfoptions.Add("Text", $"Mã của cơ quan thuế (Tax Authority Code): {objInv.MCCQT}");
            pdfoptions.Add("X", "-11");
            pdfoptions.Add("Y", "400");
            pdfoptions.Add("Fontsize", "8");
            var pdfFile = await SPC.ServicesContainer.Get<SPC.Services.DocumentAPI.ISignPdf>().PutStampAsync($"{rootFilePath}\\{objInv.fileName}", pdfoptions);

            //Upload to cloud pdf file
            var arg = new pbsCmdArgs(string.Format("pbs.BO.Azure.BlobUploader?$account=lavadata&$permission=B&$container=signed-{0}&$file={1}&$mode=I", MST.ToLower(), pdfFile));
            pbs.Helper.UIServices.RunURLService.Run(arg);
            var theUri = arg.GetOutputVariable<string>();

            //push xmlfile to cloud


        }
    }
}
 