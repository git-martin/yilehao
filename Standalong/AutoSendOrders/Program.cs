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
                _timer.Interval = MyConfig.OrderCheckTimeSpan * 100;
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
            try
            {
                _timer.Enabled = false;
                SendOrderEmail();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendOrderEmail()
        {
            var checkPoint = DateTime.Now.Date.AddHours(15);
            var now = DateTime.Now;
            var span = (now - checkPoint).TotalSeconds;
            bool needSendEmail = 0 < span && span < MyConfig.OrderCheckTimeSpan;
            if (!needSendEmail)
            {
                if (now.Minute % 10 == 8)
                {
                    Console.WriteLine(" {0} - Moniting...", now.ToString("yyyy-MM-dd hh:MM:ss"));
                }
                return;
            }
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
            EmailSender.SendEmail(data, start, checkPoint);
            Console.WriteLine(">> email sent.");

        }
    }
}
