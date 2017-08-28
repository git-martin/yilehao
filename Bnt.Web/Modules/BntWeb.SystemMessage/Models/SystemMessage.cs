/* 
    ======================================================================== 
        File name：		Activity
        Module:			
        Author：		Daniel.Wu（wujb）
        Create Time：		2016/6/17 16:26:51
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BntWeb.Data;
using BntWeb.FileSystems.Media;

namespace BntWeb.SystemMessage.Models
{
    [Table(KeyGenerator.TablePrefix + "system_messages")]
    public class SystemMessage
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [MaxLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// 是否公开
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public MessageCategory Category { get; set; } = MessageCategory.System;

        /// <summary>
        /// 来源Id
        /// </summary>
        public Guid? SourceId { get; set; }

        /// <summary>
        /// 模块Key
        /// </summary>
        public string ModuleKey { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        [NotMapped]
        public int ReadCount { get; set; }
    }


    /// <summary>
    /// 此枚举根据用户需求进行修改
    /// </summary>
    public enum MessageCategory
    {
        [Description("系统消息")]
        System = 0,

        [Description("我的消息")]
        Personal = 1,

        [Description("我的活动")]
        Activity = 2,

        [Description("我的办公")]
        Office = 3,
    }
    /// <summary>
    /// 推送目标类型
    /// </summary>
    public enum PushTargetType
    {
        /// <summary>
        /// 消息中心
        /// </summary>
        [Description("消息中心")]
        System = 1,
        /// <summary>
        /// 优惠券列表
        /// </summary>
        [Description("优惠券列表")]
        Coupon = 2,
        /// <summary>
        /// 折扣商品
        /// </summary>
        [Description("折扣商品")]
        DiscountGoods = 3,
        /// <summary>
        /// 普通商品
        /// </summary>
        [Description("普通商品")]
        Goods = 4,
        /// <summary>
        /// 订单列表
        /// </summary>
        [Description("订单列表")]
        Order = 5
    }

}