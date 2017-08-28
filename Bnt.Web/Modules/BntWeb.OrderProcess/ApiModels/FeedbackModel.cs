using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BntWeb.FileSystems.Media;

namespace BntWeb.OrderProcess.ApiModels
{
    public class CreateFeedbackModel
    {
        public Guid OrderId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [MaxLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// 反馈图片 多图
        /// </summary>
        public List<Base64Image> ImageFiles { get; set; }
    }
}