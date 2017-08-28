/* 
    ======================================================================== 
        File name：        MarkupService
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/6/22 14:14:21
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.MemberBase;
using BntWeb.Security;
using BntWeb.Services;
using BntWeb.SystemMessage.Models;
using BntWeb.Utility.Extensions;

namespace BntWeb.SystemMessage.Services
{
    public class SystemMessageService : ISystemMessageService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IPushService _pushService;
        private readonly IStorageFileService _storageFileService;
        public SystemMessageService(ICurrencyService currencyService, IPushService pushService, IStorageFileService storageFileService)
        {
            _currencyService = currencyService;
            _pushService = pushService;
            _storageFileService = storageFileService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// 创建新的系统消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="recieveId"></param>
        /// <param name="sourceId"></param>
        /// <param name="sourceType"></param>
        /// <param name="moduleKey"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool CreateSystemMessage(string title, string content, string recieveId, Guid? sourceId, string sourceType = "", string moduleKey = "", MessageCategory category = MessageCategory.System)
        {
            return InsertSystemMessage(title, content, recieveId, sourceId, sourceType, moduleKey, category);
        }

        /// <summary>
        /// 发送推送消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="pushContent"></param>
        /// <param name="recieveId"></param>
        /// <param name="sourceId"></param>
        /// <param name="pushParas"></param>
        /// <param name="sourceType"></param>
        /// <param name="moduleKey"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool CreatePushSystemMessage(string title, string content, string pushContent, string recieveId, Guid? sourceId, Dictionary<string, string> pushParas = null, string sourceType = "", string moduleKey = "", MessageCategory category = MessageCategory.System)
        {
            try
            {
                var success = InsertSystemMessage(title, content, recieveId, sourceId, sourceType, moduleKey, category);
                if (success)
                {
                    if (string.IsNullOrWhiteSpace(pushContent))
                    {
                        pushContent = "你有一条新消息";
                    }
                    if (recieveId == null)
                    {
                        _pushService.Push(pushContent, pushParas);
                    }
                    else
                    {
                        _pushService.Push(pushContent, recieveId.ToString(), pushParas);
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "发送消息失败");
                return false;
            }
        }


        /// <summary>
        /// 插入系统信息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="recieveId"></param>
        /// <param name="sourceId"></param>
        /// <param name="sourceType"></param>
        /// <param name="moduleKey"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private bool InsertSystemMessage(string title, string content, string recieveId, Guid? sourceId, string sourceType, string moduleKey, MessageCategory category = MessageCategory.System)
        {
            using (var dbContext = new SystemMessageDbContext())
            {
                var systemMessage = new Models.SystemMessage
                {
                    Id = KeyGenerator.GetGuidKey(),
                    Title = title,
                    Content = content,
                    SourceId = sourceId,
                    MessageType = recieveId == null ? MessageType.Public : MessageType.Personal,
                    CreateTime = DateTime.Now,
                    SourceType = sourceType,
                    ModuleKey = moduleKey,
                    Category = category
                };
                dbContext.SystemMessages.Add(systemMessage);
                if (recieveId != null)
                {
                    var systemMessageReciever = new SystemMessageReciever
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        MessageId = systemMessage.Id,
                        RecieveId = recieveId,
                        CreateTime = DateTime.Now,
                        Status = Status.UnRead
                    };
                    dbContext.SystemMessageRecievers.Add(systemMessageReciever);
                }

                var result = dbContext.SaveChanges();
                return result > 0;
            }
        }

        public SystemMessageUnion GetFirstMessage(MessageCategory category, string recieveId, out int unreadCount)
        {
            using (var dbContext = new SystemMessageDbContext())
            {
                var unreadCountQuery = from m in dbContext.SystemMessages
                                       where m.Category == category &&
                                       //未读的私有消息
                                       ((dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(recieveId) && mr.Status == Status.UnRead) && m.MessageType == MessageType.Personal) ||

                                       //还不存在记录的公有消息
                                       (!dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(recieveId))

                                       && m.MessageType == MessageType.Public)
                                       )
                                       select new { };
                unreadCount = unreadCountQuery.Count();


                var query = from m in dbContext.SystemMessages
                            where m.Category == category &&
                            //没有删除的私有消息
                            ((dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(recieveId) && mr.Status != Status.Deleted) && m.MessageType == MessageType.Personal) ||

                            //存在并没有删除的公有消息
                            ((dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(recieveId) && mr.Status != Status.Deleted) ||
                            //还不存在记录的公有消息
                            (!dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(recieveId)))

                            && m.MessageType == MessageType.Public))
                            )
                            orderby m.CreateTime descending
                            select new SystemMessageUnion { Id = m.Id, Category = m.Category, Title = m.Title, Content = m.Content, CreateTime = m.CreateTime, SourceId = m.SourceId, SourceType = m.SourceType, HasRead = dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(recieveId) && mr.Status == Status.HasRead) };
                var message = query.FirstOrDefault();
                if (message == null) return null;
                return message;
            }
        }


        /// <summary>
        /// 分页加载系统消息
        /// </summary>
        /// <param name="category"></param>
        /// <param name="memberId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public IEnumerable<SystemMessageUnion> LoadSystemMessageByPage(MessageCategory category, string memberId, int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new SystemMessageDbContext())
            {
                var queryList = from m in dbContext.SystemMessages
                                where m.Category == category &&
                                //没有删除的私有消息
                                ((dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(memberId) && mr.Status != Status.Deleted) && m.MessageType == MessageType.Personal) ||

                                //存在并没有删除的公有消息
                                ((dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(memberId) && mr.Status != Status.Deleted) ||
                                //还不存在记录的公有消息
                                (!dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(memberId)))

                                && m.MessageType == MessageType.Public))
                                )


                                select new SystemMessageUnion { Id = m.Id, Category = m.Category, Title = m.Title, Content = m.Content, CreateTime = m.CreateTime, SourceId = m.SourceId, SourceType = m.SourceType, HasRead = dbContext.SystemMessageRecievers.Any(mr => mr.MessageId == m.Id && mr.RecieveId.Equals(memberId) && mr.Status == Status.HasRead) };
                totalCount = queryList.Count();
                return queryList.OrderBy(o => o.HasRead).ThenByDescending(o => o.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            }
        }

        public bool SetRead(string memberId, Guid messageId)
        {
            var message = _currencyService.GetSingleById<Models.SystemMessage>(messageId);

            return SetMessageRead(message, memberId);
        }

        public bool SetRead(string memberId, MessageCategory category)
        {
            using (var dbContext = new SystemMessageDbContext())
            {
                var query = from m in dbContext.SystemMessages
                            where m.Category == category && !dbContext.SystemMessageRecievers.Any(
                                mr =>
                                    mr.MessageId == m.Id && mr.Status != Status.UnRead &&
                                    mr.RecieveId == memberId)
                            select m;
                foreach (var message in query.ToList())
                {
                    SetMessageRead(message, memberId);
                }

                return true;
            }
        }

        private bool SetMessageRead(Models.SystemMessage message, string memberId)
        {
            if (message.MessageType == MessageType.Personal)
            {
                using (var dbContext = new SystemMessageDbContext())
                {
                    var query = (from mr in dbContext.SystemMessageRecievers
                                 where mr.MessageId == message.Id && mr.RecieveId == memberId && mr.Status == Status.UnRead
                                 select mr).ToList();
                    foreach (var item in query)
                    {
                        item.Status = Status.HasRead;
                        dbContext.Set<SystemMessageReciever>().Attach(item);
                        dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                    dbContext.SaveChanges();
                    return true;
                }
            }
            if (message.MessageType == MessageType.Public)
            {
                using (var dbContext = new SystemMessageDbContext())
                {

                    var query = (from mr in dbContext.SystemMessageRecievers
                                 where mr.MessageId == message.Id && mr.RecieveId == memberId
                                 select mr).ToList();
                    if (query.Count > 0)
                    {
                        foreach (var item in query)
                        {
                            if (item.Status == Status.UnRead)
                            {
                                item.Status = Status.HasRead;
                                dbContext.Set<SystemMessageReciever>().Attach(item);
                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                    }
                    else
                    {
                        var messageReciever = new SystemMessageReciever();
                        messageReciever.Id = KeyGenerator.GetGuidKey();
                        messageReciever.MessageId = message.Id;
                        messageReciever.RecieveId = memberId;
                        messageReciever.Status = Status.HasRead;
                        messageReciever.CreateTime = DateTime.Now;
                        messageReciever.ReadTime = DateTime.Now;
                        dbContext.SystemMessageRecievers.Add(messageReciever);
                    }
                    dbContext.SaveChanges();
                    return true;
                }
            }

            return true;
        }

        public bool Delete(string memberId, Guid messageId)
        {
            var message = _currencyService.GetSingleById<Models.SystemMessage>(messageId);
            if (message == null) return true;
            bool result;
            if (message.MessageType == MessageType.Personal)
            {
                //删除消息和接收人关联数据
                _currencyService.DeleteByConditon<Models.SystemMessage>(s => s.Id.Equals(messageId));
                _currencyService.DeleteByConditon<SystemMessageReciever>(s => s.MessageId.Equals(messageId));

                result = true;
            }
            else
            {
                var systemMessageReciever = _currencyService.GetSingleByConditon<SystemMessageReciever>(me => me.RecieveId == memberId && me.MessageId == messageId);
                if (systemMessageReciever != null && systemMessageReciever.Status != Status.Deleted)
                {
                    systemMessageReciever.Status = Status.Deleted;
                    result = _currencyService.Update(systemMessageReciever);
                }
                else
                {
                    var messageReciever = new SystemMessageReciever();
                    messageReciever.Id = KeyGenerator.GetGuidKey();
                    messageReciever.MessageId = message.Id;
                    messageReciever.RecieveId = memberId;
                    messageReciever.Status = Status.Deleted;
                    messageReciever.CreateTime = DateTime.Now;
                    result = _currencyService.Create(messageReciever);
                }
            }

            if (result)
                Logger.Operation($"会员-{memberId}删除消息-{message.Id}", SystemMessageModule.Instance);

            return result;
        }

        public IEnumerable<Models.SystemMessage> GetSystemMessageListByPage(int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new SystemMessageDbContext())
            {
                var query = from m in dbContext.SystemMessages
                            where m.Category == MessageCategory.System && m.SourceType.Equals("System", StringComparison.OrdinalIgnoreCase)
                            select m;
                totalCount = query.Count();
                var list = query.OrderByDescending(s => s.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                foreach (var message in list)
                {
                    message.ReadCount = dbContext.SystemMessageRecievers.Count(mr => mr.MessageId == message.Id && mr.Status == Status.HasRead);
                }

                return list;
            }
        }
    }

}