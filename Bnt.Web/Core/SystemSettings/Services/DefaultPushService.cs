/* 
    ======================================================================== 
        File name：        DefaultPushService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/13 17:09:30
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using BntWeb.Logging;
using BntWeb.Security.Identity;
using BntWeb.Services;
using cn.jpush.api;
using cn.jpush.api.common;
using cn.jpush.api.common.resp;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;

namespace BntWeb.Core.SystemSettings.Services
{
    public class DefaultPushService : IPushService
    {
        private readonly DefaultUserManager _userManager;
        public DefaultPushService(DefaultUserManager userManager)
        {
            _userManager = userManager;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string AppKey { get; set; }

        public string MasterSecret { get; set; }

        public bool ApnsProduction { get; set; }


        public bool Push(string message, Dictionary<string, string> paras = null)
        {
            PushPayload pushPayload = new PushPayload();
            pushPayload.platform = Platform.all();
            pushPayload.audience = Audience.all();
            pushPayload.notification = new Notification().setAlert(message);
            pushPayload.notification.IosNotification = new IosNotification().setAlert(message).autoBadge();
            pushPayload.notification.AndroidNotification = new AndroidNotification().setAlert(message);

            if (paras != null)
            {
                foreach (var para in paras)
                {
                    pushPayload.notification.IosNotification.AddExtra(para.Key, para.Value);
                    pushPayload.notification.AndroidNotification.AddExtra(para.Key, para.Value);
                }
            }

            pushPayload.options.apns_production = ApnsProduction;
            try
            {
                JPushClient client = new JPushClient(AppKey, MasterSecret);
                client.SendPush(pushPayload);
                return true;
            }
            catch (APIRequestException e)
            {
                Logger.Error("Error response from JPush server. Should review and fix it. HTTP Status: " + e.Status +
                             "Error Code: " + e.ErrorCode + "Error Message: " + e.ErrorCode);
                return false;
            }
            catch (APIConnectionException e)
            {
                Logger.Error(e, "消息推送失败");
                return false;
            }
        }

        public bool Push(string message, User user, Dictionary<string, string> paras = null)
        {
            return Push(message, new List<User> { user }, paras);
        }

        public bool Push(string message, List<User> users, Dictionary<string, string> paras = null)
        {
            JPushClient client = new JPushClient(AppKey, MasterSecret);
            PushPayload pushPayload = new PushPayload();

            pushPayload.platform = Platform.all();
            pushPayload.audience = Audience.s_registrationId(users.Select(u => u.MobileDevice).ToArray());
            pushPayload.notification = new Notification().setAlert(message);
            pushPayload.notification.IosNotification = new IosNotification().setAlert(message).autoBadge();
            pushPayload.notification.AndroidNotification = new AndroidNotification().setAlert(message);

            if (paras != null)
            {
                foreach (var para in paras)
                {
                    pushPayload.notification.IosNotification.AddExtra(para.Key, para.Value);
                    pushPayload.notification.AndroidNotification.AddExtra(para.Key, para.Value);
                }
            }

            pushPayload.options.apns_production = ApnsProduction;
            try
            {
                client.SendPush(pushPayload);
                return true;
            }
            catch (APIRequestException e)
            {
                Logger.Error("Error response from JPush server. Should review and fix it. HTTP Status: " + e.Status +
                             "Error Code: " + e.ErrorCode + "Error Message: " + e.ErrorCode);
                return false;
            }
            catch (APIConnectionException e)
            {
                Logger.Error(e, "消息推送失败");
                return false;
            }
        }

        public bool Push(string message, string userId, Dictionary<string, string> paras = null)
        {
            var user = _userManager.FindByIdAsync(userId);

            return user?.Result != null && Push(message, user.Result, paras);
        }

        public bool Push(string message, List<string> userIds, Dictionary<string, string> paras = null)
        {
            var users = _userManager.Users.Where(u => userIds.Contains(u.Id)).ToList();

            return users.Count > 0 && Push(message, users, paras);
        }
    }
}