using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AutoSendOrders.Models;
using NPOI.SS.Formula.Functions;
using System.Data.Entity;

namespace AutoSendOrders
{
    class Program
    {
        private static System.Timers.Timer _timer;
        static void Main(string[] args)
        {
            Console.WriteLine("======Auto Sender Order Monitor======");
            try
            {
                _timer = new System.Timers.Timer();
                _timer.Elapsed += timer_Elapsed;
                _timer.Interval = MyConfig.OrderCheckTimeSpan * 1000;
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine(">>monitor is running，please press ESC to close.");
            bool close = false;
            while (!close)
            {
                var a = Console.ReadKey();
                if (a.Key != ConsoleKey.Escape)
                {
                    Console.WriteLine(">>monitor is running，please press ESC to close.");
                }
                else
                {
                    close = true;
                }
            }

        }
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var checkPoint = DateTime.Now.Date.AddHours(18).AddMinutes(54);
            try
            {
                var now = DateTime.Now;
                var span = (now - checkPoint).TotalSeconds;
                if (0< span && span < MyConfig.OrderCheckTimeSpan)
                {
                    var start = checkPoint.AddDays(-1);
                    var data = EmailSender.GetTodayOrders(start, checkPoint);
                    if (data == null || !data.Any())
                    {
                        Console.WriteLine(">> {0} no order found.", now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        Console.WriteLine(">> {0} order count {1}.", now.ToString("yyyy-MM-dd HH:mm:ss"), data.Count);
                    }
                    EmailSender.SendEmail(data);
                    Console.WriteLine(">> email sent.");
                }
                else if(now.Minute%10==8)
                {
                    Console.WriteLine(" {0} - Moniting...", now.ToString("yyyy-MM-dd hh:MM:ss"));
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine();
        }

        public static bool CheckServiceIsRunningByName(string serviceName, bool autoStart)
        {

            //#region send email
            //if (MyConfig.IsSendRemindMail)
            //{
            //    try
            //    {
            //        if (autoStart || MyConfig.OrderCheckTimeSpan >= 100)
            //        {
            //            EmailSender.SendEmail(serviceName, srv.Status.ToString());
            //            Console.WriteLine("Mail  {1}  | send <{0}>remind email succeed.", serviceName, dt);
            //        }
            //        else
            //        {
            //            if (_monitorStatuses.FirstOrDefault(a => String.Equals(a.ServiceName, serviceName, StringComparison.CurrentCultureIgnoreCase)) == null)
            //            {
            //                EmailSender.SendEmail(serviceName, srv.Status.ToString());
            //                _monitorStatuses.Add(new MyConfig.MonitorStatus
            //                {
            //                    //AutoStart = false,
            //                    //CurrentStatus = srv.Status,
            //                    EmailSendStatus = 1,
            //                    LastEmailSendTime = DateTime.Now,
            //                    ServiceName = serviceName,
            //                });
            //                Console.WriteLine("Mail  {1}  | send <{0}>remind email succeed.", serviceName, dt);
            //            }
            //            else
            //            {
            //                var service = _monitorStatuses.FirstOrDefault(a => String.Equals(a.ServiceName, serviceName, StringComparison.CurrentCultureIgnoreCase));
            //                //间隔10分钟才发，而不是每次监控都发（如果监控间隔大雨10分钟则每次都发
            //                if (service != null && service.MinitesNotSendEmail * 60 >= (ulong)sendEmailPerSeconds && service.EmailSendStatus > 0)
            //                {
            //                    EmailSender.SendEmail(serviceName, srv.Status.ToString());
            //                    service.EmailSendStatus = 2;
            //                    service.LastEmailSendTime = DateTime.Now;
            //                    Console.WriteLine("Mail  {1}  | send <{0}>remind email succeed.", serviceName, dt);
            //                }
            //            }
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Mail  {1}  | send <{0}>remind email failed.", serviceName, dt);
            //    }
            //}
            //#endregion
            return true;
        }
    }
}
