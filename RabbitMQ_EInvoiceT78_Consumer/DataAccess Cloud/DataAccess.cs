using RabbitMQ_EInvoiceT78_Consumer.Base;
using RabbitMQ_EInvoiceT78_Consumer.Log_Event.Log_Evnet_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_EInvoiceT78_Consumer.DataAccess_Cloud
{
    public class DataAccess
    {
        //Push log event Register Invoice
        public async Task<string> PushEventFeedBack(LogEventModel obj, string TaxCode)
        {
            try
            {
                //back up spc
                var tableService1 = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.EventTableCloudResponse.EventRegisterInvoice}");
                await tableService1.CreateTableIfNotExistsAsync();
                var _dic1 =AddInsertResponseToLogEvent(obj);
                await tableService1.WriteAsync(_dic1);

                //push to table user
                var tableName = string.Format("{0}{1}", TVAN_CONST.EventTableCloudResponse.EventInvoice, TaxCode.Replace("-", ""));
                var tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{tableName}");
                await tableService.CreateTableIfNotExistsAsync();
                var _dic =AddInsertResponseToLogEvent(obj);
                await tableService.WriteAsync(_dic);
                return TVAN_CONST.TAG_QUEUE.SUCCESS;
            }
            catch (Exception ex)
            {
                return TVAN_CONST.TAG_QUEUE.ERROR;
            }
        }

        //Addinsert data to table cloud log event
        private Dictionary<string, object> AddInsertResponseToLogEvent(LogEventModel obj)
        {
            if (obj != null)
            {
                var _dic = new Dictionary<string, object>();
                _dic.Add("PartitionKey", obj.MaThongDiepThamChieu);
                _dic.Add("RowKey", obj.MaThongDiep);
                _dic.Add("MaLoaiThongDiep", obj.MLTDiep);
                _dic.Add("TrangThai", obj.TrangThai);
                _dic.Add("NoiGui", obj.NoiGui);
                _dic.Add("NoiNhan", obj.NoiNhan);
                _dic.Add("ThongDiep", obj.ThongDiep);
                _dic.Add("NgayGiaoDich", obj.NgayGiaoDich);
                _dic.Add("FileData", obj.FileData);
                return _dic;
            }
            return null;
        }
    }
}
