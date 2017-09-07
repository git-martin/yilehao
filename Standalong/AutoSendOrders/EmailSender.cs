using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using AutoSendOrders.Models;
using NPOI.SS.UserModel;

namespace AutoSendOrders
{
    public class EmailSender
    {
        public static void SendEmail(List<Order> list, DateTime start, DateTime end)
        {
            var smtp = MyConfig.SmtpInfoConfig;
            var msg = new MailMessage
            {
                From = new MailAddress(smtp.EmailAccount, smtp.MailNickName),
                Subject = string.Format("亿乐豪：{0}日订单汇总", DateTime.Now.ToString("yyyy-MM-dd")),
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Body = BuildEmailBody(list, start, end),
            };
            if (MyConfig.ToEmails.Any())
                MyConfig.ToEmails.ForEach(a => msg.To.Add(a));
            if (MyConfig.CcEmails.Any())
                MyConfig.CcEmails.ForEach(a => msg.CC.Add(a));
            if (MyConfig.BccEmails.Any())
                MyConfig.BccEmails.ForEach(a => msg.Bcc.Add(a));
            var client = new SmtpClient
            {
                Host = smtp.Host,
                //QQ 企业邮箱不支持用SSL
                //Port = 465,
                //EnableSsl = true,
                Port = smtp.Port,
                EnableSsl = smtp.EnableSsl,
                Timeout = 120 * 1000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtp.EmailAccount, smtp.EmailPassword),
            };
            var book = BuildExcelBook(list);
            if (book != null)
            {

                // 写入到客户端   
                var ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var dt = DateTime.Now;
                var dateTime = dt.ToString("yyMMddHHmmssfff");
                var fileName = "订单列表" + dateTime + ".xls";
                msg.Attachments.Add(new Attachment(ms, fileName));
            }
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(msg);//发送邮件  
        }

        private static string BuildEmailBody(List<Order> list, DateTime start, DateTime end)
        {
            var total = 0;
            var unshipped = 0;
            var detalHtml = "<div style='border-top:1px dotted #ccc; color: #0000ff;font-size: 16px;padding:10px'>今日无订单，请进网站后台确认</div>";
            if (list != null && list.Any())
            {
                total = list.Count;
                unshipped = list.Count(x => x.ShippingStatus == ShippingStatus.Unshipped);
            }
            var tmp = BuildOrderDetailsHtml(list);
            if (!string.IsNullOrWhiteSpace(tmp))
                detalHtml = tmp;

            var sb = new StringBuilder();
            sb.Append("<div style='color:#0000ff;'>");
            sb.AppendFormat(@"
            <h3>每日待发货订单汇总：</h3> 
            <ul style='border-left: 10px solid #ccc;color:#0000ff;line-height: 24px;'>
            <li>统计时间：{0}至{1}</li>
            <li>总订单数：{2}</li>
            <li>未发货单：{3}</li>
            </ul>
            <div>{4}</div>
            <div style='border-top:1px dotted #ccc; color: #0000ff;font-size: 12px;'>
                注：
                <br> - 每天统计时间段是前天是15：00（不包含）至今天的15：00（包含）
                <br> - 订单发货信息如果没有及时录入系统则可能订单重复，请注意！！！
            </div>", start.ToString("yyyy-MM-dd hh:MM:ss"),
                   end.ToString("yyyy-MM-dd hh:MM:ss"),
                   total,
                   unshipped,
                   detalHtml
                   );
            sb.Append("</div>");
            return sb.ToString();
        }

        private static string BuildOrderDetailsHtml(List<Order> list)
        {
            if (list == null || !list.Any())
                return string.Empty;

            var builder = new StringBuilder("<table cellspacing='0' style='border: solid 1px #a9c6c9;border-collapse: collapse;'>");
            builder.Append("<tr align='left' valign='top' style='border: solid 1px #a9c6c9;font-weight: bold;padding: 8px;background-color:#c3dde0;'>");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "序号");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "订单号");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "商品名称");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "规格");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "数量");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "下单时间");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "收货人");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "收货地址");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "联系电话");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "商品总价");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "物流费用");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "积分折抵");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "应付金额");
            builder.AppendFormat("<td align='left' valign='top'>{0}</td>", "订单状态");
            builder.Append("</tr>");
            var i = 0;
            foreach (var item in list)
            {
                i++;
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
                        m += item.OrderGoods[j].GoodsAttribute + "\n";
                        k += item.OrderGoods[j].Quantity + item.OrderGoods[j].Unit + "\n";
                    }
                }
                var tdSyleAlt = "style='border:1px solid ##a9c6c9;padding:8px;background-color:#d4e3e5;border-collapse: collapse;'";
                var tdSyleAltOdd = "style='border:1px solid ##a9c6c9;padding:8px;background-color:#c3dde0;border-collapse: collapse;'";
                var rowStyle = i%2 == 1 ? tdSyleAlt : tdSyleAltOdd;
                builder.Append("<tr align='left' valign='top'>");
                builder.AppendFormat("<td {1}>{0}</td>", i,rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.OrderNo, rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", n, rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", m, rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", k, rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.CreateTime, rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.Consignee, rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.PCDS + item.Address, rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.Tel, rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.GoodsAmount.ToString("#0.00"), rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.ShippingFee.ToString("#0.00"), rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.IntegralMoney.ToString("#0.00"), rowStyle);
                builder.AppendFormat("<td {1}>{0}</td>", item.PayFee.ToString("#0.00"), rowStyle);
                var statusName = item.OrderStatus.Description();
                if (item.RefundStatus > 0)
                    statusName += "(" + item.RefundStatus.Description() + ")";
                if (item.EvaluateStatus > 0)
                    statusName += "(" + item.EvaluateStatus.Description() + ")";
                builder.AppendFormat("<td {1}>{0}</td>", statusName, rowStyle);
                builder.Append("</tr>");
            }
            builder.Append("</table>");
            return builder.ToString();
        }

        private static NPOI.HSSF.UserModel.HSSFWorkbook BuildExcelBook(List<Order> list)
        {
            if (list == null || !list.Any())
                return null;
            //创建Excel文件的对象  
            var book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet  
            ISheet sheet1 = book.CreateSheet("订单");

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
                        m += item.OrderGoods[j].GoodsAttribute + "\n";
                        k += item.OrderGoods[j].Quantity + item.OrderGoods[j].Unit + "\n";
                    }

                }
                IRow rowtemp = sheet1.CreateRow(i + 1);
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
                rowtemp.CreateCell(6).SetCellValue(item.PCDS + item.Address);
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
            return book;
        }

        public static List<Order> GetTodayOrders(DateTime start, DateTime end)
        {
            using (var dbContex = new OrderDbContext())
            {
                var query = dbContex.Orders.Include(s => s.OrderGoods).AsNoTracking()
                    .Where(o => o.PayStatus == PayStatus.Paid
                        //&& o.ShippingStatus == ShippingStatus.Shipped
                        && o.PayTime > start
                        && o.PayTime <= end);
                var data = query.ToList();
                return data;
            }
        }
    }
}
