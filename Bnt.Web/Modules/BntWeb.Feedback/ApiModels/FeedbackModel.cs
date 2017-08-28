using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BntWeb.Feedback.Models;
using BntWeb.FileSystems.Media;

namespace BntWeb.Feedback.ApiModels
{
    public class CreateFeedbackModel
    {
        /// <summary>
        /// 反馈类型
        /// </summary>
        public FeedbackType FeedbackType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [MaxLength(1000)]
        public string Content { get; set; }

        /// <summary>
        /// 反馈图片 多图
        /// </summary>
        //public List<Base64Image> ImageFiles { get; set; }
    }

    public class FeedbackListModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; }
        
        public DateTime CreateTime { get; set; }

        public FeedbackListModel(Models.Feedback model)
        {
            Id = model.Id;
            Content = model.Content;
            CreateTime = model.CreateTime;
        }
    }
}