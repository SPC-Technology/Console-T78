using RabbitMQ_EInvoiceT78_Consumer.Log_Event.Log_Evnet_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ_EInvoiceT78_Consumer.Base;
using System.Xml.Linq;
using pbs.Helper;

namespace RabbitMQ_EInvoiceT78_Consumer.DataAccess_Cloud
{
    public class RegisterInvoiceEvent
    {
        public async Task UpdateStatusRegisterInvoiceMaster(string maThongDiep,string MST,string Status)
        {

            try
            {
                var tableService = await SPC.ServicesContainer.ShortCut.AzureTable.GetTableServiceAsync($"{TVAN_CONST.STR_StorageAccount}", $"{TVAN_CONST.TableCloudRequest.STR_RegisterEinvoiceT78}");
                await tableService.CreateTableIfNotExistsAsync();
                var data = await tableService.ReadAsync($"{MST}:{maThongDiep}");
                data["STATUS"] = Status;
                await tableService.WriteAsync(data);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
