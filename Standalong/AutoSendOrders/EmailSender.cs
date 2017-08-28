using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using AutoSendOrders.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace AutoSendOrders
{
    public class EmailSender
    {

        public static void SendEmail()
        {
            const string serviceName = "test";
            var smtp = MyConfig.SmtpInfoConfig;
            var msg = new MailMessage();
            msg.To.Add(MyConfig.ToEmails);//收件人地址  
            msg.From = new MailAddress(smtp.EmailAccount, smtp.MailNickName);//发件人邮箱，名称
            msg.Subject = string.Format("警告:检查到服务<{0}>未运行！", "");//邮件标题  
            msg.SubjectEncoding = Encoding.UTF8;//标题格式为UTF8  
            var sb = new StringBuilder();
            sb.Append("<div style='color:#0000ff;'>");
            sb.AppendFormat(@"
                    <h3>监控人员请注意，发现服务【{0}】未正常运行：</h3>
                    <ul style='border-left: 10px solid #ccc;color:#0000ff;'>
                        <li>检查时间：{1}</li>
                    </ul>", serviceName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append(@"<div style='border-top:1px dotted #ccc; color: #0000ff;font-size: 12px;'>
	注：<br> - 在发现服务未运行时即发送次邮件，并根据配置会尝试启动服务;<br> - CPU,内存等信息都是瞬时状态（不代表常态），仅供参考；<br> - StartPending状态代表服务由监控程序启动中，如果后续未收到监控邮件说明启动成功；
	</div>");
            sb.Append("<div>");
            msg.BodyEncoding = Encoding.UTF8;//内容格式为UTF8  
            msg.IsBodyHtml = true;
            msg.Body = sb.ToString();
            var client = new SmtpClient
            {
                Host = "smtp.exmail.qq.com",
                //QQ 企业邮箱不支持用SSL
                //Port = 465,
                //EnableSsl = true,
                Port = 25,
                EnableSsl = false,
                Timeout = 120 * 1000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtp.EmailAccount, smtp.EmailPassword),
            };
            var list = GetTodayOrders(DateTime.Now, DateTime.Now);

            //创建Excel文件的对象  
            var book = new NPOI.HSSF.UserModel.HSSFWorkbook();

            //添加一个sheet  
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            if (list.Any())
            {
                //给sheet1添加第一行的头部标题  
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("订单号");
                row1.CreateCell(1).SetCellValue("商品名称");
                row1.CreateCell(2).SetCellValue("规格");
                row1.CreateCell(3).SetCellValue("数量");
                row1.CreateCell(4).SetCellValue("下单时间");
                row1.CreateCell(5).SetCellValue("收货人");
                row1.CreateCell(6).SetCellValue("收货地址");
                row1.CreateCell(7).SetCellValue("联系电话");
                row1.CreateCell(8).SetCellValue("商品总价");
                row1.CreateCell(9).SetCellValue("物流费用");
                row1.CreateCell(10).SetCellValue("积分折抵");
                row1.CreateCell(11).SetCellValue("应付金额");
                row1.CreateCell(12).SetCellValue("订单状态");
                var i = 0;
                var cs = book.CreateCellStyle();
                cs.WrapText = true;
                foreach (var item in list)
                {
                    string n = "";
                    string m = "";
                    string k = "";
                    for (int j = 0; j < item.OrderGoods.Count; j++)
                    {
                        if (j == item.OrderGoods.Count - 1)
                        {
                            n += item.OrderGoods[j].GoodsName;
                            m += item.OrderGoods[j].GoodsAttribute;
                            k += item.OrderGoods[j].Quantity + item.OrderGoods[j].Unit;
                        }
                        else
                        {
                            n += item.OrderGoods[j].GoodsName + "\n";
                            m += item.OrderGoods[j].GoodsAttribute+"\n";
                            k += item.OrderGoods[j].Quantity + item.OrderGoods[j].Unit+"\n";
                        }

                    }
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(item.OrderNo);
                    rowtemp.CreateCell(1).SetCellValue(n);
                    rowtemp.CreateCell(2).SetCellValue(m);
                    rowtemp.CreateCell(3).SetCellValue(k);
                    if (item.OrderGoods.Count > 1)
                    {
                        rowtemp.GetCell(1).CellStyle = cs;
                        rowtemp.GetCell(2).CellStyle = cs;
                        rowtemp.GetCell(3).CellStyle = cs;
                    }
                    rowtemp.CreateCell(4).SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.CreateTime));
                    rowtemp.CreateCell(5).SetCellValue(item.Consignee);
                    rowtemp.CreateCell(6).SetCellValue(item.PCDS+item.Address);
                    rowtemp.CreateCell(7).SetCellValue(item.Tel);
                    rowtemp.CreateCell(8).SetCellValue(item.GoodsAmount.ToString("#0.00"));
                    rowtemp.CreateCell(9).SetCellValue(item.ShippingFee.ToString("#0.00"));
                    rowtemp.CreateCell(10).SetCellValue(item.IntegralMoney.ToString("#0.00"));
                    rowtemp.CreateCell(11).SetCellValue(item.PayFee.ToString("#0.00"));
                    var statusName = item.OrderStatus.Description();
                    if (item.RefundStatus > 0)
                        statusName += "(" + item.RefundStatus.Description() + ")";
                    if (item.EvaluateStatus > 0)
                        statusName += "(" + item.EvaluateStatus.Description() + ")";
                    rowtemp.CreateCell(12).SetCellValue(statusName);
                    i++;
                }
            }
            // 写入到客户端   
            var ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var dt = DateTime.Now;
            var dateTime = dt.ToString("yyMMddHHmmssfff");
            var fileName = "订单列表" + dateTime + ".xls";

            msg.Attachments.Add(new Attachment(ms,fileName));

            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(msg);//发送邮件  
        }

        public static List<Order> GetTodayOrders(DateTime start, DateTime end)
        {
            using (var dbContex = new OrderDbContext())
            {
                var query = dbContex.Orders.Include(s => s.OrderGoods).AsNoTracking().Where(o => o.PayStatus == PayStatus.Paid && o.ShippingStatus == ShippingStatus.Shipped);
                var data = query.ToList();
                return data;
            }
        }
    }
}
