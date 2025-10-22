using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using pbs.Helper;

namespace RabbitMQ_EInvoiceT78_Consumer.Base
{
    public class ParsingXmlToSINV
    {
    //    public pbs.BO.RE.SINV Import_SINV_VerNhaNuoc(string message)
    //    {
    //        try
    //        {
    //            var node = XElement.Parse(message);
    //            var ProformaNo = from test in node.Descendants("TTin")
    //                             where test.GetString("TTruong") == "ProformaNo"
    //                             select test.GetString("DLieu");


    //            var obj = pbs.BO.RE.SINV.NewSINV(ProformaNo.FirstOrDefault());
    //            var node_hddv = node.Descendants("HHDVu");
    //            var node_ttc = node.Descendants("TTChung");
    //            var node_ttnm = node.Descendants("NMua");
    //            var node_ttnb = node.Descendants("NBan");
    //            Read_DSHHDVu(node_hddv, ref obj);
    //            Read_TTChung(node_ttc, ref obj);
    //            Read_NDHDon(node_ttnm, node_ttnb, ref obj);
    //            return obj;
    //        }
    //        catch (Exception ex)
    //        {
    //            return null;
    //        }
    //    }

    //    private void Read_TTChung(IEnumerable<XElement> node_ttc, ref pbs.BO.RE.SINV obj)
    //    {
    //        obj.InvDate = (from item in node_ttc
    //                       select item.GetString("TDLap")).FirstOrDefault();
    //        obj.InvPrefix = (from item in node_ttc
    //                         select item.GetString("KHMSHDon")).FirstOrDefault();
    //        obj.InvNo = (from item in node_ttc
    //                     select item.GetString("SHDon")).FirstOrDefault();
    //        obj.Serial = (from item in node_ttc
    //                      select item.GetString("KHHDon")).FirstOrDefault();
    //        obj.ConvCode = (from item in node_ttc
    //                        select item.GetString("DVTTe")).FirstOrDefault();
    //        obj.ConvRate = (from item in node_ttc
    //                        select item.GetString("TGia")).FirstOrDefault();

    //        var node_TTin = node_ttc.Descendants("TTKhac").Descendants("TTin");
    //        obj.Period = (from item in node_TTin
    //                      where item.GetString("TTruong") == "Period"
    //                      select item.GetString("DLieu")).FirstOrDefault();
    //        obj.PurchName = (from item in node_TTin
    //                         where item.GetString("TTruong") == "PurchName"
    //                         select item.GetString("DLieu")).FirstOrDefault();
    //        obj.Notes = (from item in node_TTin
    //                     where item.GetString("TTruong") == "Notes"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi0 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi0"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi1 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi1"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi2 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi2"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi3 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi3"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi4 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi4"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi5 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi5"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi6 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi6"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi7 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi7"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi8 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi8"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.NcSi9 = (from item in node_TTin
    //                     where item.GetString("TTruong") == "NcSi9"
    //                     select item.GetString("DLieu")).FirstOrDefault();
    //        obj.ExtDesc1 = (from item in node_TTin
    //                        where item.GetString("TTruong") == "ExtDesc1"
    //                        select item.GetString("DLieu")).FirstOrDefault();
    //        obj.ExtDesc2 = (from item in node_TTin
    //                        where item.GetString("TTruong") == "ExtDesc2"
    //                        select item.GetString("DLieu")).FirstOrDefault();
    //        obj.ExtDate1 = (from item in node_TTin
    //                        where item.GetString("TTruong") == "ExtDate1"
    //                        select item.GetString("DLieu")).FirstOrDefault();
    //        obj.ExtDate2 = (from item in node_TTin
    //                        where item.GetString("TTruong") == "ExtDate2"
    //                        select item.GetString("DLieu")).FirstOrDefault();
    //        obj.ExtDate3 = (from item in node_TTin
    //                        where item.GetString("TTruong") == "ExtDate3"
    //                        select item.GetString("DLieu")).FirstOrDefault();
    //        obj.ExtDate4 = (from item in node_TTin
    //                        where item.GetString("TTruong") == "ExtDate4"
    //                        select item.GetString("DLieu")).FirstOrDefault();
    //    }

    //    private void Read_NDHDon(IEnumerable<XElement> node_ttnm, IEnumerable<XElement> node_ttnb, ref pbs.BO.RE.SINV obj)
    //    {
    //        // TT nguoi mua
    //        obj.ClientName = (from item in node_ttnm
    //                          select item.GetString("Ten")).FirstOrDefault();
    //        obj.TaxCode = (from item in node_ttnm
    //                       select item.GetString("MST")).FirstOrDefault();
    //        obj.Address = (from item in node_ttnm
    //                       select item.GetString("DChi")).FirstOrDefault();
    //        var node_TTin = node_ttnm.Descendants("TTKhac").Descendants("TTin");
    //        obj.ClientId = (from item in node_TTin
    //                        where item.GetString("TTruong") == "ClientId"
    //                        select item.GetString("DLieu")).FirstOrDefault();
    //        obj.BankCode = obj.ClientId;
    //    }

    //    private void Read_DSHHDVu(IEnumerable<XElement> node_hddv, ref pbs.BO.RE.SINV obj)
    //    {
    //        foreach (var hddv in node_hddv)
    //        {
    //            var obj_line = obj.Lines.AddNew();
    //            var Item_Des = hddv.GetString("Ten").FirstOrDefault().ToString().Split('.');
    //            obj_line.ItemCode = Item_Des[0];
    //            obj_line.Descriptn = Item_Des[1];
    //            obj_line.Unit = hddv.GetString("DVTinh");
    //            obj_line.Qty = hddv.GetString("SLuong");
    //            obj_line.Price = hddv.GetString("DGia");
    //            obj_line.Net = hddv.GetString("ThTien").ToDecimal();
    //            obj_line.VatRate = hddv.GetString("TSuat");
    //            var node_TTin = hddv.Descendants("TTKhac").Descendants("TTin");
    //            obj_line.SpecialLineType = (from item in node_TTin
    //                                        where item.GetString("TTruong") == "SpecialLineType"
    //                                        select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.ExtDesc1 = (from item in node_TTin
    //                                 where item.GetString("TTruong") == "ExtDesc1"
    //                                 select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.Vat = (from item in node_TTin
    //                            where item.GetString("TTruong") == "Vat"
    //                            select item.GetString("DLieu")).FirstOrDefault().ToDecimal();
    //            obj_line.NcSi0 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi0"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.NcSi1 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi1"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.NcSi2 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi2"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.NcSi3 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi3"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.NcSi4 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi4"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.NcSi5 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi5"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.NcSi6 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi6"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.NcSi7 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi7"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.NcSi8 = (from item in node_TTin
    //                              where item.GetString("TTruong") == "NcSi8"
    //                              select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.ExtDesc2 = (from item in node_TTin
    //                                 where item.GetString("TTruong") == "ExtDesc2"
    //                                 select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.ExtDesc3 = (from item in node_TTin
    //                                 where item.GetString("TTruong") == "ExtDesc3"
    //                                 select item.GetString("DLieu")).FirstOrDefault();
    //            obj_line.ExtValue0 = (from item in node_TTin
    //                                  where item.GetString("TTruong") == "ExtValue0"
    //                                  select item.GetString("DLieu")).FirstOrDefault().ToDecimal();
    //            obj_line.ExtValue1 = (from item in node_TTin
    //                                  where item.GetString("TTruong") == "ExtValue1"
    //                                  select item.GetString("DLieu")).FirstOrDefault().ToDecimal();
    //            obj_line.ExtValue2 = (from item in node_TTin
    //                                  where item.GetString("TTruong") == "ExtValue2"
    //                                  select item.GetString("DLieu")).FirstOrDefault().ToDecimal();
    //            obj_line.ExtValue3 = (from item in node_TTin
    //                                  where item.GetString("TTruong") == "ExtValue3"
    //                                  select item.GetString("DLieu")).FirstOrDefault().ToDecimal();
    //            obj_line.ExtValue4 = (from item in node_TTin
    //                                  where item.GetString("TTruong") == "ExtValue4"
    //                                  select item.GetString("DLieu")).FirstOrDefault().ToDecimal();
    //        }
    //    }
    }
}
