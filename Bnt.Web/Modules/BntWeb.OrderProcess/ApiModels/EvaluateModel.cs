using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BntWeb.FileSystems.Media;

namespace BntWeb.OrderProcess.ApiModels
{
    public class CreateEvaluateListModel
    {
        //public Guid OrderId { get; set; }
        public List<CreateEvaluateModel> Evaluates { get; set; }
    }

    public class CreateEvaluateModel
    {
        /// <summary>
        /// 订单商品单品id
        /// </summary>
        public Guid SingleGoodsId { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [MaxLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnonymity { get; set; }
        /// <summary>
        /// 评价照片
        /// </summary>
        public List<Guid> FilesId { get; set; }
    }

    /// <summary>
    /// 评价详情
    /// </summary>
    public class EvaluateDetailsModel
    {
        /// <summary>
        /// 订单商品id
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 订单单品id
        /// </summary>
        public Guid SingleGoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 属性组合值
        /// </summary>
        public string GoodsAttribute { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 主图
        /// </summary>
        public SimplifiedStorageFile MainImage { set; get; }

        /// <summary>
        /// 评分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnonymity { get; set; }

        /// <summary>
        /// 评价人名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 评价时间
        /// </summary>
        public DateTime EvaluateTime { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ReplyTime { get; set; }

    }
}