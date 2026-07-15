using pbs.BO;
using pbs.Helper;
using RabbitMQ_EInvoiceT78_Consumer.Base;
using RabbitMQ_EInvoiceT78_Consumer.Model;
using SPC.Services.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
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
            data["PDFFileName"] = objInv.fileName;
            await tableService.WriteAsync(data);
        }

        public async Task UpdateInvoiceStatusInvoiceWithoutCode(string MaThongDiep,  string MST)
        {
            var tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.TableCloudRequest.STR_SendEinvoiceCodeT78}{MST.Replace("-", "")}");
            await tableService.CreateTableIfNotExistsAsync();
            var data = await tableService.ReadAsync($"{MST}:{MaThongDiep}");
            if( data.Count!=0)
            {
                var objInv = new InvoiceModel();
                objInv.fileName = (from itm in XElement.Parse(data["Data"].ToString().Decompress()).Descendants("DLieu").Descendants("HDon").Descendants("TTin") where itm.GetString("TTruong") == "PDFFileName" select itm.GetString("DLieu")).FirstOrDefault();
                var xmlFileName = await UploadSignedXml(data["Data"].ToString().Decompress(), objInv, MST);
                data["PDFFileName"] = xmlFileName;
                data["STATUS"] = TVAN_CONST.STATUS_RESPONSE.SUCCESS_REQUEST;
                await tableService.WriteAsync(data);
            }
            else
            {
                tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.TableCloudRequest.STR_SendPOSEinvoiceCodeT78}{MST.Replace("-", "")}");
                data = await tableService.ReadAsync($"{MST}:{MaThongDiep}");
                if (data.Count != 0) {
                    var objInv = new InvoiceModel();
                    data["STATUS"] = TVAN_CONST.STATUS_RESPONSE.SUCCESS_REQUEST;
                    await tableService.WriteAsync(data);
                }
            }
        }

        public async Task UpdateSoThongBaoSaiSot(string MaThongDiep,string SoThongBao, string MST)
        {
            // Nothing reliable to write - never persist an empty notification number.
            if (string.IsNullOrWhiteSpace(MST) || string.IsNullOrWhiteSpace(MaThongDiep) || string.IsNullOrWhiteSpace(SoThongBao))
                return;

            var tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.TableCloudRequest.STR_SendEinvoiceErrorT78}{MST.Replace("-", "")}");
            await tableService.CreateTableIfNotExistsAsync();
            var data = await tableService.ReadAsync($"{MST}:{MaThongDiep}");

            // ReadAsync returns an empty dictionary when the send row does not exist yet
            // (e.g. the desktop calls the TVAN API before persisting the row, so a fast CQT
            // response can arrive here first). Writing that dictionary back would create a
            // phantom row with blank PartitionKey/RowKey and silently lose the number.
            // Pin the keys explicitly so the merge always targets the correct row.
            data["PartitionKey"] = MST;
            data["RowKey"] = MaThongDiep;
            data["SOTHONGBAO"] = SoThongBao;
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
            var rootFilePath = string.Format($"{System.IO.Directory.GetCurrentDirectory()}\\SignedFile");
            //Create directory containing files
            if (!Directory.Exists(rootFilePath))
            {
                Directory.CreateDirectory(rootFilePath);
            }
            var serviceFactory = SPC.ServicesContainer.Get<SPC.Services.Cloud.IBlobFactory>();
            var blobService = await serviceFactory.GetBlobClientAsync($"{TVAN_CONST.STR_LavaData}.signed-{MST}");
            await blobService.DownloadAsync($"{objInv.fileName}", $"{rootFilePath}");

            //Add MCCQT in pdf file
            var pdfoptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            pdfoptions.Add("Text", $"Mã của cơ quan thuế (Tax Authority Code): {objInv.MCCQT}");
            pdfoptions.Add("X", "-11");
            pdfoptions.Add("Y", "350");
            pdfoptions.Add("Fontsize", "8");
            var pdfFile = await SPC.ServicesContainer.Get<SPC.Services.DocumentAPI.ISignPdf>().PutStampAsync($"{rootFilePath}\\{objInv.fileName}", pdfoptions);

            //Upload to cloud pdf file
            //var cmd = new pbs.Helper.pbsCmdArgs($"pbs.BO.Azure.BlobUploader?$account=lavadata&$permission=B&$container=signed-{MST.ToLower()}&$file={pdfFile}&$mode=I");
            //var runable = new pbs.BO.Azure.BlobUploader();
            //runable.Run(cmd);
            //var ret = cmd.GetOutputVariable<string>();

            //Send mail to customer.pdf
            //if (!String.IsNullOrEmpty(objInv.CusMail))
            //{
            //    SPC.CORE.COM.ModuleRegister.Register();
            //    await SendAzureLinkToCustomer(ret, objInv, MST);
            //}

        }

        public async Task<string> UploadSignedXml(string message,InvoiceModel pInv,string MST)
        {
            var rootFilePath = string.Format($"{System.IO.Directory.GetCurrentDirectory()}\\SignedFile");
            var theXmlFileName = string.Format($"{rootFilePath}\\{pInv.fileName.Replace("pdf", "xml")}");
            var theDoc = new XmlDocument();
            //var a = XElement.Parse(message).Descendants("HDon").FirstOrDefault().ToString().Replace("\r\n", "");
            //theDoc.PreserveWhitespace = false;
            theDoc.LoadXml(message);
            File.WriteAllText(theXmlFileName, theDoc.InnerXml);
            //theDoc.Save(theXmlFileName);
            var cmd = new pbs.Helper.pbsCmdArgs($"pbs.BO.Azure.BlobUploader?$account=lavadata&$permission=B&$container=signed-{MST.ToLower()}&$file={theXmlFileName}&$mode=I");
            var runable = new pbs.BO.Azure.BlobUploader();
            runable.Run(cmd);
            var ret = cmd.GetOutputVariable<string>();
            return ret;
        }

        private static async Task SendAzureLinkToCustomer(string AzureUrl, InvoiceModel pInv,string MST)
        {
            var srv = await SPC.ServicesContainer.ShortCut.Mail.GetSendServiceAsync();
            string theContent = GetIssuedInvoiceNotificationContent(pInv, AzureUrl,MST);

            foreach (var itm in pInv.CusMail.Split(','))
            {
                var NtfDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                NtfDic.Add(nameof(pbs.BO.Mail.NTF.SendTo), itm);

                var sbj = ("Your Invoice# {0}/{1}").Replace("{0}", pInv.Serial);
                sbj = sbj.Replace("{1}", pInv.InvNo);

                NtfDic.Add(nameof(pbs.BO.Mail.NTF.Subject), sbj);
                NtfDic.Add(nameof(pbs.BO.Mail.NTF.Body), theContent);

                NtfDic.Add(nameof(pbs.BO.Mail.NTF.ClassId), "SINV");
                NtfDic.Add(nameof(pbs.BO.Mail.NTF.Reference), pInv.ProformaNo);
                NtfDic.Add(nameof(pbs.BO.Mail.NTF.MsgType), "INV_NOTI");
                await srv.SendNotificationAsync(NtfDic, new List<string>());
            }

          
        }

        private static string GetIssuedInvoiceNotificationContent(InvoiceModel pInv, string AzureUrl,string MST)
        {
            string theContent = "<h4>This email is automatically generated by system. <b>PLEASE DO NOT REPLY</b>.</h4><br>" +
                                $"<h2><font color={"#ff6500"}>HÓA ĐƠN ĐIỆN TỬ /EInvoice</font></h2><br>" +
                                $"<p>Kính gửi: <b>{pInv.ClientName}</b>, Dear: <b>{ pInv.ClientName}</b></p>" +
                                $"<p>Mã số thuế: <b>{MST}</b>, Tax code: <b>{MST}<b>,</p>" +
                                $"<h4>Chúng tôi gửi đến Quý Công ty Hóa đơn điện tử với nội dung như sau:<br>We would like to send your company E - invoice with the content below:</h4>" +
                                $"<p>.Mã cấp bởi cơ quan thuế: <font color={"red"}><b>{pInv.MCCQT}</b></font></p>" +
                                $"<p>·Mẫu số/ Form:  <font color={"red"}><b>{pInv.KHMSHDon}</b></font><p>" +
                                $"<p>·Ký hiệu/ Serial No:  <font color={"red"}><b>{pInv.Serial}</b></font></p>" +
                                $"<p>·Số hóa đơn / Invoice No:  <font color={"red"}><b>{pInv.InvNo}</b></font></p>" +
                                $"<p>·Ngày phát hành / Issued date:  <font color={"red"}><b>{pInv.InvoiceDate}</b></font><p>" +
                                $"<p>·Đường dẫn tải hóa đơn / Link to download invoice: <b>{AzureUrl}</b></p><br>" +
                                $"<p>Mọi thắc mắc vui lòng liên hệ:</p>" +
                                $"<p>If you have any concern, please contact us:</p>" +
                                $"<h2>{pInv.ClientName}</h2>" +
                                $"<h3>Địa chỉ: <b>{pInv.Address}</b></h3>" +
                                $"<p>Xin chân thành cám ơn Quý Công ty đã hợp tác với chúng tôi! <br>Thank you for your cooperation!</p>" +
                                $"<p>Trân trọng,</p>" +
                                $"<p><i>Giải pháp Hóa đơn Điện tử được cung cấp bởi Công ty Cổ phần Công nghệ San Phú - MST: 0303430876<i></p>";

                         theContent = pbs.BO.DM.MarkDownGenerator.ToHtml(theContent);


            return theContent;
        }

            public async Task DeleteFileInServer()
        {
            var rootFilePath = string.Format($"{System.IO.Directory.GetCurrentDirectory()}\\SignedFile");
            var extensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase){ ".pdf", ".xml"};

            var files = new DirectoryInfo($"{rootFilePath}").GetFiles()
                                                 .Where(p => extensions.Contains(p.Extension));
            foreach (var file in files)
            {
                file.Attributes = FileAttributes.Normal;
                File.Delete(file.FullName);
            }
        }
    }
}
