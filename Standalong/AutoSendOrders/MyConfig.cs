using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;

namespace AutoSendOrders
{
    public static class MyConfig
    {
        public class QqEnterpriseMailSmtpInfo
        {
            public string EmailAccount { get; set; }
            public string EmailPassword { get; set; }
            public string MailNickName { get; set; }

            public int Port { get { return 25; } }
            public string Host { get { return "smtp.exmail.qq.com"; } }
            public bool EnableSsl { get { return false; } }
        }

        public class MonitorStatus
        {
            public string ServiceName { get; set; }
            public bool AutoStart { get; set; }
            public ServiceControllerStatus CurrentStatus { get; set; }
            public DateTime LastEmailSendTime { get; set; }
            public int EmailSendStatus { get; set; }

            public ulong MinitesNotSendEmail
            {
                get
                {
                    if (LastEmailSendTime == DateTime.MinValue)
                        return ulong.MaxValue;
                    else
                    {
                        return (ulong)(DateTime.Now - LastEmailSendTime).TotalMinutes;
                    }
                }
            }
        }

        /// <summary>
        /// 表名字
        /// </summary>
        public static QqEnterpriseMailSmtpInfo SmtpInfoConfig
        {
            get
            {
                var value = ConfigurationManager.AppSettings["QQEnterpriseMailSmtpInfo"].ToString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("QQEnterpriseMailSmtpInfo未配置或者配置不正确");
                }
                var infos = value.Split(':');
                if (infos == null || infos.Length != 3)
                {
                    throw new Exception("QQEnterpriseMailSmtpInfo格式配置不正确");
                }
                var ret = new QqEnterpriseMailSmtpInfo
                {
                    EmailAccount = infos[0],
                    EmailPassword = infos[1],
                    MailNickName = infos[2],
                };
                return ret;
            }
        }


        /// <summary>
        /// </summary>
        public static List<string> ToEmails
        {
            get
            {
                var value = ConfigurationManager.AppSettings["ToEmails"].ToString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("ToEmails未配置或者配置不正确");
                }
                var tmp = value.Split(';');
                return tmp.ToList();
            }
        }

        /// <summary>
        /// </summary>
        public static List<string> BccEmails
        {
            get
            {
                var value = ConfigurationManager.AppSettings["BccEmails"].ToString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("CcEmails未配置或者配置不正确");
                }
                var tmp = value.Split(';');
                return tmp.ToList();
            }
        }

        /// <summary>
        /// </summary>
        public static bool IsSendRemindMail
        {
            get
            {
                var boolValue = false;
                var value = ConfigurationManager.AppSettings["IsSendRemindMail"];
                if (string.IsNullOrWhiteSpace(value) || bool.TryParse(value, out boolValue) == false)
                {
                    throw new Exception("IsSendRemindMail未配置或者配置不正确");
                }
                return boolValue;
            }
        }

        /// <summary>
        /// </summary>
        public static int OrderCheckTimeSpan
        {
            get
            {
                int ret = 0;
                var value = ConfigurationManager.AppSettings["OrderCheckTimeSpan"].ToString();
                if (string.IsNullOrWhiteSpace(value) || int.TryParse(value, out ret) == false)
                {
                    throw new Exception("OrderCheckTimeSpan未配置或者配置不正确");
                }
                return ret; ;
            }
        }

    }
}
