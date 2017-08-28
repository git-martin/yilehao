/* 
    ======================================================================== 
        File name：        SystemConfig
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/27 13:42:49
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/

using System;
using System.Collections.Generic;
using BntWeb.FileSystems.Media;

namespace BntWeb.Config.Models
{
    public class SystemConfig
    {
        /// <summary>
        /// 库存警告
        /// </summary>
        public int StockWarning { get; set; } = 10;

        /// <summary>
        /// 提现申请超时时间
        /// 单位小时
        /// </summary>
        public int CrashApplyOutTime { get; set; } = 24;

        /// <summary>
        /// 什么时候减库存
        /// </summary>
        public ReduceStock ReduceStock { get; set; } = ReduceStock.AfterPay;

        /// <summary>
        /// 推荐积分
        /// </summary>
        public int RecommendIntegral { get; set; }

        /// <summary>
        /// 消费积分
        /// 消费X元赠送1分
        /// </summary>
        public int ConsumptionIntegral { get; set; }

        /// <summary>
        /// 每100积分折抵X元
        /// </summary>
        public int DiscountRate { get; set; }

        /// <summary>
        /// 最大级别
        /// </summary>
        public int MaxLevel { get; set; } = 3;

        /// <summary>
        /// 每级佣金比例
        /// </summary>
        public List<double> Rates = new List<double>();




        /// <summary>
        /// 水印配置
        /// </summary>
        public WaterMark WaterMark { get; set; } = new WaterMark();

    }

    public enum ReduceStock
    {
        AfterOrder = 0,

        AfterPay = 1,

        AfterDeliver = 2,

        AfterCompleted = 3
    }

    public class SystemConfigViewModel
    {
        /// <summary>
        /// 库存警告
        /// </summary>
        public int StockWarning { get; set; }

        /// <summary>
        /// 提现申请超时时间
        /// 单位小时
        /// </summary>
        public int CrashApplyOutTime { get; set; }

        /// <summary>
        /// 什么时候减库存
        /// </summary>
        public ReduceStock ReduceStock { get; set; }

        /// <summary>
        /// 推荐积分
        /// </summary>
        public int RecommendIntegral { get; set; }

        /// <summary>
        /// 消费积分
        /// 消费X元赠送1分
        /// </summary>
        public int ConsumptionIntegral { get; set; }

        /// <summary>
        /// 每100积分折抵X元
        /// </summary>
        public int DiscountRate { get; set; }

        /// <summary>
        /// 最大级别
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public int[] Rates { get; set; }


        /// <summary>
        /// 水印类型
        /// </summary>
        public WaterMarkType WaterMarkType { get; set; }

        /// <summary>
        /// 水印位置
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 透明度
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// 水印文字
        /// </summary>
        public string WaterMarkText { get; set; }

        /// <summary>
        /// 水印文件
        /// </summary>
        public Guid WaterMarkImage { get; set; }
    }


}