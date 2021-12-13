using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_EInvoiceT78_Consumer.Model
{
    public partial class InvoiceModel
    {
        public string ProformaNo {get;set;}
        public string MaThongDiep { get; set; }
        public string InvoiceDate { get; set; }
        public string FriendlyName { get; set; }
        public string MCCQT { get; set; }
        public string fileName { get; set; }

        public string CusMail { get; set; }
        public string ClientName { get; set; }
        public string Serial { get; set; }
        public string InvNo { get; set; }

    }
}
