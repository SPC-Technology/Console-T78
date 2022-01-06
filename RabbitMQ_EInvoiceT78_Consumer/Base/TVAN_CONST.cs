using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_EInvoiceT78_Consumer.Base
{
    public class TVAN_CONST
    {
        // Storage Account
        public const string STR_StorageAccount = "phoebusfiles";
        public const string STR_LavaData = "lavadata";
        public class TableCloudRequest
        {
            

            //Table cloud register invoice
            public const string STR_RegisterEinvoiceT78 = "RegisterEinvoiceT78";

            //Table cloud Send invoice with code
            public const string STR_SendEinvoiceCodeT78 = "EinvoiceRequestCodeT78";

            //Table cloud send invoice without code
            public const string STR_SendEinvoiceWithoutCodeT78 = "EinvoiceWithoutCodeT78";

            //Table cloud send error invoice 
            public const string STR_SendEinvoiceErrorT78 = "EivoiceErrorT78";

            //Table cloud send Report invoice
            public const string STR_SendReportEinvoiceT78 = "ReportEinvoiceT78";

        }

        public class EventTableCloudResponse
        {
            //Table cloud for user
            public const string EventInvoice = "EventInvoice";

            //Backup SPC table cloud
            public const string EventRegisterInvoice = "EventRegisterInvoice";
            public const string EventInvoiceWithCode = "EventInvoiceWithCode";
            public const string EventInvoiceWithoutCode = "EventInvoiceWithoutCode";
            public const string EventReportInvoice = "EventReportInvoice";
        }

        public class TCT_THONGDIEP
        {
            //Register Invoice message
            public const string Description_Reg = "Thông điệp gửi tờ khai đăng ký/thay đổi thông tin sử dụng hóa đơn điện tử";
            public const string Description_Update = "Thông điệp gửi tờ khai đăng ký thay đổi thông tin đăng ký sử dụng HĐĐT khi ủy nhiệm/nhận ủy nhiệm lập hóa đơn";
            public const string Description_RecieveReg = "Thông điệp thông báo về việc tiếp nhận/không tiếp nhận tờ khai đăng ký/thay đổi thông tin sử dụng HĐĐT, tờ khai đăng ký thay đổi thông tin đăng k‎ý sử dụng HĐĐT khi ủy nhiệm/nhận ủy nhiệm lập hoá đơn";
            public const string Description_AcceptReg = "Thông điệp thông báo về việc chấp nhận/không chấp nhận đăng ký/thay đổi thông tin sử dụng hóa đơn điện tử";
            public const string Description_AcceptUpdate = "Thông điệp thông báo về việc chấp nhận/không chấp nhận đăng ký thay đổi thông tin đăng k‎ý sử dụng HĐĐT khi ủy nhiệm/nhận ủy nhiệm lập hoá đơn";
            public const string Description_ExpiredInv = "Thông điệp thông báo về việc hết thời gian sử dụng hóa đơn điện tử có mã qua cổng thông tin điện tử Tổng cục Thuế/qua ủy thác tổ chức cung cấp dịch vụ về hóa đơn điện tử; không thuộc trường hợp sử dụng hóa đơn điện tử không có mã";


            //Einvoice Request Code
            public const string Description_SendInv = "Thông điệp gửi hóa đơn điện tử tới cơ quan thuế để cấp mã";
            public const string Description_AcceptRequestCodeInv = "Thông điệp thông báo kết quả cấp mã hóa đơn điện tử của cơ quan thuế";
            public const string Description_ErrorRequestCodeInv = "Thông điệp thông báo mẫu số 01/TB-KTDL về việc kết quả kiểm tra dữ liệu hóa đơn điện tử";

            //Erorr Invoice report 
            public const string Description_ReportErorrInvoice = "Thông điệp gửi thông báo về việc tiếp nhận và kết quả xử lý về việc hóa đơn điện tử đã lập có sai sót";

            // Technical Feedback message
            public const string TechnicalFeedback = "Thông điệp phản hồi kỹ thuật";

            public string DescriptionMessage(int TypeMess)
            {
                switch (TypeMess)
                {
                    //Register invoice
                    case 100:
                        return Description_Reg;
                    case 101:
                        return Description_Update;
                    case 102:
                        return Description_RecieveReg;
                    case 103:
                        return Description_AcceptReg;
                    case 104:
                        return Description_AcceptUpdate;
                    case 105:
                        return Description_ExpiredInv;
                    case 200:
                        return Description_SendInv;
                    case 202:
                        return Description_AcceptRequestCodeInv;
                    case 204:
                        return Description_ErrorRequestCodeInv;
                    case 301:
                        return Description_ReportErorrInvoice;
                    case 999:
                        return TechnicalFeedback;

                    default: return "";
                }
            }
        }

        public class STATUS_RESPONSE
        {
            public const string LOI = "Xảy ra lỗi";
            public const string DANHAN = "Đã nhận, đang xử lý";
            public const string DAXULI = "Đã xử lý";
            public const string DKTHANHCONG = "Đăng ký thành công";
            public const string DKLOI = "Đăng ký lỗi";
            public const string SUCCESS_REQUEST_CODE = "Issued";
            public const string ERROR_REQUEST_CODE = "Cấp mã không thành công";
            public const string SUCCESS_REQUEST = "10";
            public const string ERROR_REQUEST = "11";
            public const string CHECK_DATA_ERROR_INVOICE = "Kiểm tra dữ liệu hợp lệ";
            public const string FAILED_HANDLE_ERROR = "Kiểm tra dữ liệu xảy ra lỗi";
            public const string ACCEPT_ERROR_INVOICE_REPORT = "Chấp nhận thông báo sai sót";
            public const string NOT_ACCEPT_ERROR_INVOICE_REPORT = "Không chấp nhận thông báo sai sót";
        }

        public class MA_THONG_DIEP
        {
            //Technical feedback
            public const string TechnicalFeedBack = "999";

            //Register invoice
            public const string RegisterInvoice = "100";
            public const string UpdateRegisterInvoice = "101";
            public const string RecieveRegisterInvoice = "102";
            public const string AcceptRegisterInvoice = "103";

            //Send invoice request code
            public const string SendInvoiceRequestCode = "200";
            public const string SuccessRequestCodeInv = "202";
            public const string ErrorRequestCodeInv = "204";

            //Error invoice report
            public const string SuccessErrorInvoiceReport = "301";
        }

        public class TAG_QUEUE
        {
            public const string SUCCESS = "SUCCESS";
            public const string ERROR = "ERROR";
        }
    }
}
