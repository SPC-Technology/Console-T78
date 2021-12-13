using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_EInvoiceT78_Consumer.Log_Event.Log_Evnet_Model
{
    public class LogEventModel
    {
        public string NoiGui { get; set; }
        public string NoiNhan { get; set; }
        public string NgayGiaoDich { get; set; }
        public string MaThongDiep { get; set; }
        public string MaThongDiepThamChieu { get; set; }
        public string ThongDiep { get; set; }
        public string MLTDiep { get; set; }
        public string TrangThai { get; set; }
        public string FileData { get; set; }

    }
}
